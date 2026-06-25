using Newtonsoft.Json;
using Serilog;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class JsonButtonEditor : DialogForm
{
    private static readonly ILogger Logger = Log.ForContext(typeof(JsonButtonEditor));

    private Size _originalClientSize;

    private static JsonSerializerSettings jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        Error = (sender, args) =>
        {
            Logger.Warning(args.ErrorContext.Error.Message);
            args.ErrorContext.Handled = true;
        }
    };


    public ActionButton.ActionButton ActionButton { get; set; }


    public JsonButtonEditor(ActionButton.ActionButton actionButton)
    {
        InitializeComponent();

        ActionButton = actionButton;
        if (ActionButton.LabelOff != null)
        {
            ActionButton.LabelOff.LabelBase64 = string.Empty;
        }

        if (ActionButton.LabelOn != null)
        {
            ActionButton.LabelOn.LabelBase64 = string.Empty;
        }

        jsonTextBox.Text = JsonConvert.SerializeObject(ActionButton, jsonSerializerSettings);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }

    private void BtnApply_Click(object sender, EventArgs e)
    {
        ActionButton
            = JsonConvert.DeserializeObject<ActionButton.ActionButton>(jsonTextBox.Text, jsonSerializerSettings);
        DialogResult = DialogResult.OK;
        Close();
    }
}
