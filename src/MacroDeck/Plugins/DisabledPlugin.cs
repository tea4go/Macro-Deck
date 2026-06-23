namespace SuchByte.MacroDeck.Plugins;

/// <summary>
/// 已禁用的插件类，用于表示被用户禁用的插件。
/// 仅保留元数据（名称、版本、作者），Enable 方法为空实现。
/// </summary>
public class DisabledPlugin : MacroDeckPlugin
{
    /// <summary>插件名称</summary>
    internal override string Name { get; set; }

    /// <summary>插件版本</summary>
    internal override string Version { get; set; }

    /// <summary>插件作者</summary>
    internal override string Author { get; set; }

    /// <summary>
    /// 启用插件（空实现，禁用的插件不执行任何操作）
    /// </summary>
    public override void Enable()
    {
    }
}
