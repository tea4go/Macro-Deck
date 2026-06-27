using SuchByte.MacroDeck.Device;
using Serilog;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class DeviceConfigurator : DialogForm
{
    private static readonly ILogger Logger = Log.ForContext(typeof(DeviceConfigurator));

    private MacroDeckDevice _macroDeckDevice;

    private Size _originalClientSize;

    public DeviceConfigurator(MacroDeckDevice macroDeckDevice)
    {
        _macroDeckDevice = macroDeckDevice;
        InitializeComponent();
        lblBrightness.Text = LanguageManager.Strings.Brightness;
        checkAutoConnect.Text = LanguageManager.Strings.AutoConnect;
        btnOk.Text = LanguageManager.Strings.Ok;
        lblKeepWake.Text = LanguageManager.Strings.KeepAwake;
        radioKeepAwakeNever.Text = LanguageManager.Strings.Never;
        radioKeepAwakeConnected.Text = LanguageManager.Strings.WhenConnected;
        radioKeepAwakeAlways.Text = LanguageManager.Strings.Always;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustAllLabelHeights(this);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }

    private void DeviceConfigurator_Load(object sender, EventArgs e)
    {
        brightness.Scroll -= Brightness_Scroll;
        brightness.Value = (int)(_macroDeckDevice.Configuration.Brightness * 10.0);
        brightness.Scroll += Brightness_Scroll;
        checkAutoConnect.CheckedChanged -= CheckAutoConnect_CheckedChanged;
        checkAutoConnect.Checked = _macroDeckDevice.Configuration.AutoConnect;
        checkAutoConnect.CheckedChanged += CheckAutoConnect_CheckedChanged;
        radioKeepAwakeNever.CheckedChanged -= RadioKeepAwakeNever_CheckedChanged;
        radioKeepAwakeConnected.CheckedChanged -= RadioKeepAwakeConnected_CheckedChanged;
        radioKeepAwakeAlways.CheckedChanged -= RadioKeepAwakeAlways_CheckedChanged;
        switch (_macroDeckDevice.Configuration.WakeLockMethod)
        {
            case WakeLockMethod.Never:
                radioKeepAwakeNever.Checked = true;
                break;
            case WakeLockMethod.Connected:
                radioKeepAwakeConnected.Checked = true;
                break;
            case WakeLockMethod.Always:
                radioKeepAwakeAlways.Checked = true;
                break;
        }

        radioKeepAwakeNever.CheckedChanged += RadioKeepAwakeNever_CheckedChanged;
        radioKeepAwakeConnected.CheckedChanged += RadioKeepAwakeConnected_CheckedChanged;
        radioKeepAwakeAlways.CheckedChanged += RadioKeepAwakeAlways_CheckedChanged;
    }

    private void Brightness_Scroll(object sender, EventArgs e)
    {
        if (_macroDeckDevice == null || !_macroDeckDevice.Available)
        {
            return;
        }

        _macroDeckDevice.Configuration.Brightness = brightness.Value / 10.0f;
        DeviceManager.SaveKnownDevices();
        var macroDeckClient = MacroDeckServer.GetMacroDeckClient(_macroDeckDevice.ClientId);
        macroDeckClient?.DeviceMessage.SendConfiguration(macroDeckClient);
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void CheckAutoConnect_CheckedChanged(object sender, EventArgs e)
    {
        Logger.Debug($"已将自动连接设置为 {checkAutoConnect.Checked}");
        if (_macroDeckDevice == null || !_macroDeckDevice.Available)
        {
            return;
        }

        _macroDeckDevice.Configuration.AutoConnect = checkAutoConnect.Checked;
        DeviceManager.SaveKnownDevices();
        var macroDeckClient = MacroDeckServer.GetMacroDeckClient(_macroDeckDevice.ClientId);
        macroDeckClient?.DeviceMessage.SendConfiguration(macroDeckClient);
    }

    private void RadioKeepAwakeNever_CheckedChanged(object sender, EventArgs e)
    {
        if (radioKeepAwakeNever.Checked)
        {
            Logger.Debug("屏幕常亮设置为：从不");
            if (_macroDeckDevice == null || !_macroDeckDevice.Available)
            {
                return;
            }

            _macroDeckDevice.Configuration.WakeLockMethod = WakeLockMethod.Never;
            DeviceManager.SaveKnownDevices();
            var macroDeckClient = MacroDeckServer.GetMacroDeckClient(_macroDeckDevice.ClientId);
            macroDeckClient?.DeviceMessage.SendConfiguration(macroDeckClient);
        }
    }

    private void RadioKeepAwakeConnected_CheckedChanged(object sender, EventArgs e)
    {
        if (radioKeepAwakeConnected.Checked)
        {
            Logger.Debug("屏幕常亮设置为：连接时");
            if (_macroDeckDevice == null || !_macroDeckDevice.Available)
            {
                return;
            }

            _macroDeckDevice.Configuration.WakeLockMethod = WakeLockMethod.Connected;
            DeviceManager.SaveKnownDevices();
            var macroDeckClient = MacroDeckServer.GetMacroDeckClient(_macroDeckDevice.ClientId);
            macroDeckClient?.DeviceMessage.SendConfiguration(macroDeckClient);
        }
    }

    private void RadioKeepAwakeAlways_CheckedChanged(object sender, EventArgs e)
    {
        if (radioKeepAwakeAlways.Checked)
        {
            Logger.Debug("屏幕常亮设置为：始终");
            if (_macroDeckDevice == null || !_macroDeckDevice.Available)
            {
                return;
            }

            _macroDeckDevice.Configuration.WakeLockMethod = WakeLockMethod.Always;
            DeviceManager.SaveKnownDevices();
            var macroDeckClient = MacroDeckServer.GetMacroDeckClient(_macroDeckDevice.ClientId);
            macroDeckClient?.DeviceMessage.SendConfiguration(macroDeckClient);
        }
    }
}
