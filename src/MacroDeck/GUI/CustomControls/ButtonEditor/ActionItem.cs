using Serilog;
using SuchByte.MacroDeck.Interfaces;
using SuchByte.MacroDeck.Plugins;

namespace SuchByte.MacroDeck.GUI.CustomControls.ButtonEditor;


public partial class ActionItem : UserControl, IActionConditionItem
{
    private static readonly ILogger Logger = Log.ForContext(typeof(ActionItem));

    public PluginAction? Action { get; set; }

    public event EventHandler OnRemoveClick;
    public event EventHandler OnEditClick;
    public event EventHandler OnMoveUpClick;
    public event EventHandler OnMoveDownClick;


    public ActionItem(PluginAction? macroDeckAction)
    {
        Action = macroDeckAction;
        InitializeComponent();
        lblPlugin.Text = PluginManager.GetPluginByAction(Action).Name;
        lblAction.Text = Action.Name;
        lblConfigurationSummary.Text = Action.ConfigurationSummary;
    }

    private void ActionItem_Load(object sender, EventArgs e)
    {
        Logger.Information("ActionItem_Load 触发，准备应用配置字体。Family={Family}, Size={Size}, Bold={Bold}",
            Utils.FontManager.FontFamily, Utils.FontManager.FontSize, Utils.FontManager.FontBold);
        Utils.FontManager.Apply(this);
        Logger.Information("ActionItem 字体应用完成。lblPlugin=[{LblPlugin}], lblAction=[{LblAction}]",
            lblPlugin.Font, lblAction.Font);
    }


    private void BtnRemove_Click(object sender, EventArgs e)
    {
        OnRemoveClick?.Invoke(this, EventArgs.Empty);
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        OnEditClick?.Invoke(this, EventArgs.Empty);
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
        OnMoveUpClick?.Invoke(this, EventArgs.Empty);
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
        OnMoveDownClick?.Invoke(this, EventArgs.Empty);
    }
}
