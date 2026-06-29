
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.CustomControls
{
    partial class ActionConfiguratorActionItem
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
            lblActionName = new Label();
            SuspendLayout();
            // 
            // lblActionName
            // 
            lblActionName.Cursor = Cursors.Hand;
            lblActionName.Location = new Point(1, 0);
            lblActionName.Name = "lblActionName";
            lblActionName.Size = new Size(265, 48);
            lblActionName.TabIndex = 0;
            lblActionName.Text = "label1";
            lblActionName.TextAlign = ContentAlignment.MiddleLeft;
            lblActionName.UseMnemonic = false;
            // 
            // ActionConfiguratorActionItem
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(35, 35, 35);
            Controls.Add(lblActionName);
            Cursor = Cursors.Hand;
            Font = new Font("Tahoma", 9F);
            ForeColor = Color.White;
            Margin = new Padding(6, 0, 6, 1);
            Name = "ActionConfiguratorActionItem";
            Size = new Size(268, 50);
            Load += ActionConfiguratorActionItem_Load;
            ResumeLayout(false);

        }

        #endregion

        private Label lblActionName;
    }
}
