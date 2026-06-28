using System.ComponentModel;
using SuchByte.MacroDeck.GUI.CustomControls;
#pragma warning disable CS0618 // Designer 中使用旧版 ComboBox，待设计器迁移
using ComboBox = SuchByte.MacroDeck.GUI.CustomControls.ComboBox;
#pragma warning restore CS0618

#pragma warning disable CS0618 // Designer 中使用旧版 ComboBox，待设计器迁移
namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class AddFolder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            folderName = new RoundedTextBox();
            lblFolderName = new Label();
            btnCreateFolder = new ButtonPrimary();
            groupAutomaticallySwitchFolder = new GroupBox();
            applicationDeviceSettings = new Panel();
            btnReloadApplications = new ButtonPrimary();
            devicesList = new CheckedListBox();
            lblDevices = new Label();
            applicationList = new ComboBox();
            lblApplication = new Label();
            radioOnFocus = new RadioButton();
            radioNever = new RadioButton();
            groupAutomaticallySwitchFolder.SuspendLayout();
            applicationDeviceSettings.SuspendLayout();
            SuspendLayout();
            // 
            // folderName
            // 
            folderName.BackColor = Color.FromArgb(65, 65, 65);
            folderName.Font = new Font("Tahoma", 9F);
            folderName.ForeColor = Color.White;
            folderName.Icon = null;
            folderName.Location = new Point(195, 9);
            folderName.MaxCharacters = 32767;
            folderName.Multiline = false;
            folderName.Name = "folderName";
            folderName.Padding = new Padding(8, 5, 8, 5);
            folderName.PasswordChar = false;
            folderName.PlaceHolderColor = Color.Gray;
            folderName.PlaceHolderText = "";
            folderName.ReadOnly = false;
            folderName.ScrollBars = ScrollBars.None;
            folderName.SelectionStart = 0;
            folderName.Size = new Size(310, 33);
            folderName.TabIndex = 0;
            folderName.TextAlignment = HorizontalAlignment.Left;
            // 
            // lblFolderName
            // 
            lblFolderName.AutoSize = true;
            lblFolderName.Font = new Font("Tahoma", 12F);
            lblFolderName.ForeColor = Color.White;
            lblFolderName.Location = new Point(11, 9);
            lblFolderName.Name = "lblFolderName";
            lblFolderName.Size = new Size(154, 29);
            lblFolderName.TabIndex = 1;
            lblFolderName.Text = "Folder name:";
            lblFolderName.TextAlign = ContentAlignment.MiddleLeft;
            lblFolderName.UseMnemonic = false;
            // 
            // btnCreateFolder
            // 
            btnCreateFolder.BorderRadius = 8;
            btnCreateFolder.Cursor = Cursors.Hand;
            btnCreateFolder.FlatAppearance.BorderSize = 0;
            btnCreateFolder.FlatStyle = FlatStyle.Flat;
            btnCreateFolder.Font = new Font("Tahoma", 9.75F);
            btnCreateFolder.ForeColor = Color.White;
            btnCreateFolder.HoverColor = Color.FromArgb(0, 89, 184);
            btnCreateFolder.Icon = null;
            btnCreateFolder.Location = new Point(369, 366);
            btnCreateFolder.Name = "btnCreateFolder";
            btnCreateFolder.Progress = 0;
            btnCreateFolder.ProgressColor = Color.FromArgb(0, 46, 94);
            btnCreateFolder.Size = new Size(121, 46);
            btnCreateFolder.TabIndex = 2;
            btnCreateFolder.Text = "Ok";
            btnCreateFolder.UseMnemonic = false;
            btnCreateFolder.UseVisualStyleBackColor = false;
            btnCreateFolder.UseWindowsAccentColor = true;
            btnCreateFolder.WriteProgress = true;
            btnCreateFolder.Click += BtnCreateFolder_Click;
            // 
            // groupAutomaticallySwitchFolder
            // 
            groupAutomaticallySwitchFolder.Controls.Add(applicationDeviceSettings);
            groupAutomaticallySwitchFolder.Controls.Add(radioOnFocus);
            groupAutomaticallySwitchFolder.Controls.Add(radioNever);
            groupAutomaticallySwitchFolder.Font = new Font("Tahoma", 12F);
            groupAutomaticallySwitchFolder.ForeColor = Color.White;
            groupAutomaticallySwitchFolder.Location = new Point(12, 52);
            groupAutomaticallySwitchFolder.Name = "groupAutomaticallySwitchFolder";
            groupAutomaticallySwitchFolder.Size = new Size(493, 308);
            groupAutomaticallySwitchFolder.TabIndex = 3;
            groupAutomaticallySwitchFolder.TabStop = false;
            groupAutomaticallySwitchFolder.Text = "Automatically switch to folder";
            // 
            // applicationDeviceSettings
            // 
            applicationDeviceSettings.Controls.Add(btnReloadApplications);
            applicationDeviceSettings.Controls.Add(devicesList);
            applicationDeviceSettings.Controls.Add(lblDevices);
            applicationDeviceSettings.Controls.Add(applicationList);
            applicationDeviceSettings.Controls.Add(lblApplication);
            applicationDeviceSettings.Enabled = false;
            applicationDeviceSettings.Location = new Point(6, 94);
            applicationDeviceSettings.Name = "applicationDeviceSettings";
            applicationDeviceSettings.Size = new Size(481, 208);
            applicationDeviceSettings.TabIndex = 2;
            // 
            // btnReloadApplications
            // 
            btnReloadApplications.BackgroundImage = Properties.Resources.reload;
            btnReloadApplications.BackgroundImageLayout = ImageLayout.Stretch;
            btnReloadApplications.BorderRadius = 8;
            btnReloadApplications.Cursor = Cursors.Hand;
            btnReloadApplications.FlatAppearance.BorderSize = 0;
            btnReloadApplications.FlatStyle = FlatStyle.Flat;
            btnReloadApplications.Font = new Font("Tahoma", 9.75F);
            btnReloadApplications.ForeColor = Color.White;
            btnReloadApplications.HoverColor = Color.FromArgb(0, 89, 184);
            btnReloadApplications.Icon = Properties.Resources.reload;
            btnReloadApplications.Location = new Point(438, 5);
            btnReloadApplications.Name = "btnReloadApplications";
            btnReloadApplications.Progress = 0;
            btnReloadApplications.ProgressColor = Color.FromArgb(0, 46, 94);
            btnReloadApplications.Size = new Size(38, 37);
            btnReloadApplications.TabIndex = 6;
            btnReloadApplications.UseMnemonic = false;
            btnReloadApplications.UseVisualStyleBackColor = false;
            btnReloadApplications.UseWindowsAccentColor = true;
            btnReloadApplications.WriteProgress = true;
            btnReloadApplications.Click += BtnReloadApplications_Click;
            // 
            // devicesList
            // 
            devicesList.BackColor = Color.FromArgb(65, 65, 65);
            devicesList.BorderStyle = BorderStyle.None;
            devicesList.ForeColor = Color.White;
            devicesList.FormattingEnabled = true;
            devicesList.Location = new Point(6, 84);
            devicesList.Name = "devicesList";
            devicesList.Size = new Size(472, 99);
            devicesList.TabIndex = 5;
            // 
            // lblDevices
            // 
            lblDevices.AutoSize = true;
            lblDevices.Font = new Font("Tahoma", 11.25F);
            lblDevices.ForeColor = Color.White;
            lblDevices.Location = new Point(3, 46);
            lblDevices.Name = "lblDevices";
            lblDevices.Size = new Size(89, 28);
            lblDevices.TabIndex = 4;
            lblDevices.Text = "Devices";
            lblDevices.UseMnemonic = false;
            // 
            // applicationList
            // 
            applicationList.BackColor = Color.FromArgb(65, 65, 65);
            applicationList.BorderRadius = 0;
            applicationList.Cursor = Cursors.Hand;
            applicationList.DropDownStyle = ComboBoxStyle.DropDownList;
            applicationList.Font = new Font("Tahoma", 11.25F);
            applicationList.ForeColor = Color.White;
            applicationList.FormattingEnabled = true;
            applicationList.Location = new Point(102, 9);
            applicationList.Margin = new Padding(0, 0, 0, 0);
            applicationList.Name = "applicationList";
            applicationList.Size = new Size(320, 27);
            applicationList.TabIndex = 3;
            // 
            // lblApplication
            // 
            lblApplication.Font = new Font("Tahoma", 11.25F);
            lblApplication.ForeColor = Color.White;
            lblApplication.Location = new Point(3, 6);
            lblApplication.Name = "lblApplication";
            lblApplication.Size = new Size(93, 24);
            lblApplication.TabIndex = 2;
            lblApplication.Text = "Application:";
            lblApplication.TextAlign = ContentAlignment.MiddleLeft;
            lblApplication.UseMnemonic = false;
            // 
            // radioOnFocus
            // 
            radioOnFocus.AutoSize = true;
            radioOnFocus.Cursor = Cursors.Hand;
            radioOnFocus.Font = new Font("Tahoma", 11.25F);
            radioOnFocus.Location = new Point(143, 47);
            radioOnFocus.Name = "radioOnFocus";
            radioOnFocus.Size = new Size(242, 32);
            radioOnFocus.TabIndex = 1;
            radioOnFocus.Text = "On application focus";
            radioOnFocus.UseMnemonic = false;
            radioOnFocus.UseVisualStyleBackColor = true;
            radioOnFocus.CheckedChanged += RadioOnFocus_CheckedChanged;
            // 
            // radioNever
            // 
            radioNever.AutoSize = true;
            radioNever.Checked = true;
            radioNever.Cursor = Cursors.Hand;
            radioNever.Font = new Font("Tahoma", 11.25F);
            radioNever.Location = new Point(16, 47);
            radioNever.Name = "radioNever";
            radioNever.Size = new Size(95, 32);
            radioNever.TabIndex = 0;
            radioNever.TabStop = true;
            radioNever.Text = "Never";
            radioNever.UseMnemonic = false;
            radioNever.UseVisualStyleBackColor = true;
            radioNever.CheckedChanged += RadioOnFocus_CheckedChanged;
            // 
            // AddFolder
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            ClientSize = new Size(524, 426);
            Controls.Add(groupAutomaticallySwitchFolder);
            Controls.Add(btnCreateFolder);
            Controls.Add(lblFolderName);
            Controls.Add(folderName);
            Name = "AddFolder";
            Text = "Macro Deck :: Create folder";
            Load += AddFolder_Load;
            groupAutomaticallySwitchFolder.ResumeLayout(false);
            groupAutomaticallySwitchFolder.PerformLayout();
            applicationDeviceSettings.ResumeLayout(false);
            applicationDeviceSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private RoundedTextBox folderName;
        private Label lblFolderName;
        private ButtonPrimary btnCreateFolder;
        private GroupBox groupAutomaticallySwitchFolder;
        private RadioButton radioOnFocus;
        private RadioButton radioNever;
        private Panel applicationDeviceSettings;
        private CheckedListBox devicesList;
        private Label lblDevices;
        private ComboBox applicationList;
        private Label lblApplication;
        private ButtonPrimary btnReloadApplications;
    }
}