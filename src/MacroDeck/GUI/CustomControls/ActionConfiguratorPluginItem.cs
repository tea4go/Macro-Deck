using System.Windows.Forms;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.CustomControls;

/// <summary>
/// 动作配置器中用于显示插件条目的自定义控件。
/// 包含插件图标、插件名称、动作数量标签以及展开/折叠箭头。
/// </summary>
public partial class ActionConfiguratorPluginItem : RoundedUserControl
{
    /// <summary>
    /// 内部选中状态，用于控制展开/折叠箭头的显示。
    /// </summary>
    private bool selected;

    /// <summary>
    /// 与此条目关联的 MacroDeck 插件实例。
    /// </summary>
    public MacroDeckPlugin Plugin { get; set; }

    /// <summary>
    /// 获取或设置当前插件的展开/折叠状态。
    /// 选中时显示向下箭头，未选中时显示向右箭头。
    /// </summary>
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            // 根据展开状态切换箭头图标
            chevron.BackgroundImage = selected ? Resources.Chevron_Down : Resources.Chevron_Right;
        }
    }

    /// <summary>
    /// 初始化 ActionConfiguratorPluginItem 控件的新实例。
    /// </summary>
    /// <param name="macroDeckPlugin">与此条目关联的插件。</param>
    public ActionConfiguratorPluginItem(MacroDeckPlugin macroDeckPlugin)
    {
        Plugin ??= macroDeckPlugin;
        InitializeComponent();
        DoubleBuffered = true;

        // 将子控件的点击事件传递到父控件，实现整行点击效果
        pluginIcon.MouseClick += Control_MouseClick;
        pluginName.MouseClick += Control_MouseClick;
        lblCountActions.MouseClick += Control_MouseClick;
        chevron.MouseClick += Control_MouseClick;
    }

    /// <summary>
    /// 处理子控件的鼠标点击事件，将其冒泡传递到父控件。
    /// </summary>
    private void Control_MouseClick(object sender, MouseEventArgs e)
    {
        OnMouseClick(e);
    }

    /// <summary>
    /// 控件加载时初始化插件图标、名称和动作数量显示。
    /// </summary>
    private void ActionConfiguratorPluginItem_Load(object sender, EventArgs e)
    {
        if (Plugin == null)
        {
            return;
        }

        // 显示插件图标，若无自定义图标则使用默认图标
        pluginIcon.BackgroundImage = Plugin.PluginIcon ?? Resources.Icon;
        pluginName.Text = Plugin.Name;
        // 根据动作数量显示单数或复数形式的文本
        lblCountActions.Text
            = string.Format(Plugin.Actions.Count == 1
                    ? LanguageManager.Strings.XAction
                    : LanguageManager.Strings.XActions,
                Plugin.Actions.Count);
    }

    /// <summary>
    /// 根据当前字体自适应调整内部标签的高度与位置，确保双行文字不重叠。
    /// 插件名称字体：配置字号 + 3，始终粗体。
    /// </summary>
    public void AdjustLayout()
    {
        // 插件名称字体：配置字号 + 3，始终粗体
        var baseFont = pluginName.Font;
        var nameFontSize = FontManager.FontSize + 3;
        pluginName.Font = new Font(FontManager.FontFamily, nameFontSize, FontStyle.Bold, baseFont.Unit, baseFont.GdiCharSet);
        lblCountActions.Font = new Font(FontManager.FontFamily, FontManager.FontSize, FontStyle.Bold, baseFont.Unit, baseFont.GdiCharSet);
        // 计算各标签在当前字体下的最小高度（以 Ay 为基准加上边距）
        var nameTextHeight = TextRenderer.MeasureText("Ay", pluginName.Font).Height + 4;
        var countTextHeight = TextRenderer.MeasureText("Ay", lblCountActions.Font).Height + 4;

        // 确保标签高度足够容纳当前字体
        if (pluginName.Height < nameTextHeight) pluginName.Height = nameTextHeight;
        if (lblCountActions.Height < countTextHeight) lblCountActions.Height = countTextHeight;

        // 将计数标签紧贴在名称标签下方，避免重叠
        lblCountActions.Top = pluginName.Bottom;

        // 控件高度取图标底部或计数标签底部的较大值 + 底部边距
        var contentBottom = Math.Max(pluginIcon.Bottom, lblCountActions.Bottom);
        var neededHeight = contentBottom + 5;
        if (Height < neededHeight) Height = neededHeight;
    }
}
