using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using SuchByte.MacroDeck.DataTypes.FileDownloader;
using SuchByte.MacroDeck.DataTypes.Updater;
using SuchByte.MacroDeck.Enums;
using Serilog;
using SuchByte.MacroDeck.Extension;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Services;

/// <summary>
/// 更新服务，负责检查、下载和安装 Macro Deck 的新版本。
/// 采用单例模式，通过定时轮询检查更新，使用信号量控制并发，
/// 并在下载完成后自动启动静默安装程序。
/// </summary>
public class UpdateService : IDisposable
{
    private static readonly ILogger Logger = Log.ForContext(typeof(UpdateService));

    // Make UpdateService singleton
    private static UpdateService? _instance;

    /// <summary>
    /// 获取 UpdateService 的单例实例。
    /// </summary>
    public static UpdateService Instance()
    {
        _instance ??= new UpdateService();
        return _instance;
    }

    /// <summary>
    /// 当前平台的标识符，固定为 WinX64。
    /// </summary>
    public const PlatformIdentifier PlatformIdentifier = Enums.PlatformIdentifier.WinX64;
    /// <summary>
    /// 更新检查 API 的基础 URL。
    /// </summary>
    private const string UpdateServiceApiUrl = "https://update.api.macro-deck.app/v2";

    /// <summary>
    /// 当检测到新版本可用时触发的事件。
    /// </summary>
    public event EventHandler<UpdateApiVersionInfo>? UpdateAvailable;

    /// <summary>
    /// 用于取消定时更新检查的取消令牌源。
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    /// <summary>
    /// 检查更新时的并发控制信号量（最多允许 1 个并发检查）。
    /// </summary>
    private readonly SemaphoreSlim _checkSemaphoreSlim = new(1);
    /// <summary>
    /// 下载更新时的并发控制信号量（最多允许 1 个并发下载）。
    /// </summary>
    private readonly SemaphoreSlim _downloadSemaphoreSlim = new(1);

    /// <summary>
    /// 最新检测到的可用版本信息。
    /// </summary>
    public UpdateApiVersionInfo? VersionInfo { get; set; }

    /// <summary>
    /// 启动定时更新检查任务。创建一个后台 Task，
    /// 每隔 30 分钟自动检查一次是否有新版本可用。
    /// </summary>
    public void StartPeriodicalUpdateCheck()
    {
        var cancellationToken = _cancellationTokenSource.Token;
        Task.Run(async () => await DoWork(cancellationToken), cancellationToken);
    }

    /// <summary>
    /// 释放更新服务占用的资源，取消定时检查任务。
    /// </summary>
    public void Dispose()
    {
        // 取消定时更新检查循环
        _cancellationTokenSource.Cancel();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 异步检查是否有新版本可用。
    /// 向更新 API 发送当前版本号和平台标识，获取检查结果。
    /// </summary>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>如果有新版本则返回版本信息，否则返回 null。</returns>
    public async Task<UpdateApiVersionInfo?> CheckForUpdatesAsync(CancellationToken cancellationToken)
    {
        var checkUrl = $"{UpdateServiceApiUrl}/versions/check/{MacroDeck.Version.ToString()}/{PlatformIdentifier}";
        if (MacroDeck.Configuration.UpdateBetaVersions)
        {
            checkUrl += "?betaVersions=true";
        }

        // 使用信号量确保同一时间只有一个检查任务在执行
        await _checkSemaphoreSlim.WaitAsync(cancellationToken);

        using var httpClient = new HttpClient();
        try
        {
            var result =
                await httpClient.GetFromJsonAsync<UpdateApiCheckResult>(checkUrl, cancellationToken);
            if (result?.NewerVersionAvailable == null)
            {
                throw new InvalidOperationException("Result was null");
            }

            if (!result.NewerVersionAvailable.Value && VersionInfo == result.Version)
            {
                return null;
            }

            UpdateAvailable?.Invoke(this, result.Version ?? throw new InvalidOperationException("Version was null"));

            VersionInfo = result.Version;
            return result.Version;
        }
        finally
        {
            // 无论成功或失败都要释放信号量
            _checkSemaphoreSlim.Release();
        }
    }

    /// <summary>
    /// 下载指定版本并启动安装程序。
    /// 下载完成后会校验文件哈希，校验通过后自动启动静默安装。
    /// </summary>
    /// <param name="updateApiVersionInfo">要下载的版本信息。</param>
    /// <param name="progress">用于报告下载进度的 IProgress 接口。</param>
    public async ValueTask DownloadAndInstallVersion(
        UpdateApiVersionInfo updateApiVersionInfo,
        IProgress<DownloadProgressInfo> progress)
    {
        // 使用信号量确保同一时间只有一个下载任务在执行
        await _downloadSemaphoreSlim.WaitAsync();

        try
        {
            var versionFileInfo = updateApiVersionInfo.Platforms?[PlatformIdentifier];
            var destinationPath =
                await DownloadUpdate(versionFileInfo?.DownloadUrl, updateApiVersionInfo.Version, progress);

            await VerifyDownloadedFile(destinationPath, versionFileInfo);
            StartInstallation(destinationPath!);
        }
        finally
        {
            _downloadSemaphoreSlim.Release();
        }
    }

    /// <summary>
    /// 启动更新安装程序并退出当前应用。
    /// 使用 Inno Setup 静默安装参数，不显示任何用户界面。
    /// </summary>
    /// <param name="path">安装程序的可执行文件路径。</param>
    private static void StartInstallation(string path)
    {
        new Process
        {
            StartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true,
                Arguments = "/SILENT /SUPPRESSMSGBOXES /CLOSEAPPLICATIONS"
            }
        }.Start();
        // 退出当前 Macro Deck 进程，让安装程序完成更新
        MacroDeck.Exit();
    }

