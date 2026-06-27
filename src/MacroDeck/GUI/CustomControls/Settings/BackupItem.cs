using System.Diagnostics;
using System.IO;
using SuchByte.MacroDeck.Backup;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.GUI.CustomControls.Settings;

public partial class BackupItem : RoundedUserControl
{
    private MacroDeckBackupInfo macroDeckBackupInfo;

    public BackupItem(MacroDeckBackupInfo macroDeckBackupInfo)
    {
        this.macroDeckBackupInfo = macroDeckBackupInfo;
        InitializeComponent();
    }

    private void BackupItem_Load(object sender, EventArgs e)
    {
        lblFileName.Text = new FileInfo(macroDeckBackupInfo.FileName).Name;
        lblDateCreated.Text = LanguageManager.Strings.Created +
            ": " +
            macroDeckBackupInfo.BackupCreated.ToString("d") +
            " - " +
            macroDeckBackupInfo.BackupCreated.ToString("t");
        lblSize.Text = macroDeckBackupInfo.SizeMb.ToString("0.##") + "MB";
    }

    private void BtnRestore_Click(object sender, EventArgs e)
    {
        using var restoreBackupDialog = new RestoreBackupDialog();
        if (restoreBackupDialog.ShowDialog() == DialogResult.OK)
        {
            using var msgBox = new MessageBox();
            if (msgBox.ShowDialog(LanguageManager.Strings.AreYouSure,
                    LanguageManager.Strings.ActionCannotBeReversed,
                    MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                Task.Run(() =>
                {
                    BackupManager.RestoreBackup(macroDeckBackupInfo.FileName, restoreBackupDialog.RestoreBackupInfo);
                    SpinnerDialog.SetVisisble(false, ParentForm);
                });
                SpinnerDialog.SetVisisble(true, ParentForm);
            }
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        using var msgBox = new MessageBox();
        if (msgBox.ShowDialog(LanguageManager.Strings.AreYouSure,
                LanguageManager.Strings.ThisWillDeleteBackupPermanently,
                MessageBoxButtons.YesNo) ==
            DialogResult.Yes)
        {
            BackupManager.DeleteBackup(macroDeckBackupInfo.FileName);
        }
    }

    private void BtnOpenFolder_Click(object? sender, EventArgs e)
    {
        var fullPath = Path.Combine(ApplicationPaths.BackupsDirectoryPath, macroDeckBackupInfo.FileName);
        if (!File.Exists(fullPath))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{fullPath}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, "打开备份目录失败：{FileName}", macroDeckBackupInfo.FileName);
        }
    }
}
