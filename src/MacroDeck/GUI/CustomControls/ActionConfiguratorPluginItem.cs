using System.Windows.Forms;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.CustomControls;

public partial class ActionConfiguratorPluginItem : RoundedUserControl
{
    private bool selected;

    public MacroDeckPlugin Plugin { get; set; }

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            chevron.BackgroundImage = selected ? Resources.Chevron_Down : Resources.Chevron_Right;
        }
    }

    public ActionConfiguratorPluginItem(MacroDeckPlugin macroDeckPlugin)
    {
        Plugin ??= macroDeckPlugin;
        InitializeComponent();
        DoubleBuffered = true;

        pluginIcon.MouseClick += Control_MouseClick;
        pluginName.MouseClick += Control_MouseClick;
        lblCountActions.MouseClick += Control_MouseClick;
        chevron.MouseClick += Control_MouseClick;
    }


    private void Control_MouseClick(object sender, MouseEventArgs e)
    {
        OnMouseClick(e);
    }

    private void ActionConfiguratorPluginItem_Load(object sender, EventArgs e)
    {
        if (Plugin == null)
        {
            return;
        }

        pluginIcon.BackgroundImage = Plugin.PluginIcon ?? Resources.Icon;
        pluginName.Text = Plugin.Name;
        lblCountActions.Text
            = string.Format(Plugin.Actions.Count == 1
                    ? LanguageManager.Strings.XAction
                    : LanguageManager.Strings.XActions,
                Plugin.Actions.Count);
    }

    /// <summary>
    /// 根据当前字体自适应调整内部标签的高度与位置，确保双行文字不重叠。
    /// </summary>
    public void AdjustLayout()
    {
        // 插件名称字体：配置字号 + 3，始终粗体
        var baseFont = pluginName.Font;
        var nameFontSize = FontManager.FontSize + 3;
        pluginName.Font = new Font(
            FontManager.FontFamily,
            nameFontSize,
            FontStyle.Bold,
            baseFont.Unit,
            baseFont.GdiCharSet);

        // 计算各标签在当前字体下的最小高度
        var nameTextHeight = TextRenderer.MeasureText("Ay", pluginName.Font).Height + 4;
        var countTextHeight = TextRenderer.MeasureText("Ay", lblCountActions.Font).Height + 4;

        if (pluginName.Height < nameTextHeight)
            pluginName.Height = nameTextHeight;
        if (lblCountActions.Height < countTextHeight)
            lblCountActions.Height = countTextHeight;

        // 将计数标签紧贴在名称标签下方，避免重叠
        lblCountActions.Top = pluginName.Bottom;

        // 控件高度取图标底部或计数标签底部的较大值 + 底部边距
        var contentBottom = Math.Max(pluginIcon.Bottom, lblCountActions.Bottom);
        var neededHeight = contentBottom + 5;
        if (Height < neededHeight)
            Height = neededHeight;
    }
}
