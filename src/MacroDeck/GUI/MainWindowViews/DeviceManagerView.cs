using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.GUI.MainWindowViews;

/// <summary>
/// 设备管理视图，用于管理已知设备列表和新连接行为策略。
/// </summary>
public partial class DeviceManagerView : UserControl
{
    /// <summary>
    /// 初始化设备管理视图，加载界面文本翻译。
    /// </summary>
    public DeviceManagerView()
    {
        InitializeComponent();
        Dock = DockStyle.Fill;
        Name = LanguageManager.Strings.DeviceManagerTitle;
        lblKnownDevices.Text = LanguageManager.Strings.KnownDevices;
        lblBehaviour.Text = LanguageManager.Strings.Behaviour;
        radioAllowAll.Text = LanguageManager.Strings.AllowAllNewConnections;
        radioAskNewConnections.Text = LanguageManager.Strings.AskOnNewConnections;
        radioBlockNew.Text = LanguageManager.Strings.BlockAllNewConnections;
    }

    /// <summary>
    /// 设备管理页面加载时初始化，加载设备列表并注册事件监听。
    /// </summary>
    private void DeviceManagerPage_Load(object sender, EventArgs e)
    {
        LoadDevices();
        MacroDeckServer.OnDeviceConnectionStateChanged += OnClientsChanged;
        DeviceManager.OnDevicesChange += OnClientsChanged;
        // 先移除事件，防止重复注册导致多次触发
        radioAllowAll.CheckedChanged -= RadioBehaviour_CheckedChanged;
        radioAskNewConnections.CheckedChanged -= RadioBehaviour_CheckedChanged;
        radioBlockNew.CheckedChanged -= RadioBehaviour_CheckedChanged;
        radioAllowAll.Checked
            = !MacroDeck.Configuration.AskOnNewConnections && !MacroDeck.Configuration.BlockNewConnections;
        radioAskNewConnections.Checked = MacroDeck.Configuration.AskOnNewConnections;
        radioBlockNew.Checked = MacroDeck.Configuration.BlockNewConnections;
        radioAllowAll.CheckedChanged += RadioBehaviour_CheckedChanged;
        radioAskNewConnections.CheckedChanged += RadioBehaviour_CheckedChanged;
        radioBlockNew.CheckedChanged += RadioBehaviour_CheckedChanged;
    }

    /// <summary>
    /// 客户端设备状态发生变化时重新加载设备列表。
    /// </summary>
    private void OnClientsChanged(object sender, EventArgs e)
    {
        LoadDevices();
    }

    /// <summary>
    /// 加载已知设备列表到界面，移除已断开的设备并添加新设备。
    /// </summary>
    private void LoadDevices()
    {
        if (InvokeRequired)
        {
            Invoke(() => LoadDevices());
            return;
        }

        // 遍历现有控件，清理已不在已知列表中的设备
        foreach (var control in devicesList.Controls.OfType<DeviceInfo>())
        {
            if (!DeviceManager.GetKnownDevices().Contains(control.MacroDeckDevice))
            {
                control.Dispose();
                devicesList.Controls.Remove(control);
                continue;
            }

            control.LoadDevice();
        }

        // 添加尚未在面板中显示的新设备
        foreach (var macroDeckDevice in DeviceManager.GetKnownDevices().ToArray())
        {
            if (devicesList.Controls.OfType<DeviceInfo>()
                    .FirstOrDefault(x => x.MacroDeckDevice.Equals(macroDeckDevice)) !=
                null)
            {
                continue;
            }

            var deviceInfo = new DeviceInfo(macroDeckDevice);
            devicesList.Controls.Add(deviceInfo);
        }
    }

    /// <summary>
    /// 新连接行为策略变更时，保存配置到文件。
    /// </summary>
    private void RadioBehaviour_CheckedChanged(object sender, EventArgs e)
    {
        MacroDeck.Configuration.AskOnNewConnections = radioAskNewConnections.Checked;
        MacroDeck.Configuration.BlockNewConnections = radioBlockNew.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }
}
