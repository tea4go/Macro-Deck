using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class IconImportQuality : DialogForm
{
    public int Pixels;

    private Size _originalClientSize;

    public IconImportQuality(bool gif = false)
    {
        InitializeComponent();
        SuspendLayout();
        lblInfo.Text = LanguageManager.Strings.IconImportQualityInfo;
        label1.Text = LanguageManager.Strings.Quality;
        qualityOriginal.Text = LanguageManager.Strings.Original;
        qualityHigh.Text = LanguageManager.Strings.High350px;
        qualityNormal.Text = LanguageManager.Strings.Normal200px;
        qualityLow.Text = LanguageManager.Strings.Low150px;
        qualityLowest.Text = LanguageManager.Strings.Lowest100px;
        btnOk.Text = LanguageManager.Strings.Ok;
        if (gif)
        {
            qualityLowest.Checked = true;
        }

        ResumeLayout();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustLabelHeight(lblInfo);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
        if (qualityOriginal.Checked)
        {
            Pixels = -1;
        }
        else if (qualityHigh.Checked)
        {
            Pixels = 350;
        }
        else if (qualityNormal.Checked)
        {
            Pixels = 250;
        }
        else if (qualityLow.Checked)
        {
            Pixels = 150;
        }
        else if (qualityLowest.Checked)
        {
            Pixels = 100;
        }

        DialogResult = DialogResult.OK;
    }
}
