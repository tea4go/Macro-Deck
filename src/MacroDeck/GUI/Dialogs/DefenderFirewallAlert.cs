using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class DefenderFirewallAlert : DialogForm
{
    private Size _originalClientSize;

    public DefenderFirewallAlert()
    {
        InitializeComponent();
        SetCloseIconVisible(false);

        lblImportant.Text = LanguageManager.Strings.Important;
        lblInfo.Text = LanguageManager.Strings.FirewallAlertInfo;
        btnGotIt.Text = LanguageManager.Strings.GotIt;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustLabelHeight(lblImportant);
        LayoutHelper.AdjustLabelHeight(lblInfo);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }

    private void BtnGotIt_Click(object sender, EventArgs e)
    {
        Close();
    }
}