    /// <summary>
    /// 定时轮询更新的后台工作方法。
    /// 每隔 30 分钟调用一次 CheckForUpdatesAsync，直到收到取消信号。
    /// </summary>
    /// <param name="cancellationToken">用于取消循环的令牌。</param>
    private async ValueTask DoWork(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await CheckForUpdatesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to automatically check for updates");
            }

            // 每次检查后等待 30 分钟
            Thread.Sleep(TimeSpan.FromMinutes(30));
        } while (!cancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// 校验下载的更新文件完整性。
    /// 通过计算文件的 SHA-256 哈希值，与服务端提供的哈希值进行加密学比较，
    /// 确保文件在传输过程中未被篡改或损坏。
    /// </summary>
    /// <param name="path">已下载文件的本地路径。</param>
    /// <param name="updateApiVersionFileInfo">服务端提供的文件信息，包含期望的哈希值。</param>
    /// <exception cref="InvalidOperationException">当路径为空或哈希不匹配时抛出。</exception>
    /// <exception cref="FileNotFoundException">当文件不存在时抛出。</exception>
    private static async ValueTask VerifyDownloadedFile(string? path,
        UpdateApiVersionFileInfo? updateApiVersionFileInfo)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("The path was empty");
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The update file was not found");
        }

        // 打开文件流并计算 SHA-256 哈希值
        await using var ms = File.OpenRead(path);
        var calculatedHash = await ms.CalculateSha256Hash();
        if (!calculatedHash.EqualsCryptographically(updateApiVersionFileInfo?.FileHash))
        {
            throw new InvalidOperationException(
                "The hash of the downloaded file does not match the file on the server");
        }
    }

    /// <summary>
    /// 从指定 URL 下载更新文件到临时目录。
    /// </summary>
    /// <param name="url">更新文件的下载地址。</param>
    /// <param name="version">目标版本号，用于生成文件名。</param>
    /// <param name="progress">用于报告下载进度的 IProgress 接口。</param>
    /// <returns>下载文件的本地路径。</returns>
    /// <exception cref="InvalidOperationException">当下载 URL 为空时抛出。</exception>
    private static async ValueTask<string?> DownloadUpdate(string? url,
        string? version,
        IProgress<DownloadProgressInfo> progress)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException("Download url was empty");
        }

        // 将文件保存到临时目录，文件名包含版本号以便区分
        var destinationPath = Path.Combine(ApplicationPaths.TempDirectoryPath, $"update-{version}.exe");
        await FileDownloader.DownloadFileAsync(url, destinationPath, progress);

        return destinationPath;
    }
}
