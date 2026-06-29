using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Utils;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.CustomControls;

public partial class ActionConfiguratorActionItem : RoundedUserControl
{
    public PluginAction PluginAction { get; set; }

    public MacroDeckPlugin Plugin { get; set; }


    public ActionConfiguratorActionItem(MacroDeckPlugin plugin, PluginAction pluginAction)
    {
        Plugin ??= plugin;
        PluginAction ??= pluginAction;
        InitializeComponent();
        DoubleBuffered = true;
        lblActionName.MouseClick += Control_MouseClick;
    }

    private void Control_MouseClick(object sender, MouseEventArgs e)
    {
        OnMouseClick(e);
    }

    private void ActionConfiguratorActionItem_Load(object sender, EventArgs e)
    {
        lblActionName.Text = PluginAction.Name;
    }

    /// <summary>
    /// 根据当前字体自适应调整内部标签高度和控件高度。
    /// </summary>
    public void AdjustLayout()
    {
        // 插件名称字体：配置字号，始终粗体
        lblActionName.Font = new Font(FontManager.FontFamily, FontManager.FontSize, FontStyle.Bold, GraphicsUnit.Point);

        var textHeight = TextRenderer.MeasureText("Ay", lblActionName.Font).Height + 8;
        if (lblActionName.Height < textHeight)
            lblActionName.Height = textHeight;

        var neededHeight = lblActionName.Bottom + 5;
        if (Height < neededHeight)
            Height = neededHeight;
    }
}
