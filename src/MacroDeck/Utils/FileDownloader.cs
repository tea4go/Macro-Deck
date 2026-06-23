using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using SuchByte.MacroDeck.DataTypes.FileDownloader;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 文件下载器，提供文件下载、图片下载和 JSON API 请求功能。
/// 使用共享的 HttpClient 实例避免重复的 DNS/TLS 握手开销和端口耗尽问题。
/// 支持下载进度报告和取消操作。
/// </summary>
public class FileDownloader
{
    /// <summary>
    /// 共享的 HttpClient 实例，所有下载复用同一连接池
    /// </summary>
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// 异步下载文件到指定路径，支持进度报告和取消操作。
    /// 使用流式下载避免将整个文件加载到内存中。
    /// </summary>
    /// <param name="url">下载 URL</param>
    /// <param name="destinationFileName">目标文件路径</param>
    /// <param name="progress">下载进度报告器（可选）</param>
    /// <param name="cancellationToken">取消令牌（可选）</param>
    public static async Task DownloadFileAsync(string url,
        string destinationFileName,
        IProgress<DownloadProgressInfo>? progress = null,
        CancellationToken? cancellationToken = null)
    {
        if (File.Exists(destinationFileName))
        {
            File.Delete(destinationFileName);
        }

        using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using Stream contentStream = await response.Content.ReadAsStreamAsync(),
            fileStream = new FileStream(destinationFileName,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                8192,
                true);
        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var bytesDownloaded = 0L;
        var buffer = new byte[8192];
        int bytesRead;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while ((bytesRead = await contentStream.ReadAsync(buffer)) != 0 &&
            cancellationToken?.IsCancellationRequested != true)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            bytesDownloaded += bytesRead;

            if (progress == null || totalBytes == -1)
            {
                continue;
            }

            // 计算下载速度和进度百分比
            var downloadSpeed = bytesDownloaded / stopwatch.Elapsed.TotalSeconds;
            progress.Report(new DownloadProgressInfo
            {
                TotalBytes = totalBytes,
                DownloadedBytes = bytesDownloaded,
                DownloadSpeed = downloadSpeed,
                Percentage = (int)Math.Round((double)bytesDownloaded / totalBytes * 100)
            });
        }

        stopwatch.Stop();
    }

    /// <summary>
    /// 异步下载图片到内存流
    /// </summary>
    /// <param name="url">图片 URL</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>包含图片数据的内存流</returns>
    public static async Task<MemoryStream> DownloadImageAsync(string url, CancellationToken cancellationToken)
    {
        using var response = await HttpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var imageStream = new MemoryStream();
        await response.Content.CopyToAsync(imageStream, cancellationToken);
        imageStream.Position = 0;

        return imageStream;
    }

    /// <summary>
    /// 异步从指定 URL 获取 JSON 数据并反序列化为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="url">API URL</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>反序列化后的对象，失败返回 null</returns>
    public static async Task<T?> GetFromJsonAsync<T>(string url, CancellationToken cancellationToken)
    {
        return await HttpClient.GetFromJsonAsync<T>(url, cancellationToken);
    }
}
