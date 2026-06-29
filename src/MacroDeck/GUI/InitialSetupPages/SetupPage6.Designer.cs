
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.InitialSetupPages
{
    partial class SetupPage6
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
            lblAlmostDone = new Label();
            checkAutoUpdates = new CheckBox();
            checkAutoStart = new CheckBox();
            SuspendLayout();
            // 
            // lblAlmostDone
            // 
            lblAlmostDone.Font = new Font("Tahoma", 24F);
            lblAlmostDone.ForeColor = Color.White;
            lblAlmostDone.ImageAlign = ContentAlignment.BottomCenter;
            lblAlmostDone.Location = new Point(3, 0);
            lblAlmostDone.Name = "lblAlmostDone";
            lblAlmostDone.Size = new Size(685, 73);
            lblAlmostDone.TabIndex = 7;
            lblAlmostDone.Text = "We're almost done!";
            lblAlmostDone.TextAlign = ContentAlignment.TopCenter;
            lblAlmostDone.UseMnemonic = false;
            // 
            // checkAutoUpdates
            // 
            checkAutoUpdates.AutoSize = true;
            checkAutoUpdates.Checked = true;
            checkAutoUpdates.CheckState = CheckState.Checked;
            checkAutoUpdates.Font = new Font("Tahoma", 12F);
            checkAutoUpdates.ForeColor = Color.White;
            checkAutoUpdates.Location = new Point(16, 100);
            checkAutoUpdates.Name = "checkAutoUpdates";
            checkAutoUpdates.Size = new Size(377, 33);
            checkAutoUpdates.TabIndex = 8;
            checkAutoUpdates.Text = "Automatically check for updates";
            checkAutoUpdates.UseMnemonic = false;
            checkAutoUpdates.UseVisualStyleBackColor = true;
            checkAutoUpdates.CheckedChanged += CheckAutoUpdates_CheckedChanged;
            // 
            // checkAutoStart
            // 
            checkAutoStart.AutoSize = true;
            checkAutoStart.Checked = true;
            checkAutoStart.CheckState = CheckState.Checked;
            checkAutoStart.Font = new Font("Tahoma", 12F);
            checkAutoStart.ForeColor = Color.White;
            checkAutoStart.Location = new Point(16, 145);
            checkAutoStart.Name = "checkAutoStart";
            checkAutoStart.Size = new Size(391, 33);
            checkAutoStart.TabIndex = 9;
            checkAutoStart.Text = "Automatically start with Windows\r\n";
            checkAutoStart.UseMnemonic = false;
            checkAutoStart.UseVisualStyleBackColor = true;
            checkAutoStart.CheckedChanged += CheckAutoStart_CheckedChanged;
            // 
            // SetupPage6
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(checkAutoStart);
            Controls.Add(checkAutoUpdates);
            Controls.Add(lblAlmostDone);
            Name = "SetupPage6";
            Size = new Size(691, 571);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label lblAlmostDone;
        private CheckBox checkAutoUpdates;
        private CheckBox checkAutoStart;
    }
}
