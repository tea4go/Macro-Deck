
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.InitialSetupPages
{
    partial class SetupPage4
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
            lblPickAllPlugins = new Label();
            lblDontWorry = new Label();
            plugins = new FlowLayoutPanel();
            progressBar = new ProgressBar();
            SuspendLayout();
            // 
            // lblPickAllPlugins
            // 
            lblPickAllPlugins.Font = new Font("Tahoma", 24F);
            lblPickAllPlugins.ForeColor = Color.White;
            lblPickAllPlugins.ImageAlign = ContentAlignment.BottomCenter;
            lblPickAllPlugins.Location = new Point(3, 0);
            lblPickAllPlugins.Name = "lblPickAllPlugins";
            lblPickAllPlugins.Size = new Size(685, 81);
            lblPickAllPlugins.TabIndex = 5;
            lblPickAllPlugins.Text = "Pick all the plugins you need";
            lblPickAllPlugins.TextAlign = ContentAlignment.TopCenter;
            lblPickAllPlugins.UseMnemonic = false;
            // 
            // lblDontWorry
            // 
            lblDontWorry.Font = new Font("Tahoma", 12F);
            lblDontWorry.ForeColor = Color.White;
            lblDontWorry.ImageAlign = ContentAlignment.BottomCenter;
            lblDontWorry.Location = new Point(3, 81);
            lblDontWorry.Name = "lblDontWorry";
            lblDontWorry.Size = new Size(685, 79);
            lblDontWorry.TabIndex = 6;
            lblDontWorry.Text = "Don't worry, you can always install/uninstall plugins later in the package manager";
            lblDontWorry.TextAlign = ContentAlignment.TopCenter;
            lblDontWorry.UseMnemonic = false;
            // 
            // plugins
            // 
            plugins.AutoScroll = true;
            plugins.Location = new Point(120, 172);
            plugins.Name = "plugins";
            plugins.Size = new Size(450, 369);
            plugins.TabIndex = 7;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(208, 547);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(275, 31);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 8;
            progressBar.Visible = false;
            // 
            // SetupPage4
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(progressBar);
            Controls.Add(plugins);
            Controls.Add(lblDontWorry);
            Controls.Add(lblPickAllPlugins);
            Name = "SetupPage4";
            Size = new Size(691, 609);
            Load += SetupPage4_Load;
            ResumeLayout(false);

        }

        #endregion

        private Label lblPickAllPlugins;
        private Label lblDontWorry;
        private FlowLayoutPanel plugins;
        private ProgressBar progressBar;
    }
}
