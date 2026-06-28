
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.CustomControls.ExtensionsView
{
    partial class InstalledExtensionsView
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
            ExtensionStore.ExtensionStoreHelper.OnUpdateCheckFinished -= UpdateCheckFinished;
            ExtensionStore.ExtensionStoreHelper.OnInstallationFinished -= ExtensionStoreHelper_OnInstallationFinished;

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
            txtSearch = new RoundedTextBox();
            installedExtensionsGrid = new ExtensionGrid();
            btnCheckUpdates = new ButtonPrimary();
            lblUpdateState = new Label();
            btnAddViaZip = new ButtonPrimary();
            SuspendLayout();
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.BackColor = Color.FromArgb(65, 65, 65);
            txtSearch.Font = new Font("Tahoma", 9F);
            txtSearch.ForeColor = Color.White;
            txtSearch.Icon = null;
            txtSearch.Location = new Point(0, 10);
            txtSearch.Margin = new Padding(0, 0, 0, 0);
            txtSearch.MaxCharacters = 32767;
            txtSearch.Multiline = false;
            txtSearch.Name = "txtSearch";
            txtSearch.PasswordChar = false;
            txtSearch.PlaceHolderColor = Color.Gray;
            txtSearch.PlaceHolderText = "Search installed extensions…";
            txtSearch.ReadOnly = false;
            txtSearch.ScrollBars = ScrollBars.None;
            txtSearch.SelectionStart = 0;
            txtSearch.Size = new Size(1130, 23);
            txtSearch.TabIndex = 0;
            txtSearch.TextAlignment = HorizontalAlignment.Left;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            // 
            // installedExtensionsGrid
            // 
            installedExtensionsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            installedExtensionsGrid.AutoScroll = true;
            installedExtensionsGrid.AutoScrollMinSize = new Size(0, 24);
            installedExtensionsGrid.BackColor = Color.FromArgb(45, 45, 45);
            installedExtensionsGrid.Location = new Point(0, 49);
            installedExtensionsGrid.Name = "installedExtensionsGrid";
            installedExtensionsGrid.Size = new Size(1137, 360);
            installedExtensionsGrid.TabIndex = 1;
            // 
            // btnCheckUpdates
            // 
            btnCheckUpdates.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCheckUpdates.BorderRadius = 8;
            btnCheckUpdates.Cursor = Cursors.Hand;
            btnCheckUpdates.FlatAppearance.BorderSize = 0;
            btnCheckUpdates.FlatStyle = FlatStyle.Flat;
            btnCheckUpdates.Font = new Font("Tahoma", 9.75F);
            btnCheckUpdates.ForeColor = Color.White;
            btnCheckUpdates.HoverColor = Color.Empty;
            btnCheckUpdates.Icon = null;
            btnCheckUpdates.Location = new Point(230, 437);
            btnCheckUpdates.Name = "btnCheckUpdates";
            btnCheckUpdates.Progress = 0;
            btnCheckUpdates.ProgressColor = Color.FromArgb(0, 103, 225);
            btnCheckUpdates.Size = new Size(191, 37);
            btnCheckUpdates.TabIndex = 3;
            btnCheckUpdates.Text = "Check updates";
            btnCheckUpdates.UseMnemonic = false;
            btnCheckUpdates.UseVisualStyleBackColor = true;
            btnCheckUpdates.UseWindowsAccentColor = true;
            btnCheckUpdates.WriteProgress = true;
            btnCheckUpdates.Click += BtnCheckUpdates_Click;
            // 
            // lblUpdateState
            // 
            lblUpdateState.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblUpdateState.Font = new Font("Tahoma", 9.75F);
            lblUpdateState.ForeColor = Color.Silver;
            lblUpdateState.Location = new Point(4, 442);
            lblUpdateState.Name = "lblUpdateState";
            lblUpdateState.Size = new Size(220, 30);
            lblUpdateState.TabIndex = 4;
            lblUpdateState.Text = "All extensions are up-to-date";
            lblUpdateState.TextAlign = ContentAlignment.MiddleLeft;
            lblUpdateState.UseMnemonic = false;
            // 
            // btnAddViaZip
            // 
            btnAddViaZip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAddViaZip.BorderRadius = 8;
            btnAddViaZip.Cursor = Cursors.Hand;
            btnAddViaZip.FlatAppearance.BorderSize = 0;
            btnAddViaZip.FlatStyle = FlatStyle.Flat;
            btnAddViaZip.Font = new Font("Tahoma", 9.75F);
            btnAddViaZip.ForeColor = Color.White;
            btnAddViaZip.HoverColor = Color.Empty;
            btnAddViaZip.Icon = null;
            btnAddViaZip.Location = new Point(926, 438);
            btnAddViaZip.Name = "btnAddViaZip";
            btnAddViaZip.Progress = 0;
            btnAddViaZip.ProgressColor = Color.FromArgb(0, 103, 225);
            btnAddViaZip.Size = new Size(192, 37);
            btnAddViaZip.TabIndex = 5;
            btnAddViaZip.Text = "Install from file";
            btnAddViaZip.UseVisualStyleBackColor = true;
            btnAddViaZip.UseWindowsAccentColor = true;
            btnAddViaZip.WriteProgress = true;
            btnAddViaZip.Click += BtnAddViaZip_Click;
            // 
            // InstalledExtensionsView
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(btnAddViaZip);
            Controls.Add(lblUpdateState);
            Controls.Add(btnCheckUpdates);
            Controls.Add(installedExtensionsGrid);
            Controls.Add(txtSearch);
            Font = new Font("Tahoma", 9F);
            ForeColor = Color.White;
            Name = "InstalledExtensionsView";
            Size = new Size(1137, 495);
            Load += InstalledExtensionsView_Load;
            ResumeLayout(false);

        }

        #endregion
        private RoundedTextBox txtSearch;
        private ExtensionGrid installedExtensionsGrid;
        private ButtonPrimary btnCheckUpdates;
        private Label lblUpdateState;
        private ButtonPrimary btnAddViaZip;
    }
}
