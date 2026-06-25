using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Icons;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class ExportIconPackDialog : DialogForm
{
    public IconPack IconPack;

    private Size _originalClientSize;

    public ExportIconPackDialog(IconPack iconPack)
    {
        InitializeComponent();
        lblVersion.Text = LanguageManager.Strings.Version;
        btnOk.Text = LanguageManager.Strings.Ok;
        IconPack = iconPack;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustLabelHeight(lblVersion);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }

    private void ExportIconPackDialog_Load(object sender, EventArgs e)
    {
        version.Text = IconPack.Version;
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
        if (version.Text.Length < 2)
        {
            return;
        }

        IconPack.Version = version.Text;

        DialogResult = DialogResult.OK;
    }
}
