using System.Diagnostics;
using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Folders;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Profiles;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class AddFolder : DialogForm
{
    private MacroDeckFolder ParentFolder;
    private MacroDeckFolder Folder;

    /// <summary>
    /// 用于缓存控件原始高度，在每次字体变更后基于原始高度做动态重算，保证幂等性
    /// </summary>
    private readonly Dictionary<Control, int> _originalHeights = new();

    /// <summary>
    /// 用于缓存控件原始 Y 坐标，在每次字体变更后基于原始位置重算布局
    /// </summary>
    private readonly Dictionary<Control, int> _originalTops = new();

    /// <summary>
    /// 缓存窗体的原始 ClientSize，用于宽度和高度动态重算的基准
    /// </summary>
    private Size _originalClientSize;

    public AddFolder(MacroDeckFolder parentFolder)
    {
        ParentFolder = parentFolder;
        InitializeComponent();
        UpdateTranslation();
        btnCreateFolder.Text = LanguageManager.Strings.Create;
    }

    public AddFolder(MacroDeckFolder folder, bool rename)
    {
        Folder = folder;
        InitializeComponent();
        UpdateTranslation();
        btnCreateFolder.Text = LanguageManager.Strings.Save;
        folderName.Enabled = !folder.IsRootFolder;
    }

    private void UpdateTranslation()
    {
        lblFolderName.Text = LanguageManager.Strings.FolderName;
        lblApplication.Text = LanguageManager.Strings.Application;
        lblDevices.Text = LanguageManager.Strings.Devices;
        groupAutomaticallySwitchFolder.Text = LanguageManager.Strings.AutomaticallySwitchToFolder;
        radioNever.Text = LanguageManager.Strings.Never;
        radioOnFocus.Text = LanguageManager.Strings.OnApplicationFocus;
    }

    private void BtnCreateFolder_Click(object sender, EventArgs e)
    {
        if (folderName.Text.Length < 1)
        {
            return;
        }

        if (Folder == null)
        {
            if (ProfileManager.CurrentProfile.Folders.Find(x =>
                    x.DisplayName.Equals(folderName.Text, StringComparison.OrdinalIgnoreCase)) !=
                null)
            {
                using var msgBox = new MessageBox();
                msgBox.ShowDialog(LanguageManager.Strings.CantCreateFolder,
                    string.Format(LanguageManager.Strings.FolderCalledXAlreadyExists, folderName.Text),
                    MessageBoxButtons.OK);
                msgBox.Dispose();
                return;
            }

            if (ParentFolder == null || !ProfileManager.CurrentProfile.Folders.Contains(ParentFolder))
            {
                ParentFolder = ProfileManager.CurrentProfile.Folders[0];
            }

            Folder = ProfileManager.CreateFolder(folderName.Text, ParentFolder, ProfileManager.CurrentProfile);
        }
        else
        {
            if (ProfileManager.CurrentProfile.Folders.Find(x =>
                    x.DisplayName.Equals(folderName.Text, StringComparison.OrdinalIgnoreCase)) !=
                null &&
                ProfileManager.CurrentProfile.Folders.Find(x =>
                    x.DisplayName.Equals(folderName.Text, StringComparison.OrdinalIgnoreCase)) !=
                Folder)
            {
                using var msgBox = new MessageBox();
                msgBox.ShowDialog(LanguageManager.Strings.CantCreateFolder,
                    string.Format(LanguageManager.Strings.FolderCalledXAlreadyExists, folderName.Text),
                    MessageBoxButtons.OK);
                msgBox.Dispose();
                return;
            }
        }

        Folder.DisplayName = folderName.Text;
        Folder.ApplicationToTrigger = applicationList.Text;

        if (radioOnFocus.Checked && applicationList.Text.Length > 0 && devicesList.CheckedItems.Count > 0)
        {
            Folder.ApplicationToTrigger = applicationList.Text;
            Folder.ApplicationsFocusDevices = new List<string>();
            foreach (int i in devicesList.CheckedIndices)
            {
                Folder.ApplicationsFocusDevices.Add(DeviceManager
                    .GetMacroDeckDeviceByDisplayName(devicesList.Items[i].ToString()).ClientId);
            }
        }
        else
        {
            Folder.ApplicationToTrigger = "";
            Folder.ApplicationsFocusDevices = new List<string>();
        }

        ProfileManager.Save();

        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>
    /// 覆盖 OnLoad，在基类字体应用完成后动态重算所有控件的 Size 和布局。
    /// 基类 DialogForm.OnLoad 已调用 FontManager.Apply(this) 替换了所有控件字体，
    /// 此时 Designer 中硬编码的 Size 可能已不再适合新字号，需要根据当前字体重算。
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // 首次加载时缓存控件原始尺寸，供后续字体变更时幂等重算
        CacheOriginalSizes();
        AdjustLayoutForFont();
    }

    /// <summary>
    /// 缓存所有需要动态调整的控件的原始高度和 Y 坐标。
    /// 此方法仅在首次 OnLoad 时调用一次，后续字体变更时基于缓存值重算，保证幂等性。
    /// </summary>
    private void CacheOriginalSizes()
    {
        CacheControl(lblFolderName);
        CacheControl(lblApplication);
        CacheControl(lblDevices);
        CacheControl(radioNever);
        CacheControl(radioOnFocus);
        CacheControl(applicationList);
        CacheControl(btnCreateFolder);
        CacheControl(btnReloadApplications);
        CacheControl(devicesList);
        CacheControl(applicationDeviceSettings);
        CacheControl(groupAutomaticallySwitchFolder);
        CacheControl(folderName);
        if (_originalClientSize.IsEmpty)
        {
            _originalClientSize = ClientSize;
        }

        void CacheControl(Control c)
        {
            if (!_originalHeights.ContainsKey(c))
            {
                _originalHeights[c] = c.Height;
                _originalTops[c] = c.Top;
            }
        }
    }

    /// <summary>
    /// 根据当前字体动态重算所有控件的 Size 和布局。
    /// 计算逻辑：以 TextRenderer.MeasureText 测量当前字体下的文字高度为基准，
    /// 加上适当的 padding，确保文字不被裁剪。布局沿垂直方向依次排列，保持控件间
    /// 的原始间距比例。
    /// </summary>
    private void AdjustLayoutForFont()
    {
        // === 计算各字号分组在当前字体下的文字高度 ===
        var textHeight12 = TextRenderer.MeasureText("Ay", lblFolderName.Font).Height + 1;
        var textHeight1125 = TextRenderer.MeasureText("Ay", lblApplication.Font).Height + 1;
        var textHeight975 = TextRenderer.MeasureText("Ay", btnCreateFolder.Font).Height + 1;
        var textHeightGroupBox = TextRenderer.MeasureText("Ay", groupAutomaticallySwitchFolder.Font).Height + 1;

        // === 第一行: 标签 + 文本框（y=9 基线） ===
        lblFolderName.Height = Math.Max(_originalHeights[lblFolderName], textHeight12 + 4);
        // folderName (RoundedTextBox) 已由自身的 Font setter → UpdateControlHeight 自动调整
        var topRowBottom = Math.Max(lblFolderName.Bottom, folderName.Bottom);

        // === GroupBox 区域 ===
        // GroupBox 标题高度会影响内部控件的起始 Y
        var groupTitleArea = textHeightGroupBox + 6;

        // RadioButton 行
        radioNever.Height = Math.Max(_originalHeights[radioNever], textHeight1125 + 4);
        radioOnFocus.Height = Math.Max(_originalHeights[radioOnFocus], textHeight1125 + 4);
        radioNever.Top = groupTitleArea;
        radioOnFocus.Top = radioNever.Top;

        // Panel 起始位置（RadioButton 下方）
        applicationDeviceSettings.Top = radioNever.Bottom + 6;

        // === Panel 内部布局 ===
        // 应用选择行
        lblApplication.Height = Math.Max(_originalHeights[lblApplication], textHeight1125 + 4);
        applicationList.Height = Math.Max(
            CalculateComboHeight(applicationList),
            textHeight1125 + 8);
        btnReloadApplications.Height = Math.Max(_originalHeights[btnReloadApplications], textHeight975 + 6);

        var appRowHeight = new[] { lblApplication.Height, applicationList.Height, btnReloadApplications.Height }.Max();
        lblApplication.Height = appRowHeight;
        lblApplication.Top = 4;
        applicationList.Height = appRowHeight;
        applicationList.Top = 2;
        btnReloadApplications.Height = appRowHeight;
        btnReloadApplications.Top = 2;

        // 设备选择行
        lblDevices.Height = Math.Max(_originalHeights[lblDevices], textHeight1125 + 4);
        lblDevices.Top = appRowHeight + 8;

        devicesList.Top = lblDevices.Bottom + 4;
        devicesList.Height = Math.Max(
            _originalHeights[devicesList],
            textHeight1125 * 6 + 4); // 至少 6 行高度

        // Panel 高度 = 设备列表底部 + 边距
        applicationDeviceSettings.Height = Math.Max(
            _originalHeights[applicationDeviceSettings],
            devicesList.Bottom + 8);

        // GroupBox 高度 = Panel 底部 + 边距
        groupAutomaticallySwitchFolder.Height = Math.Max(
            _originalHeights[groupAutomaticallySwitchFolder],
            applicationDeviceSettings.Bottom + 10);

        // GroupBox 位置（第一行下方）
        groupAutomaticallySwitchFolder.Top = topRowBottom + 14;

        // === 底部按钮 ===
        btnCreateFolder.Height = Math.Max(_originalHeights[btnCreateFolder], textHeight975 + 10);
        btnCreateFolder.Top = groupAutomaticallySwitchFolder.Bottom + 8;

        // === 窗体 ClientSize ===
        var requiredWidth = Math.Max(
            groupAutomaticallySwitchFolder.Right + 12,
            btnCreateFolder.Right + 12);
        var requiredHeight = btnCreateFolder.Bottom + 12;
        var minSize = _originalClientSize.IsEmpty ? new Size(468, 368) : _originalClientSize;
        ClientSize = new Size(
            Math.Max(minSize.Width, requiredWidth),
            Math.Max(minSize.Height, requiredHeight));
    }

    /// <summary>
    /// 计算 ComboBox 在当前字体下的合理高度。
    /// 对于旧版 CustomControls.ComboBox（标记为 Obsolete），
    /// 它没有 Font 覆盖和自动高度调整，因此需要手动重算。
    /// </summary>
    private static int CalculateComboHeight(Control combo)
    {
        var textHeight = TextRenderer.MeasureText("Ay", combo.Font).Height + 1;
        return textHeight + 8;
    }

    private void AddFolder_Load(object sender, EventArgs e)
    {
        if (Folder != null)
        {
            folderName.Text = Folder.DisplayName;
            if (Folder.ApplicationToTrigger.Length > 0 && Folder.ApplicationsFocusDevices.Count > 0)
            {
                radioOnFocus.CheckedChanged -= RadioOnFocus_CheckedChanged;
                radioOnFocus.Checked = true;
                radioOnFocus.CheckedChanged += RadioOnFocus_CheckedChanged;
            }
        }
    }

    private void RadioOnFocus_CheckedChanged(object sender, EventArgs e)
    {
        if (radioOnFocus.Checked && !radioNever.Checked)
        {
            applicationDeviceSettings.Enabled = true;
            Task.Run(() => { LoadApplications(); });
            LoadDevices();
        }
        else
        {
            applicationDeviceSettings.Enabled = false;
            devicesList.Items.Clear();
        }
    }

    private void LoadApplications()
    {
        var processCollection = Process.GetProcesses();
        foreach (var p in processCollection)
        {
            Invoke(() =>
            {
                if (!applicationList.Items.Contains(p.ProcessName) && !string.IsNullOrEmpty(p.MainWindowTitle))
                {
                    applicationList.Items.Add(p.ProcessName);
                }
            });
        }

        if (Folder != null)
        {
            Invoke(() =>
            {
                if (Folder.ApplicationToTrigger.Length > 0 &&
                    !applicationList.Items.Contains(Folder.ApplicationToTrigger))
                {
                    applicationList.Items.Add(Folder.ApplicationToTrigger);
                }

                applicationList.Text = Folder.ApplicationToTrigger;
            });
        }
    }

    private void LoadDevices()
    {
        devicesList.Items.Clear();

        foreach (var macroDeckDevice in DeviceManager.GetKnownDevices())
        {
            devicesList.Items.Add(macroDeckDevice.DisplayName,
                Folder != null &&
                Folder.ApplicationsFocusDevices != null &&
                Folder.ApplicationsFocusDevices.Contains(macroDeckDevice.ClientId));
        }
    }

    private void BtnReloadApplications_Click(object sender, EventArgs e)
    {
        Task.Run(() => { LoadApplications(); });
    }
}
