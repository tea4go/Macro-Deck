
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.MainWindowViews
{
    partial class ExtensionsView
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
            content = new Panel();
            radioInstalled = new ButtonRadioButton();
            radioOnline = new ButtonRadioButton();
            SuspendLayout();
            // 
            // content
            // 
            content.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            content.Location = new Point(10, 62);
            content.Name = "content";
            content.Size = new Size(1117, 467);
            content.TabIndex = 0;
            // 
            // radioInstalled
            // 
            radioInstalled.BorderRadius = 8;
            radioInstalled.Checked = true;
            radioInstalled.Cursor = Cursors.Hand;
            radioInstalled.Font = new Font("Tahoma", 12F);
            radioInstalled.ForeColor = Color.White;
            radioInstalled.Icon = Resources.Harddisk;
            radioInstalled.IconAlignment = ContentAlignment.MiddleLeft;
            radioInstalled.Location = new Point(3, 3);
            radioInstalled.Name = "radioInstalled";
            radioInstalled.Size = new Size(186, 52);
            radioInstalled.TabIndex = 1;
            radioInstalled.TabStop = true;
            radioInstalled.Text = "Installed";
            radioInstalled.UseVisualStyleBackColor = true;
            radioInstalled.CheckedChanged += RadioInstalled_CheckedChanged;
            // 
            // radioOnline
            // 
            radioOnline.BorderRadius = 8;
            radioOnline.Cursor = Cursors.Hand;
            radioOnline.Font = new Font("Tahoma", 12F);
            radioOnline.ForeColor = Color.White;
            radioOnline.Icon = Resources.Web2;
            radioOnline.IconAlignment = ContentAlignment.MiddleLeft;
            radioOnline.Location = new Point(195, 3);
            radioOnline.Name = "radioOnline";
            radioOnline.Size = new Size(186, 52);
            radioOnline.TabIndex = 2;
            radioOnline.Text = "Online";
            radioOnline.UseVisualStyleBackColor = true;
            radioOnline.CheckedChanged += RadioOnline_CheckedChanged;
            // 
            // ExtensionsView
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(radioOnline);
            Controls.Add(radioInstalled);
            Controls.Add(content);
            Font = new Font("Tahoma", 9F);
            Name = "ExtensionsView";
            Size = new Size(1137, 540);
            Load += ExtensionStoreView_Load;
            ResumeLayout(false);

        }

        #endregion

        private Panel content;
        private ButtonRadioButton radioInstalled;
        private ButtonRadioButton radioOnline;
    }
}
