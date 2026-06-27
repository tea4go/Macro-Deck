using Newtonsoft.Json.Linq;
using SuchByte.MacroDeck.DataTypes;
using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Enums;
using SuchByte.MacroDeck.Extension;
using SuchByte.MacroDeck.Folders;
using Serilog;
using SuchByte.MacroDeck.JSON;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Services;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Server;

/// <summary>
/// Macro Deck 服务器核心类，管理 WebSocket 连接、客户端消息处理和按钮状态同步。
/// 负责客户端的连接认证、消息路由、按钮事件分发和状态更新。
/// </summary>
public static class MacroDeckServer
{
    private static readonly ILogger Logger = Log.ForContext(typeof(MacroDeckServer));

    /// <summary>设备连接状态变化事件</summary>
    public static event EventHandler? OnDeviceConnectionStateChanged;

    /// <summary>文件夹变化事件</summary>
    public static event EventHandler? OnFolderChanged;

    /// <summary>已连接的客户端列表</summary>
    public static List<MacroDeckClient> Clients { get; } = new();

    /// <summary>快速设置令牌，用于新设备首次连接时自动注册</summary>
    public static string QuickSetupToken { get; } = RandomStringGenerator.RandomString(8);

    /// <summary>
    /// 启动 Macro Deck 服务器。加载已知设备并启动 WebSocket 服务。
    /// </summary>
    /// <param name="port">监听端口</param>
    public static void Start(int port)
    {
        DeviceManager.LoadKnownDevices();
        Task.Run(async () => await StartWebSocketServer(port));
    }

