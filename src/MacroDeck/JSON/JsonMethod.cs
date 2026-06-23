namespace SuchByte.MacroDeck.JSON;

/// <summary>
/// JSON 通信方法枚举，定义客户端与服务器之间的 WebSocket 通信协议方法。
/// 每个值代表一种消息类型，用于标识消息的用途和处理方式。
/// </summary>
public enum JsonMethod
{
    /// <summary>客户端已连接</summary>
    CONNECTED,
    /// <summary>按钮短按</summary>
    BUTTON_PRESS,
    /// <summary>按钮短按释放</summary>
    BUTTON_RELEASE,
    /// <summary>按钮长按</summary>
    BUTTON_LONG_PRESS,
    /// <summary>按钮长按释放</summary>
    BUTTON_LONG_PRESS_RELEASE,
    /// <summary>获取按钮布局</summary>
    GET_BUTTONS,
    /// <summary>获取图标</summary>
    GET_ICONS,
    /// <summary>更新按钮状态</summary>
    UPDATE_BUTTON,
    /// <summary>更新标签</summary>
    UPDATE_LABEL,
    /// <summary>图标 Base64 数据</summary>
    ICON_BASE64,
    /// <summary>获取配置</summary>
    GET_CONFIG,
    /// <summary>按钮操作完成</summary>
    BUTTON_DONE,
    /// <summary>获取已安装的插件列表</summary>
    GET_INSTALLED_PLUGINS,
    /// <summary>获取已安装的图标包列表</summary>
    GET_INSTALLED_ICON_PACKS
}
