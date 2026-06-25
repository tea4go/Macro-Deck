using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;
using Form = System.Windows.Forms.Form;

namespace SuchByte.MacroDeck.GUI.Dialogs;

internal partial class WaitDialog : DialogForm
{
    private Size _originalClientSize;

    public WaitDialog()
    {
        InitializeComponent();
        SetCloseIconVisible(false);
        lblPleaseWait.Text = LanguageManager.Strings.PleaseWait;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustLabelHeight(lblPleaseWait);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }
}

public static class SpinnerDialog
{
    private static WaitDialog waitDialog = new();

    public static void SetVisisble(bool visible, Form owner)
    {
        owner?.Invoke(() =>
        {
            if (visible)
            {
                if (waitDialog.Visible)
                {
                    return;
                }

                waitDialog.ShowDialog();
            }
            else
            {
                waitDialog.Hide();
            }
        });
    }
}
