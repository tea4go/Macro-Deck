using SuchByte.MacroDeck.Backup;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class RestoreBackupDialog : DialogForm
{
    private Size _originalClientSize;

    public RestoreBackupDialog()
    {
        InitializeComponent();

        lblWhatToRestore.Text = LanguageManager.Strings.WhatDoYouWantToRestore;
        checkRestoreConfig.Text = LanguageManager.Strings.Configuration;
        checkRestoreProfiles.Text = LanguageManager.Strings.Profiles;
        checkRestoreDevices.Text = LanguageManager.Strings.KnownDevices;
        checkRestoreVariables.Text = LanguageManager.Strings.Variables;
        checkRestorePlugins.Text = LanguageManager.Strings.InstalledPlugins;
        checkRestorePluginConfigs.Text = LanguageManager.Strings.PluginConfigurations;
        checkRestorePluginCredentials.Text = LanguageManager.Strings.PluginCredentials;
        checkRestoreIconPacks.Text = LanguageManager.Strings.InstalledIconPacks;
        btnRestore.Text = LanguageManager.Strings.Restore;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustLabelHeight(lblWhatToRestore);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }


    public RestoreBackupInfo RestoreBackupInfo
    {
        get
        {
            var restoreBackupInfo = new RestoreBackupInfo
            {
                RestoreConfig = checkRestoreConfig.Checked,
                RestoreProfiles = checkRestoreProfiles.Checked,
                RestoreDevices = checkRestoreDevices.Checked,
                RestoreVariables = checkRestoreVariables.Checked,
                RestorePlugins = checkRestorePlugins.Checked,
                RestorePluginConfigs = checkRestorePluginConfigs.Checked,
                RestorePluginCredentials = checkRestorePluginCredentials.Checked,
                RestoreIconPacks = checkRestoreIconPacks.Checked
            };

            return restoreBackupInfo;
        }
    }

    private void BtnRestore_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }
}
