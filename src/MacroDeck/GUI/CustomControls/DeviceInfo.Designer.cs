
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.CustomControls
{
    partial class DeviceInfo
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            lblDisplayName = new Label();
            displayName = new RoundedTextBox();
            btnRemove = new PictureButton();
            lblIdLabel = new Label();
            lblId = new Label();
            checkBlockConnection = new CheckBox();
            lblStatusLabel = new Label();
            lblStatus = new Label();
            btnChangeDisplayName = new PictureButton();
            profiles = new RoundedComboBox();
            lblProfile = new Label();
            btnConfigure = new ButtonPrimary();
            ((ISupportInitialize)btnRemove).BeginInit();
            ((ISupportInitialize)btnChangeDisplayName).BeginInit();
            SuspendLayout();
            // 
            // lblDisplayName
            // 
            lblDisplayName.AutoSize = true;
            lblDisplayName.Font = new Font("Tahoma", 12F);
            lblDisplayName.ForeColor = Color.White;
            lblDisplayName.Location = new Point(5, 157);
            lblDisplayName.Name = "lblDisplayName";
            lblDisplayName.Size = new Size(163, 29);
            lblDisplayName.TabIndex = 0;
            lblDisplayName.Text = "Display name:";
            lblDisplayName.UseMnemonic = false;
            // 
            // displayName
            // 
            displayName.BackColor = Color.FromArgb(65, 65, 65);
            displayName.Cursor = Cursors.Hand;
            displayName.Font = new Font("Tahoma", 9.75F);
            displayName.ForeColor = Color.White;
            displayName.Icon = null;
            displayName.Location = new Point(198, 153);
            displayName.MaxCharacters = 32767;
            displayName.Multiline = false;
            displayName.Name = "displayName";
            displayName.Padding = new Padding(8, 5, 8, 5);
            displayName.PasswordChar = false;
            displayName.PlaceHolderColor = Color.Gray;
            displayName.PlaceHolderText = "";
            displayName.ReadOnly = false;
            displayName.ScrollBars = ScrollBars.None;
            displayName.SelectionStart = 0;
            displayName.Size = new Size(391, 35);
            displayName.TabIndex = 1;
            displayName.TextAlignment = HorizontalAlignment.Left;
            // 
            // btnRemove
            // 
            btnRemove.BackColor = Color.Transparent;
            btnRemove.BackgroundImage = Resources.Delete_Normal;
            btnRemove.BackgroundImageLayout = ImageLayout.Stretch;
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.Font = new Font("Tahoma", 9.75F);
            btnRemove.ForeColor = Color.White;
            btnRemove.HoverImage = Resources.Delete_Hover;
            btnRemove.Location = new Point(594, 9);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(32, 32);
            btnRemove.TabIndex = 2;
            btnRemove.TabStop = false;
            btnRemove.Click += BtnRemove_Click;
            // 
            // lblIdLabel
            // 
            lblIdLabel.AutoSize = true;
            lblIdLabel.Font = new Font("Tahoma", 12F);
            lblIdLabel.ForeColor = Color.White;
            lblIdLabel.Location = new Point(5, 58);
            lblIdLabel.Name = "lblIdLabel";
            lblIdLabel.Size = new Size(112, 29);
            lblIdLabel.TabIndex = 3;
            lblIdLabel.Text = "Client ID:";
            lblIdLabel.UseMnemonic = false;
            // 
            // lblId
            // 
            lblId.Font = new Font("Tahoma", 14.25F);
            lblId.ForeColor = Color.White;
            lblId.Location = new Point(198, 59);
            lblId.Name = "lblId";
            lblId.Size = new Size(194, 30);
            lblId.TabIndex = 4;
            lblId.UseMnemonic = false;
            // 
            // checkBlockConnection
            // 
            checkBlockConnection.Font = new Font("Tahoma", 12F);
            checkBlockConnection.ForeColor = Color.White;
            checkBlockConnection.Location = new Point(13, 10);
            checkBlockConnection.Name = "checkBlockConnection";
            checkBlockConnection.Size = new Size(167, 40);
            checkBlockConnection.TabIndex = 5;
            checkBlockConnection.Text = "Block connection";
            checkBlockConnection.UseMnemonic = false;
            checkBlockConnection.UseVisualStyleBackColor = true;
            checkBlockConnection.CheckedChanged += CheckBlockConnection_CheckedChanged;
            // 
            // lblStatusLabel
            // 
            lblStatusLabel.AutoSize = true;
            lblStatusLabel.Font = new Font("Tahoma", 12F);
            lblStatusLabel.ForeColor = Color.White;
            lblStatusLabel.Location = new Point(5, 105);
            lblStatusLabel.Name = "lblStatusLabel";
            lblStatusLabel.Size = new Size(87, 29);
            lblStatusLabel.TabIndex = 6;
            lblStatusLabel.Text = "Status:";
            lblStatusLabel.UseMnemonic = false;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Tahoma", 12F);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(198, 105);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(226, 23);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "?";
            lblStatus.UseMnemonic = false;
            // 
            // btnChangeDisplayName
            // 
            btnChangeDisplayName.BackColor = Color.Transparent;
            btnChangeDisplayName.BackgroundImage = Resources.Edit_Normal;
            btnChangeDisplayName.BackgroundImageLayout = ImageLayout.Stretch;
            btnChangeDisplayName.Cursor = Cursors.Hand;
            btnChangeDisplayName.Font = new Font("Tahoma", 9.75F);
            btnChangeDisplayName.ForeColor = Color.White;
            btnChangeDisplayName.HoverImage = Resources.Edit_Hover;
            btnChangeDisplayName.Location = new Point(593, 160);
            btnChangeDisplayName.Name = "btnChangeDisplayName";
            btnChangeDisplayName.Size = new Size(32, 32);
            btnChangeDisplayName.TabIndex = 8;
            btnChangeDisplayName.TabStop = false;
            btnChangeDisplayName.Click += BtnChangeDisplayName_Click;
            // 
            // profiles
            // 
            profiles.BackColor = Color.FromArgb(65, 65, 65);
            profiles.Cursor = Cursors.Hand;
            profiles.DropDownStyle = ComboBoxStyle.DropDownList;
            profiles.Font = new Font("Tahoma", 9.75F);
            profiles.ForeColor = Color.White;
            profiles.Icon = null;
            profiles.Location = new Point(198, 205);
            profiles.Margin = new Padding(0);
            profiles.Name = "profiles";
            profiles.SelectedIndex = -1;
            profiles.SelectedItem = null;
            profiles.Size = new Size(428, 38);
            profiles.TabIndex = 9;
            profiles.SelectedIndexChanged += Profiles_SelectedIndexChanged;
            // 
            // lblProfile
            // 
            lblProfile.AutoSize = true;
            lblProfile.Font = new Font("Tahoma", 12F);
            lblProfile.ForeColor = Color.White;
            lblProfile.Location = new Point(5, 210);
            lblProfile.Name = "lblProfile";
            lblProfile.Size = new Size(87, 29);
            lblProfile.TabIndex = 10;
            lblProfile.Text = "Profile:";
            lblProfile.UseMnemonic = false;
            // 
            // btnConfigure
            // 
            btnConfigure.BorderRadius = 8;
            btnConfigure.Cursor = Cursors.Hand;
            btnConfigure.FlatAppearance.BorderSize = 0;
            btnConfigure.FlatStyle = FlatStyle.Flat;
            btnConfigure.Font = new Font("Tahoma", 9.75F);
            btnConfigure.ForeColor = Color.White;
            btnConfigure.HoverColor = Color.FromArgb(0, 89, 184);
            btnConfigure.Icon = null;
            btnConfigure.Location = new Point(445, 48);
            btnConfigure.Name = "btnConfigure";
            btnConfigure.Progress = 0;
            btnConfigure.ProgressColor = Color.FromArgb(0, 46, 94);
            btnConfigure.Size = new Size(181, 53);
            btnConfigure.TabIndex = 13;
            btnConfigure.Text = "Device settings";
            btnConfigure.UseMnemonic = false;
            btnConfigure.UseVisualStyleBackColor = false;
            btnConfigure.UseWindowsAccentColor = true;
            btnConfigure.WriteProgress = true;
            btnConfigure.Click += BtnConfigure_Click;
            // 
            // DeviceInfo
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(35, 35, 35);
            Controls.Add(btnConfigure);
            Controls.Add(lblProfile);
            Controls.Add(profiles);
            Controls.Add(btnChangeDisplayName);
            Controls.Add(lblStatus);
            Controls.Add(lblStatusLabel);
            Controls.Add(checkBlockConnection);
            Controls.Add(lblId);
            Controls.Add(lblIdLabel);
            Controls.Add(btnRemove);
            Controls.Add(displayName);
            Controls.Add(lblDisplayName);
            Font = new Font("Tahoma", 9F);
            Name = "DeviceInfo";
            Size = new Size(640, 270);
            Load += DeviceInfo_Load;
            ((ISupportInitialize)btnRemove).EndInit();
            ((ISupportInitialize)btnChangeDisplayName).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label lblDisplayName;
        private RoundedTextBox displayName;
        private PictureButton btnRemove;
        private Label lblIdLabel;
        private Label lblId;
        private CheckBox checkBlockConnection;
        private Label lblStatusLabel;
        private Label lblStatus;
        private PictureButton btnChangeDisplayName;
        private RoundedComboBox profiles;
        private Label lblProfile;
        private ButtonPrimary btnConfigure;
    }
}
