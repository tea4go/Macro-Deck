using System.ComponentModel;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.CustomControls.ExtensionsView
{
    partial class ExtensionZipInstallerView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnClose = new PictureButton();
            dlgSelectPluginFile = new OpenFileDialog();
            label1 = new Label();
            lblPackageId = new Label();
            lblZipPath = new Label();
            txtAuthor = new RoundedTextBox();
            txtPackageId = new RoundedTextBox();
            txtZipPath = new RoundedTextBox();
            btnInstall = new ButtonPrimary();
            btnSelectFile = new ButtonPrimary();
            roundedTextBox1 = new RoundedTextBox();
            ((ISupportInitialize)btnClose).BeginInit();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Transparent;
            btnClose.BackgroundImage = Properties.Resources.Close_Normal;
            btnClose.BackgroundImageLayout = ImageLayout.Stretch;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.HoverImage = Properties.Resources.Close_Hover;
            btnClose.Location = new Point(10, 7);
            btnClose.Margin = new Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(25, 25);
            btnClose.TabIndex = 3;
            btnClose.TabStop = false;
            btnClose.Click += btnClose_Click;
            // 
            // dlgSelectPluginFile
            // 
            dlgSelectPluginFile.DefaultExt = "zip";
            dlgSelectPluginFile.Filter = "Macro Deck Plugin (*.macroDeckPlugin)|*.macroDeckPlugin|Macro Deck Icon Pack (*.macroDeckIconPack)|*.macroDeckIconPack|Zip Archive (*.zip)|*.zip";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(11, 151);
            label1.Margin = new Padding(6);
            label1.Name = "label1";
            label1.Padding = new Padding(2);
            label1.Size = new Size(75, 29);
            label1.TabIndex = 2;
            label1.Text = "Author:";
            // 
            // lblPackageId
            // 
            lblPackageId.AutoSize = true;
            lblPackageId.Font = new Font("Segoe UI", 9F);
            lblPackageId.ForeColor = Color.White;
            lblPackageId.Location = new Point(11, 107);
            lblPackageId.Margin = new Padding(6);
            lblPackageId.Name = "lblPackageId";
            lblPackageId.Padding = new Padding(2);
            lblPackageId.Size = new Size(107, 29);
            lblPackageId.TabIndex = 1;
            lblPackageId.Text = "Package ID:";
            // 
            // lblZipPath
            // 
            lblZipPath.AutoSize = true;
            lblZipPath.Font = new Font("Segoe UI", 9F);
            lblZipPath.ForeColor = Color.White;
            lblZipPath.Location = new Point(11, 59);
            lblZipPath.Margin = new Padding(6);
            lblZipPath.Name = "lblZipPath";
            lblZipPath.Padding = new Padding(2);
            lblZipPath.Size = new Size(54, 29);
            lblZipPath.TabIndex = 8;
            lblZipPath.Text = "Path:";
            // 
            // txtAuthor
            // 
            txtAuthor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAuthor.BackColor = Color.FromArgb(45, 45, 45);
            txtAuthor.Cursor = Cursors.Hand;
            txtAuthor.Enabled = false;
            txtAuthor.Font = new Font("Tahoma", 9F);
            txtAuthor.ForeColor = Color.White;
            txtAuthor.Icon = null;
            txtAuthor.Location = new Point(133, 137);
            txtAuthor.MaxCharacters = 32767;
            txtAuthor.Multiline = false;
            txtAuthor.Name = "txtAuthor";
            txtAuthor.Padding = new Padding(8, 5, 8, 5);
            txtAuthor.PasswordChar = false;
            txtAuthor.PlaceHolderColor = Color.Gray;
            txtAuthor.PlaceHolderText = "";
            txtAuthor.ReadOnly = true;
            txtAuthor.ScrollBars = ScrollBars.None;
            txtAuthor.SelectionStart = 0;
            txtAuthor.Size = new Size(624, 33);
            txtAuthor.TabIndex = 6;
            txtAuthor.TextAlignment = HorizontalAlignment.Left;
            // 
            // txtPackageId
            // 
            txtPackageId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPackageId.BackColor = Color.FromArgb(45, 45, 45);
            txtPackageId.Cursor = Cursors.Hand;
            txtPackageId.Enabled = false;
            txtPackageId.Font = new Font("Tahoma", 9F);
            txtPackageId.ForeColor = Color.White;
            txtPackageId.Icon = null;
            txtPackageId.Location = new Point(133, 102);
            txtPackageId.MaxCharacters = 32767;
            txtPackageId.Multiline = false;
            txtPackageId.Name = "txtPackageId";
            txtPackageId.Padding = new Padding(8, 5, 8, 5);
            txtPackageId.PasswordChar = false;
            txtPackageId.PlaceHolderColor = Color.Gray;
            txtPackageId.PlaceHolderText = "";
            txtPackageId.ReadOnly = true;
            txtPackageId.ScrollBars = ScrollBars.None;
            txtPackageId.SelectionStart = 0;
            txtPackageId.Size = new Size(624, 33);
            txtPackageId.TabIndex = 4;
            txtPackageId.TextAlignment = HorizontalAlignment.Left;
            // 
            // txtZipPath
            // 
            txtZipPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtZipPath.BackColor = Color.FromArgb(65, 65, 65);
            txtZipPath.Cursor = Cursors.Hand;
            txtZipPath.Font = new Font("Tahoma", 9F);
            txtZipPath.ForeColor = Color.White;
            txtZipPath.Icon = null;
            txtZipPath.Location = new Point(133, 56);
            txtZipPath.MaxCharacters = 32767;
            txtZipPath.Multiline = false;
            txtZipPath.Name = "txtZipPath";
            txtZipPath.Padding = new Padding(8, 5, 8, 5);
            txtZipPath.PasswordChar = false;
            txtZipPath.PlaceHolderColor = Color.Gray;
            txtZipPath.PlaceHolderText = "";
            txtZipPath.ReadOnly = true;
            txtZipPath.ScrollBars = ScrollBars.None;
            txtZipPath.SelectionStart = 0;
            txtZipPath.Size = new Size(578, 33);
            txtZipPath.TabIndex = 10;
            txtZipPath.TextAlignment = HorizontalAlignment.Left;
            // 
            // btnInstall
            // 
            btnInstall.Anchor = AnchorStyles.Top;
            btnInstall.BorderRadius = 8;
            btnInstall.Cursor = Cursors.Hand;
            btnInstall.Enabled = false;
            btnInstall.FlatAppearance.BorderSize = 0;
            btnInstall.FlatStyle = FlatStyle.Flat;
            btnInstall.Font = new Font("Tahoma", 9.75F);
            btnInstall.ForeColor = Color.White;
            btnInstall.HoverColor = Color.Empty;
            btnInstall.Icon = null;
            btnInstall.Location = new Point(316, 372);
            btnInstall.Name = "btnInstall";
            btnInstall.Progress = 0;
            btnInstall.ProgressColor = Color.FromArgb(0, 103, 225);
            btnInstall.Size = new Size(150, 40);
            btnInstall.TabIndex = 5;
            btnInstall.Text = "Install";
            btnInstall.UseVisualStyleBackColor = true;
            btnInstall.UseWindowsAccentColor = true;
            btnInstall.WriteProgress = true;
            btnInstall.Click += BtnInstall_Click;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectFile.BorderRadius = 8;
            btnSelectFile.Cursor = Cursors.Hand;
            btnSelectFile.FlatAppearance.BorderSize = 0;
            btnSelectFile.FlatStyle = FlatStyle.Flat;
            btnSelectFile.Font = new Font("Tahoma", 9.75F);
            btnSelectFile.ForeColor = Color.White;
            btnSelectFile.HoverColor = Color.Empty;
            btnSelectFile.Icon = null;
            btnSelectFile.Location = new Point(717, 55);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Progress = 0;
            btnSelectFile.ProgressColor = Color.FromArgb(0, 103, 225);
            btnSelectFile.Size = new Size(40, 37);
            btnSelectFile.TabIndex = 8;
            btnSelectFile.Text = "...";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.UseWindowsAccentColor = true;
            btnSelectFile.WriteProgress = true;
            btnSelectFile.Click += BtnSelectFile_Click;
            // 
            // roundedTextBox1
            // 
            roundedTextBox1.Anchor = AnchorStyles.Top;
            roundedTextBox1.BackColor = Color.FromArgb(45, 45, 45);
            roundedTextBox1.Cursor = Cursors.Hand;
            roundedTextBox1.Font = new Font("Tahoma", 9F, FontStyle.Italic);
            roundedTextBox1.Icon = null;
            roundedTextBox1.Location = new Point(133, 190);
            roundedTextBox1.MaxCharacters = 32767;
            roundedTextBox1.Multiline = true;
            roundedTextBox1.Name = "roundedTextBox1";
            roundedTextBox1.Padding = new Padding(8, 5, 8, 5);
            roundedTextBox1.PasswordChar = false;
            roundedTextBox1.PlaceHolderColor = Color.Gold;
            roundedTextBox1.PlaceHolderText = "Warning: You should only install plugins from trusted sources. Installing untrusted and/or untested plugins can be harmful to your device!";
            roundedTextBox1.ReadOnly = true;
            roundedTextBox1.ScrollBars = ScrollBars.None;
            roundedTextBox1.SelectionStart = 0;
            roundedTextBox1.Size = new Size(611, 158);
            roundedTextBox1.TabIndex = 11;
            roundedTextBox1.TextAlignment = HorizontalAlignment.Left;
            // 
            // ExtensionZipInstallerView
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(label1);
            Controls.Add(roundedTextBox1);
            Controls.Add(lblPackageId);
            Controls.Add(btnInstall);
            Controls.Add(lblZipPath);
            Controls.Add(txtAuthor);
            Controls.Add(btnClose);
            Controls.Add(txtPackageId);
            Controls.Add(txtZipPath);
            Controls.Add(btnSelectFile);
            Font = new Font("Segoe UI", 8.25F);
            Name = "ExtensionZipInstallerView";
            Padding = new Padding(3);
            Size = new Size(777, 428);
            ((ISupportInitialize)btnClose).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private PictureButton btnClose;
        private OpenFileDialog dlgSelectPluginFile;
        private Label label1;
        private Label lblPackageId;
        private RoundedTextBox txtPackageId;
        private RoundedTextBox txtAuthor;
        private ButtonPrimary btnInstall;
        private ButtonPrimary btnSelectFile;
        private RoundedTextBox txtZipPath;
        private Label lblZipPath;
        private RoundedTextBox roundedTextBox1;
    }
}
