using SuchByte.MacroDeck.Language;

namespace SuchByte.MacroDeck.GUI.InitialSetupPages;

public partial class SetupPage2 : UserControl
{
    private InitialSetup initialSetup;

    public SetupPage2(InitialSetup initialSetup)
    {
        InitializeComponent();
        this.initialSetup = initialSetup;
        lblConfigureNetwork.Text = LanguageManager.Strings.InitialSetupConfigureNetworkSettings;
        lblPort.Text = LanguageManager.Strings.Port;
        groupInfo.Text = LanguageManager.Strings.Info;
        lblInfo.Text = LanguageManager.Strings.ConfigureNetworkInfo;
    }

    private void port_ValueChanged(object sender, EventArgs e)
    {
        initialSetup.configuration.HostPort = (int)port.Value;
    }
}