    /// <summary>
    /// 启动 WebSocket 服务器。配置 SSL 证书（如启用）并开始监听连接。
    /// </summary>
    private static async Task StartWebSocketServer(int port)
    {
        Clients.Clear();
        WebSocketHandler.SessionDisconnected += WebSocketHandlerOnSessionDisconnected;
        WebSocketHandler.SessionConnected += WebSocketHandlerOnSessionConnected;
        WebSocketHandler.MessageReceived += WebSocketHandlerOnMessageReceived;
        SslCertificateService.EnsureValidCertificate();
        var certificate = MacroDeck.Configuration.EnableSsl ? SslCertificateService.GetX509Certificate() : null;
        try
        {
            await MacroDeckServerHelper.Setup(port, certificate);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "启动服务器失败");

            using var msgBox = new GUI.CustomControls.MessageBox();
            msgBox.ShowDialog(LanguageManager.Strings.Error,
                LanguageManager.Strings.FailedToStartServer + Environment.NewLine + ex.Message,
                MessageBoxButtons.OK);
        }
    }

    /// <summary>
    /// WebSocket 消息接收处理，将消息路由到 OnMessage 方法
    /// </summary>
    private static void WebSocketHandlerOnMessageReceived(object? sender, string message)
    {
        if (sender is not WebSocketSession session)
        {
            return;
        }

        var sessionId = session.Id;
        var macroDeckClient = Clients.FirstOrDefault(x => x.SessionId == sessionId);
        if (macroDeckClient is null)
        {
            return;
        }

        OnMessage(macroDeckClient, message);
    }

    /// <summary>
    /// WebSocket 会话连接处理。创建客户端对象，检查连接限制后添加到客户端列表。
    /// 如果阻止新连接、客户端数已达上限（10）或无可用配置，则关闭连接。
    /// </summary>
    private static void WebSocketHandlerOnSessionConnected(object? sender, EventArgs e)
    {
        if (sender is not WebSocketSession session)
        {
            return;
        }

        var macroDeckClient = new MacroDeckClient(session.Id);

        // 检查连接限制：阻止新连接、客户端数上限、无可用配置
        if (MacroDeck.Configuration.BlockNewConnections ||
            Clients.Count >= 10 ||
            ProfileManager.CurrentProfile?.Folders.Count < 1)
        {
            CloseClient(macroDeckClient);
            return;
        }

        Clients.Add(macroDeckClient);
    }

    /// <summary>
    /// WebSocket 会话断开处理。从客户端列表中移除并触发连接状态变化事件。
    /// </summary>
    private static void WebSocketHandlerOnSessionDisconnected(object? sender, EventArgs e)
    {
        if (sender is not WebSocketSession session)
        {
            return;
        }

        var sessionId = session.Id;
        var macroDeckClient = Clients.FirstOrDefault(x => x.SessionId == sessionId);
        if (macroDeckClient is null)
        {
            return;
        }

        Clients.Remove(macroDeckClient);
        Logger.Information("{ClientId} 的连接已关闭", macroDeckClient.ClientId);
        OnDeviceConnectionStateChanged?.Invoke(macroDeckClient, EventArgs.Empty);
    }

    /// <summary>
    /// 关闭指定客户端的连接
    /// </summary>
    /// <param name="macroDeckClient">要关闭的客户端</param>
    public static void CloseClient(MacroDeckClient macroDeckClient)
    {
        Logger.Information("关闭与 {ClientId} 的连接", macroDeckClient.ClientId);
        Task.Run(async () => await WebSocketHandler.Close(macroDeckClient.SessionId));
    }

    /// <summary>
    /// 消息处理核心方法。解析 JSON 消息中的方法类型并分发处理。
    /// 支持的方法：CONNECTED（连接认证）、BUTTON_PRESS/RELEASE/LONG_PRESS/LONG_PRESS_RELEASE（按钮事件）、GET_BUTTONS（获取按钮）。
    /// </summary>
    /// <param name="macroDeckClient">发送消息的客户端</param>
    /// <param name="jsonMessageString">JSON 格式的消息字符串</param>
    private static void OnMessage(MacroDeckClient macroDeckClient, string jsonMessageString)
    {
        var responseObject = JObject.Parse(jsonMessageString);

        if (responseObject["Method"] == null)
        {
            return;
        }

        if (!Enum.TryParse(typeof(JsonMethod), responseObject["Method"].ToString(), out var method))
        {
            return;
        }

        Logger.Debug("收到方法调用：{Method}", method);

        switch (method)
        {
            case JsonMethod.CONNECTED:
                // 验证客户端 API 版本和必要字段
                if (responseObject["API"] == null ||
                    responseObject["Client-Id"] == null ||
                    responseObject["Device-Type"] == null ||
                    responseObject["API"].ToObject<int>() < MacroDeck.ApiVersion)
                {
                    CloseClient(macroDeckClient);
                    return;
                }

                macroDeckClient.SetClientId(responseObject["Client-Id"].ToString());

                Logger.Information("收到来自 {ClientId} 的连接请求", macroDeckClient.ClientId);

                Enum.TryParse(responseObject["Device-Type"].ToString(), out DeviceType deviceType);
                macroDeckClient.DeviceType = deviceType;

                // 快速设置令牌验证：匹配则自动注册设备
                if (responseObject["Token"]?.ToString() is { } token && token.EqualsCryptographically(QuickSetupToken))
                {
                    DeviceManager.AddKnownDevice(macroDeckClient);
                }
                else
                {
                    // 常规连接请求验证
                    if (!DeviceManager.RequestConnection(macroDeckClient))
                    {
                        CloseClient(macroDeckClient);
                        return;
                    }
                }

                if (DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId) == null)
                {
                    return;
                }

                // 确保设备有分配的配置文件
                if (string.IsNullOrWhiteSpace(DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).ProfileId))
                {
                    DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).ProfileId
                        = ProfileManager.Profiles.FirstOrDefault().ProfileId;
                }

                DeviceManager.SaveKnownDevices();

                // 设置客户端的配置文件和文件夹
                macroDeckClient.Profile
                    = ProfileManager.FindProfileById(DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId)
                        .ProfileId);

                if (macroDeckClient.Profile == null)
                {
                    macroDeckClient.Profile = ProfileManager.Profiles.FirstOrDefault();
                }

                macroDeckClient.Folder = macroDeckClient.Profile.Folders.FirstOrDefault();

                // 发送连接成功消息
                macroDeckClient.DeviceMessage.Connected(macroDeckClient);

                OnDeviceConnectionStateChanged?.Invoke(macroDeckClient, EventArgs.Empty);
                Logger.Information("{ClientId} 已连接", macroDeckClient.ClientId);
                break;

            case JsonMethod.BUTTON_PRESS:
            case JsonMethod.BUTTON_RELEASE:
            case JsonMethod.BUTTON_LONG_PRESS:
            case JsonMethod.BUTTON_LONG_PRESS_RELEASE:
                // 将方法类型映射为按钮按压类型
                var buttonPressType = method switch
                {
                    JsonMethod.BUTTON_PRESS => ButtonPressType.SHORT,
                    JsonMethod.BUTTON_RELEASE => ButtonPressType.SHORT_RELEASE,
                    JsonMethod.BUTTON_LONG_PRESS => ButtonPressType.LONG,
                    JsonMethod.BUTTON_LONG_PRESS_RELEASE => ButtonPressType.LONG_RELEASE,
                    _ => ButtonPressType.SHORT
                };

                try
                {
                    if (macroDeckClient == null ||
                        macroDeckClient.Folder == null ||
                        macroDeckClient.Folder.ActionButtons == null)
                    {
                        return;
                    }

                    // 解析按钮位置（格式：row_column）
                    var row = int.Parse(responseObject["Message"].ToString().Split('_')[0]);
                    var column = int.Parse(responseObject["Message"].ToString().Split('_')[1]);

                    var actionButton
                        = macroDeckClient.Folder.ActionButtons.Find(aB =>
                            aB.Position_X == column && aB.Position_Y == row);
                    if (actionButton != null)
                    {
                        Execute(actionButton, macroDeckClient.ClientId, buttonPressType);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex, "处理按钮长按释放事件时发生异常");
                }

                break;

            case JsonMethod.GET_BUTTONS:
                // 客户端请求获取所有按钮数据
                Task.Run(() => { SendAllButtons(macroDeckClient); });
                break;
        }
    }

    /// <summary>
    /// 执行动作按钮的触发操作。根据按钮按压类型选择对应的动作列表并异步执行。
    /// </summary>
    /// <param name="actionButton">目标动作按钮</param>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="buttonPressType">按钮按压类型</param>
    internal static void Execute(ActionButton.ActionButton actionButton,
        string clientId,
        ButtonPressType buttonPressType)
    {
        // 根据按压类型选择对应的动作列表
        var actions = buttonPressType switch
        {
            ButtonPressType.SHORT => actionButton.Actions,
            ButtonPressType.SHORT_RELEASE => actionButton.ActionsRelease,
            ButtonPressType.LONG => actionButton.ActionsLongPress,
            ButtonPressType.LONG_RELEASE => actionButton.ActionsLongPressRelease,
            _ => actionButton.Actions
        };

        Task.Run(() =>
        {
            foreach (var action in actions)
            {
                if (action is null)
                {
                    continue;
                }

                try
                {
                    action.Trigger(clientId, actionButton);
                }
                catch
                {
                }
            }
        });
    }

    /// <summary>
    /// 设置客户端的配置文件，并发送配置和按钮数据
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    /// <param name="macroDeckProfile">要设置的配置文件</param>
    public static void SetProfile(MacroDeckClient macroDeckClient, MacroDeckProfile macroDeckProfile)
    {
        macroDeckClient.Profile = macroDeckProfile;
        macroDeckClient.DeviceMessage.SendConfiguration(macroDeckClient);

        SetFolder(macroDeckClient, macroDeckProfile.Folders[0]);
    }

    /// <summary>
    /// 设置客户端的当前文件夹，并发送该文件夹的所有按钮数据
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    /// <param name="folder">要设置的文件夹</param>
    public static void SetFolder(MacroDeckClient macroDeckClient, MacroDeckFolder folder)
    {
        macroDeckClient.Folder = folder;
        SendAllButtons(macroDeckClient);
        OnFolderChanged?.Invoke(macroDeckClient, EventArgs.Empty);
    }

    /// <summary>
    /// 更新所有正在显示指定文件夹的客户端的按钮数据
    /// </summary>
    /// <param name="folder">发生变化的文件夹</param>
    public static void UpdateFolder(MacroDeckFolder folder)
    {
        foreach (var macroDeckClient in Clients.FindAll(macroDeckClient => macroDeckClient.Folder.Equals(folder)))
        {
            SendAllButtons(macroDeckClient);
        }
    }

    /// <summary>
    /// 向客户端发送当前文件夹的所有按钮数据
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    private static void SendAllButtons(MacroDeckClient macroDeckClient)
    {
        macroDeckClient?.DeviceMessage?.SendAllButtons(macroDeckClient);
    }

    /// <summary>
    /// 向客户端发送单个按钮的更新数据
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    /// <param name="actionButton">要更新的按钮</param>
    public static void SendButton(MacroDeckClient macroDeckClient, ActionButton.ActionButton actionButton)
    {
        macroDeckClient?.DeviceMessage?.UpdateButton(macroDeckClient, actionButton);
    }

    /// <summary>
    /// 设置动作按钮的开/关状态
    /// </summary>
    /// <param name="actionButton">目标按钮</param>
    /// <param name="state">true 为开，false 为关</param>
    public static void SetState(ActionButton.ActionButton actionButton, bool state)
    {
        actionButton.State = state;
    }

    /// <summary>
    /// 更新按钮状态到所有正在显示该按钮的客户端
    /// </summary>
    /// <param name="actionButton">状态变化的按钮</param>
    internal static void UpdateState(ActionButton.ActionButton actionButton)
    {
        foreach (var macroDeckClient in Clients.FindAll(macroDeckClient =>
            macroDeckClient.Folder.ActionButtons.Contains(actionButton)))
        {
            SendButton(macroDeckClient, actionButton);
        }
    }

    /// <summary>
    /// 根据客户端 ID 获取 MacroDeckClient 实例
    /// </summary>
    /// <param name="macroDeckClientId">客户端 ID</param>
    /// <returns>客户端实例，未找到返回 null</returns>
    public static MacroDeckClient? GetMacroDeckClient(string macroDeckClientId)
    {
        return string.IsNullOrWhiteSpace(macroDeckClientId)
            ? null
            : Clients.Find(macroDeckClient => macroDeckClient.ClientId == macroDeckClientId);
    }

    /// <summary>
    /// 原始发送方法，向指定客户端发送 JSON 对象
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    /// <param name="jObject">要发送的 JSON 对象</param>
    internal static void Send(MacroDeckClient macroDeckClient, JObject jObject)
    {
        Task.Run(async () => await WebSocketHandler.SendMessageToClient(macroDeckClient.SessionId, jObject.ToString()));
    }
}
