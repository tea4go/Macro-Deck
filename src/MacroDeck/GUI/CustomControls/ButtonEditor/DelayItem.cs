using Serilog;
using SuchByte.MacroDeck.ActionButton.Plugin;
using SuchByte.MacroDeck.Interfaces;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;

namespace SuchByte.MacroDeck.GUI.CustomControls.ButtonEditor;


public partial class DelayItem : UserControl, IActionConditionItem
{
    private static readonly ILogger Logger = Log.ForContext(typeof(DelayItem));

    public PluginAction? Action { get; set; }

    public event EventHandler OnRemoveClick;
#pragma warning disable CS0067 // 接口要求的事件，DelayItem 当前不使用编辑功能
    public event EventHandler OnEditClick;
#pragma warning restore CS0067
    public event EventHandler OnMoveUpClick;
    public event EventHandler OnMoveDownClick;

    public DelayItem(PluginAction? macroDeckAction = null)
    {
        Action = macroDeckAction;
        InitializeComponent();
        lblWait.Text = LanguageManager.Strings.Wait;
        if (Action != null)
        {
            millis.ValueChanged -= DelayValueChanged;
            seconds.ValueChanged -= DelayValueChanged;
            minutes.ValueChanged -= DelayValueChanged;
            try
            {
                var t = TimeSpan.FromMilliseconds(double.Parse(Action.Configuration));
                millis.Value = t.Milliseconds;
                seconds.Value = t.Seconds;
                minutes.Value = t.Minutes;
            }
            catch
            {
            }

            millis.ValueChanged += DelayValueChanged;
            seconds.ValueChanged += DelayValueChanged;
            minutes.ValueChanged += DelayValueChanged;
        }
        else
        {
            Action = new DelayAction
            {
                Configuration = 1000.ToString()
            };
        }
    }

    private void DelayItem_Load(object sender, EventArgs e)
    {
        Logger.Information("DelayItem_Load 触发，准备应用配置字体。Family={Family}, Size={Size}, Bold={Bold}",
            Utils.FontManager.FontFamily, Utils.FontManager.FontSize, Utils.FontManager.FontBold);
        Utils.FontManager.Apply(this);
        Logger.Information("DelayItem 字体应用完成。lblWait=[{LblWait}], minutes=[{Minutes}]",
            lblWait.Font, minutes.Font);
    }

    private void DelayValueChanged(object sender, EventArgs e)
    {
        Action.Configuration = (millis.Value + seconds.Value * 1000 + minutes.Value * 1000 * 60).ToString();
    }

    private void BtnRemove_Click(object sender, EventArgs e)
    {
        OnRemoveClick?.Invoke(this, EventArgs.Empty);
    }


    private void BtnUp_Click(object sender, EventArgs e)
    {
        OnMoveUpClick?.Invoke(this, EventArgs.Empty);
    }

    private void BtnDown_Click(object sender, EventArgs e)
    {
        OnMoveDownClick?.Invoke(this, EventArgs.Empty);
    }
}
