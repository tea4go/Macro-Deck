
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.CustomControls.Settings
{
    partial class BackupItem
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
            this.lblFileName = new Label();
            this.lblDateCreated = new Label();
            this.lblSize = new Label();
            this.btnDelete = new ButtonPrimary();
            this.btnRestore = new ButtonPrimary();
            this.btnOpenFolder = new ButtonPrimary();
            this.SuspendLayout();
            //
            // lblFileName
            //
            this.lblFileName.Location = new Point(8, 4);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new Size(484, 42);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "label1";
            this.lblFileName.TextAlign = ContentAlignment.MiddleLeft;
            this.lblFileName.UseMnemonic = false;
            //
            // lblDateCreated
            //
            this.lblDateCreated.Location = new Point(498, 4);
            this.lblDateCreated.Name = "lblDateCreated";
            this.lblDateCreated.Size = new Size(234, 42);
            this.lblDateCreated.TabIndex = 1;
            this.lblDateCreated.Text = "label1";
            this.lblDateCreated.TextAlign = ContentAlignment.MiddleRight;
            this.lblDateCreated.UseMnemonic = false;
            //
            // lblSize
            //
            this.lblSize.Location = new Point(798, 4);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new Size(89, 42);
            this.lblSize.TabIndex = 2;
            this.lblSize.Text = "label1";
            this.lblSize.TextAlign = ContentAlignment.MiddleRight;
            this.lblSize.UseMnemonic = false;
            //
            // btnDelete
            //
            this.btnDelete.BorderRadius = 8;
            this.btnDelete.Cursor = Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.HoverColor = Color.FromArgb(150, 0, 0);
            this.btnDelete.Icon = Resources.Delete_Hover;
            this.btnDelete.Location = new Point(1096, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Progress = 0;
            this.btnDelete.ProgressColor = Color.FromArgb(0, 46, 94);
            this.btnDelete.Size = new Size(35, 35);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.UseMnemonic = false;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.UseWindowsAccentColor = false;
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            //
            // btnOpenFolder
            //
            this.btnOpenFolder.BorderRadius = 8;
            this.btnOpenFolder.Cursor = Cursors.Hand;
            this.btnOpenFolder.FlatAppearance.BorderSize = 0;
            this.btnOpenFolder.FlatStyle = FlatStyle.Flat;
            this.btnOpenFolder.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnOpenFolder.ForeColor = Color.White;
            this.btnOpenFolder.HoverColor = Color.FromArgb(0, 89, 184);
            this.btnOpenFolder.Icon = Resources.Harddisk;
            this.btnOpenFolder.Location = new Point(1010, 8);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Progress = 0;
            this.btnOpenFolder.ProgressColor = Color.FromArgb(0, 46, 94);
            this.btnOpenFolder.Size = new Size(35, 35);
            this.btnOpenFolder.TabIndex = 5;
            this.btnOpenFolder.UseMnemonic = false;
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.UseWindowsAccentColor = false;
            this.btnOpenFolder.Click += new EventHandler(this.BtnOpenFolder_Click);
            //
            // btnRestore
            //
            this.btnRestore.BorderRadius = 8;
            this.btnRestore.Cursor = Cursors.Hand;
            this.btnRestore.FlatAppearance.BorderSize = 0;
            this.btnRestore.FlatStyle = FlatStyle.Flat;
            this.btnRestore.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnRestore.ForeColor = Color.White;
            this.btnRestore.HoverColor = Color.FromArgb(0, 89, 184);
            this.btnRestore.Icon = Resources.Backup_Restore;
            this.btnRestore.Location = new Point(1053, 8);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Progress = 0;
            this.btnRestore.ProgressColor = Color.FromArgb(0, 46, 94);
            this.btnRestore.Size = new Size(35, 35);
            this.btnRestore.TabIndex = 4;
            this.btnRestore.UseMnemonic = false;
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.UseWindowsAccentColor = true;
            this.btnRestore.Click += new EventHandler(this.BtnRestore_Click);
            //
            // BackupItem
            //

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(35, 35, 35);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lblDateCreated);
            this.Controls.Add(this.lblFileName);
            this.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.ForeColor = Color.White;
            this.Name = "BackupItem";
            this.Padding = new Padding(5, 4, 5, 4);
            this.Size = new Size(1136, 50);
            this.Load += new EventHandler(this.BackupItem_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Label lblFileName;
        private Label lblDateCreated;
        private Label lblSize;
        private ButtonPrimary btnDelete;
        private ButtonPrimary btnRestore;
        private ButtonPrimary btnOpenFolder;
    }
}
