using System.ComponentModel;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI
{
    partial class IconSelector
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
            iconList = new IconGrid();
            btnImport = new ButtonPrimary();
            btnPreview = new RoundedButton();
            btnOk = new ButtonPrimary();
            iconPacksBox = new RoundedComboBox();
            btnCreateIconPack = new PictureButton();
            btnDeleteIconPack = new PictureButton();
            btnImportIconPack = new ButtonPrimary();
            btnExportIconPack = new ButtonPrimary();
            btnDeleteIcon = new ButtonPrimary();
            lblSizeLabel = new Label();
            lblSize = new Label();
            lblType = new Label();
            lblTypeLabel = new Label();
            btnCreateIcon = new ButtonPrimary();
            panelCreateIcon = new FlowLayoutPanel();
            lblManaged = new Label();
            btnGenerateStatic = new ButtonPrimary();
            ((ISupportInitialize)btnPreview).BeginInit();
            ((ISupportInitialize)btnCreateIconPack).BeginInit();
            ((ISupportInitialize)btnDeleteIconPack).BeginInit();
            panelCreateIcon.SuspendLayout();
            SuspendLayout();
            // 
            // iconList
            // 
            iconList.AutoScroll = true;
            iconList.AutoScrollMinSize = new Size(0, 6);
            iconList.BackColor = Color.FromArgb(45, 45, 45);
            iconList.Location = new Point(18, 102);
            iconList.Margin = new Padding(4, 4, 4, 4);
            iconList.Name = "iconList";
            iconList.Radius = 0;
            iconList.SelectedIcon = null;
            iconList.Size = new Size(1140, 554);
            iconList.TabIndex = 0;
            // 
            // btnImport
            // 
            btnImport.BorderRadius = 8;
            btnImport.Cursor = Cursors.Hand;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Font = new Font("Tahoma", 9.75F);
            btnImport.ForeColor = Color.White;
            btnImport.HoverColor = Color.FromArgb(0, 89, 184);
            btnImport.Icon = null;
            btnImport.Location = new Point(4, 4);
            btnImport.Margin = new Padding(4, 4, 4, 4);
            btnImport.Name = "btnImport";
            btnImport.Progress = 0;
            btnImport.ProgressColor = Color.FromArgb(0, 46, 94);
            btnImport.Size = new Size(172, 56);
            btnImport.TabIndex = 1;
            btnImport.Text = "Import icon";
            btnImport.UseMnemonic = false;
            btnImport.UseVisualStyleBackColor = true;
            btnImport.UseWindowsAccentColor = true;
            btnImport.WriteProgress = true;
            btnImport.Click += BtnImport_Click;
            // 
            // btnPreview
            // 
            btnPreview.BackColor = Color.FromArgb(35, 35, 35);
            btnPreview.BackgroundImageLayout = ImageLayout.Stretch;
            btnPreview.Column = 0;
            btnPreview.ForegroundImage = null;
            btnPreview.Location = new Point(18, 664);
            btnPreview.Margin = new Padding(4, 4, 4, 4);
            btnPreview.Name = "btnPreview";
            btnPreview.Radius = 40;
            btnPreview.Row = 0;
            btnPreview.ShowGIFIndicator = false;
            btnPreview.ShowKeyboardHotkeyIndicator = false;
            btnPreview.Size = new Size(225, 225);
            btnPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            btnPreview.TabIndex = 5;
            btnPreview.TabStop = false;
            // 
            // btnOk
            // 
            btnOk.BorderRadius = 8;
            btnOk.Cursor = Cursors.Hand;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Tahoma", 9.75F);
            btnOk.ForeColor = Color.White;
            btnOk.HoverColor = Color.FromArgb(0, 89, 184);
            btnOk.Icon = null;
            btnOk.Location = new Point(1046, 846);
            btnOk.Margin = new Padding(4, 4, 4, 4);
            btnOk.Name = "btnOk";
            btnOk.Progress = 0;
            btnOk.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOk.Size = new Size(112, 56);
            btnOk.TabIndex = 6;
            btnOk.Text = "Ok";
            btnOk.UseMnemonic = false;
            btnOk.UseVisualStyleBackColor = true;
            btnOk.UseWindowsAccentColor = true;
            btnOk.WriteProgress = true;
            btnOk.Click += BtnOk_Click;
            // 
            // iconPacksBox
            // 
            iconPacksBox.BackColor = Color.FromArgb(65, 65, 65);
            iconPacksBox.Cursor = Cursors.Hand;
            iconPacksBox.DropDownStyle = ComboBoxStyle.DropDownList;
            iconPacksBox.Font = new Font("Tahoma", 9F);
            iconPacksBox.ForeColor = Color.White;
            iconPacksBox.Icon = null;
            iconPacksBox.Location = new Point(18, 29);
            iconPacksBox.Margin = new Padding(0, 0, 0, 0);
            iconPacksBox.Name = "iconPacksBox";
            iconPacksBox.SelectedIndex = -1;
            iconPacksBox.SelectedItem = null;
            iconPacksBox.Size = new Size(480, 36);
            iconPacksBox.TabIndex = 7;
            iconPacksBox.SelectedIndexChanged += IconPacksBox_SelectedIndexChanged;
            // 
            // btnCreateIconPack
            // 
            btnCreateIconPack.BackColor = Color.Transparent;
            btnCreateIconPack.BackgroundImage = Properties.Resources.Create_Normal;
            btnCreateIconPack.BackgroundImageLayout = ImageLayout.Stretch;
            btnCreateIconPack.Cursor = Cursors.Hand;
            btnCreateIconPack.Font = new Font("Tahoma", 9.75F);
            btnCreateIconPack.ForeColor = Color.White;
            btnCreateIconPack.HoverImage = Properties.Resources.Create_Hover;
            btnCreateIconPack.Location = new Point(508, 31);
            btnCreateIconPack.Margin = new Padding(4, 4, 4, 4);
            btnCreateIconPack.Name = "btnCreateIconPack";
            btnCreateIconPack.Size = new Size(38, 38);
            btnCreateIconPack.TabIndex = 9;
            btnCreateIconPack.TabStop = false;
            btnCreateIconPack.Click += BtnCreateIconPack_Click;
            // 
            // btnDeleteIconPack
            // 
            btnDeleteIconPack.BackColor = Color.Transparent;
            btnDeleteIconPack.BackgroundImage = Properties.Resources.Delete_Normal;
            btnDeleteIconPack.BackgroundImageLayout = ImageLayout.Stretch;
            btnDeleteIconPack.Cursor = Cursors.Hand;
            btnDeleteIconPack.Font = new Font("Tahoma", 9.75F);
            btnDeleteIconPack.ForeColor = Color.White;
            btnDeleteIconPack.HoverImage = Properties.Resources.Delete_Hover;
            btnDeleteIconPack.Location = new Point(559, 31);
            btnDeleteIconPack.Margin = new Padding(4, 4, 4, 4);
            btnDeleteIconPack.Name = "btnDeleteIconPack";
            btnDeleteIconPack.Size = new Size(38, 38);
            btnDeleteIconPack.TabIndex = 10;
            btnDeleteIconPack.TabStop = false;
            btnDeleteIconPack.Click += BtnDeleteIconPack_Click;
            // 
            // btnImportIconPack
            // 
            btnImportIconPack.BorderRadius = 8;
            btnImportIconPack.Cursor = Cursors.Hand;
            btnImportIconPack.FlatAppearance.BorderSize = 0;
            btnImportIconPack.FlatStyle = FlatStyle.Flat;
            btnImportIconPack.Font = new Font("Tahoma", 9.75F);
            btnImportIconPack.ForeColor = Color.White;
            btnImportIconPack.HoverColor = Color.FromArgb(0, 89, 184);
            btnImportIconPack.Icon = null;
            btnImportIconPack.Location = new Point(626, 15);
            btnImportIconPack.Margin = new Padding(4, 4, 4, 4);
            btnImportIconPack.Name = "btnImportIconPack";
            btnImportIconPack.Progress = 0;
            btnImportIconPack.ProgressColor = Color.FromArgb(0, 46, 94);
            btnImportIconPack.Size = new Size(195, 56);
            btnImportIconPack.TabIndex = 11;
            btnImportIconPack.Text = "Import icon pack...";
            btnImportIconPack.UseMnemonic = false;
            btnImportIconPack.UseVisualStyleBackColor = false;
            btnImportIconPack.UseWindowsAccentColor = true;
            btnImportIconPack.WriteProgress = true;
            btnImportIconPack.Click += BtnImportIconPack_Click;
            // 
            // btnExportIconPack
            // 
            btnExportIconPack.BorderRadius = 8;
            btnExportIconPack.Cursor = Cursors.Hand;
            btnExportIconPack.FlatAppearance.BorderSize = 0;
            btnExportIconPack.FlatStyle = FlatStyle.Flat;
            btnExportIconPack.Font = new Font("Tahoma", 9.75F);
            btnExportIconPack.ForeColor = Color.White;
            btnExportIconPack.HoverColor = Color.FromArgb(0, 89, 184);
            btnExportIconPack.Icon = null;
            btnExportIconPack.Location = new Point(830, 16);
            btnExportIconPack.Margin = new Padding(4, 4, 4, 4);
            btnExportIconPack.Name = "btnExportIconPack";
            btnExportIconPack.Progress = 0;
            btnExportIconPack.ProgressColor = Color.FromArgb(0, 46, 94);
            btnExportIconPack.Size = new Size(195, 56);
            btnExportIconPack.TabIndex = 12;
            btnExportIconPack.Text = "Export icon pack...";
            btnExportIconPack.UseMnemonic = false;
            btnExportIconPack.UseVisualStyleBackColor = false;
            btnExportIconPack.UseWindowsAccentColor = true;
            btnExportIconPack.Visible = false;
            btnExportIconPack.WriteProgress = true;
            btnExportIconPack.Click += BtnExportIconPack_Click;
            // 
            // btnDeleteIcon
            // 
            btnDeleteIcon.BorderRadius = 8;
            btnDeleteIcon.Cursor = Cursors.Hand;
            btnDeleteIcon.FlatStyle = FlatStyle.Flat;
            btnDeleteIcon.Font = new Font("Tahoma", 9.75F);
            btnDeleteIcon.ForeColor = Color.White;
            btnDeleteIcon.HoverColor = Color.FromArgb(130, 0, 0);
            btnDeleteIcon.Icon = null;
            btnDeleteIcon.Location = new Point(1000, 664);
            btnDeleteIcon.Margin = new Padding(4, 4, 4, 4);
            btnDeleteIcon.Name = "btnDeleteIcon";
            btnDeleteIcon.Progress = 0;
            btnDeleteIcon.ProgressColor = Color.FromArgb(0, 46, 94);
            btnDeleteIcon.Size = new Size(158, 56);
            btnDeleteIcon.TabIndex = 13;
            btnDeleteIcon.Text = "Delete icon";
            btnDeleteIcon.UseMnemonic = false;
            btnDeleteIcon.UseVisualStyleBackColor = false;
            btnDeleteIcon.UseWindowsAccentColor = false;
            btnDeleteIcon.WriteProgress = true;
            btnDeleteIcon.Click += BtnDeleteIcon_Click;
            // 
            // lblSizeLabel
            // 
            lblSizeLabel.AutoSize = true;
            lblSizeLabel.Location = new Point(252, 748);
            lblSizeLabel.Margin = new Padding(4, 0, 4, 0);
            lblSizeLabel.Name = "lblSizeLabel";
            lblSizeLabel.Size = new Size(53, 24);
            lblSizeLabel.TabIndex = 14;
            lblSizeLabel.Text = "Size:";
            lblSizeLabel.UseMnemonic = false;
            // 
            // lblSize
            // 
            lblSize.Location = new Point(321, 751);
            lblSize.Margin = new Padding(4, 0, 4, 0);
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(352, 22);
            lblSize.TabIndex = 15;
            lblSize.TextAlign = ContentAlignment.MiddleLeft;
            lblSize.UseMnemonic = false;
            // 
            // lblType
            // 
            lblType.Location = new Point(321, 813);
            lblType.Margin = new Padding(4, 0, 4, 0);
            lblType.Name = "lblType";
            lblType.Size = new Size(69, 24);
            lblType.TabIndex = 17;
            lblType.TextAlign = ContentAlignment.MiddleLeft;
            lblType.UseMnemonic = false;
            // 
            // lblTypeLabel
            // 
            lblTypeLabel.AutoSize = true;
            lblTypeLabel.Location = new Point(252, 808);
            lblTypeLabel.Margin = new Padding(4, 0, 4, 0);
            lblTypeLabel.Name = "lblTypeLabel";
            lblTypeLabel.Size = new Size(61, 24);
            lblTypeLabel.TabIndex = 16;
            lblTypeLabel.Text = "Type:";
            lblTypeLabel.UseMnemonic = false;
            // 
            // btnCreateIcon
            // 
            btnCreateIcon.BorderRadius = 8;
            btnCreateIcon.Cursor = Cursors.Hand;
            btnCreateIcon.FlatStyle = FlatStyle.Flat;
            btnCreateIcon.Font = new Font("Tahoma", 9.75F);
            btnCreateIcon.ForeColor = Color.White;
            btnCreateIcon.HoverColor = Color.FromArgb(0, 89, 184);
            btnCreateIcon.Icon = null;
            btnCreateIcon.Location = new Point(184, 4);
            btnCreateIcon.Margin = new Padding(4, 4, 4, 4);
            btnCreateIcon.Name = "btnCreateIcon";
            btnCreateIcon.Progress = 0;
            btnCreateIcon.ProgressColor = Color.FromArgb(0, 46, 94);
            btnCreateIcon.Size = new Size(172, 56);
            btnCreateIcon.TabIndex = 18;
            btnCreateIcon.Text = "Create icon";
            btnCreateIcon.UseMnemonic = false;
            btnCreateIcon.UseVisualStyleBackColor = true;
            btnCreateIcon.UseWindowsAccentColor = true;
            btnCreateIcon.WriteProgress = true;
            btnCreateIcon.Click += BtnCreateIcon_Click;
            // 
            // panelCreateIcon
            // 
            panelCreateIcon.Controls.Add(btnImport);
            panelCreateIcon.Controls.Add(btnCreateIcon);
            panelCreateIcon.Controls.Add(lblManaged);
            panelCreateIcon.Location = new Point(252, 664);
            panelCreateIcon.Margin = new Padding(4, 4, 4, 4);
            panelCreateIcon.Name = "panelCreateIcon";
            panelCreateIcon.Size = new Size(740, 57);
            panelCreateIcon.TabIndex = 21;
            // 
            // lblManaged
            // 
            lblManaged.AutoSize = true;
            lblManaged.Location = new Point(4, 64);
            lblManaged.Margin = new Padding(4, 0, 4, 0);
            lblManaged.Name = "lblManaged";
            lblManaged.Size = new Size(452, 24);
            lblManaged.TabIndex = 21;
            lblManaged.Text = "This icon pack is managed by the plugin manager";
            lblManaged.UseMnemonic = false;
            lblManaged.Visible = false;
            // 
            // btnGenerateStatic
            // 
            btnGenerateStatic.BorderRadius = 8;
            btnGenerateStatic.Cursor = Cursors.Hand;
            btnGenerateStatic.FlatStyle = FlatStyle.Flat;
            btnGenerateStatic.Font = new Font("Tahoma", 9.75F);
            btnGenerateStatic.ForeColor = Color.White;
            btnGenerateStatic.HoverColor = Color.FromArgb(0, 89, 184);
            btnGenerateStatic.Icon = null;
            btnGenerateStatic.Location = new Point(399, 794);
            btnGenerateStatic.Margin = new Padding(4, 4, 4, 4);
            btnGenerateStatic.Name = "btnGenerateStatic";
            btnGenerateStatic.Progress = 0;
            btnGenerateStatic.ProgressColor = Color.FromArgb(0, 46, 94);
            btnGenerateStatic.Size = new Size(218, 56);
            btnGenerateStatic.TabIndex = 22;
            btnGenerateStatic.Text = "Generate static";
            btnGenerateStatic.UseMnemonic = false;
            btnGenerateStatic.UseVisualStyleBackColor = true;
            btnGenerateStatic.UseWindowsAccentColor = true;
            btnGenerateStatic.Visible = false;
            btnGenerateStatic.WriteProgress = true;
            btnGenerateStatic.Click += BtnGenerateStatic_Click;
            // 
            // IconSelector
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            ClientSize = new Size(1180, 909);
            Controls.Add(panelCreateIcon);
            Controls.Add(lblType);
            Controls.Add(lblTypeLabel);
            Controls.Add(btnGenerateStatic);
            Controls.Add(lblSize);
            Controls.Add(lblSizeLabel);
            Controls.Add(btnDeleteIcon);
            Controls.Add(btnExportIconPack);
            Controls.Add(btnImportIconPack);
            Controls.Add(btnDeleteIconPack);
            Controls.Add(btnCreateIconPack);
            Controls.Add(iconPacksBox);
            Controls.Add(btnOk);
            Controls.Add(btnPreview);
            Controls.Add(iconList);
            Margin = new Padding(6, 9, 6, 9);
            Name = "IconSelector";
            Padding = new Padding(3, 3, 3, 3);
            Text = "Macro Deck :: Icon selector";
            Shown += IconSelector_Shown;
            ((ISupportInitialize)btnPreview).EndInit();
            ((ISupportInitialize)btnCreateIconPack).EndInit();
            ((ISupportInitialize)btnDeleteIconPack).EndInit();
            panelCreateIcon.ResumeLayout(false);
            panelCreateIcon.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private IconGrid iconList;
        private ButtonPrimary btnImport;
        private RoundedButton btnPreview;
        private ButtonPrimary btnOk;
        private RoundedComboBox iconPacksBox;
        private PictureButton btnCreateIconPack;
        private PictureButton btnDeleteIconPack;

        private ButtonPrimary btnImportIconPack;
        private ButtonPrimary btnExportIconPack;
        private ButtonPrimary btnDeleteIcon;
        private Label lblSizeLabel;
        private Label lblSize;
        private Label lblType;
        private Label lblTypeLabel;
        private ButtonPrimary btnCreateIcon;
        private FlowLayoutPanel panelCreateIcon;
        private Label lblManaged;
        private ButtonPrimary btnGenerateStatic;
    }
}