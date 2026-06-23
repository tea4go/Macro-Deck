using SuchByte.MacroDeck.Plugins;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.ActionButton.Plugin;
// Don't change because of backwards compatibility!

/// <summary>
/// 延迟动作，在动作序列中插入指定毫秒数的延迟。
/// 配置值为延迟的毫秒数（字符串形式）。
/// </summary>
public class DelayAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => "Delay";

    /// <summary>动作描述</summary>
    public override string Description => "";

    /// <summary>
    /// 触发延迟动作，使当前线程休眠配置中指定的毫秒数。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        try
        {
            // 解析配置中的毫秒数并休眠当前线程
            Thread.Sleep(int.Parse(Configuration));
        }
        catch
        {
        }
    }
}
