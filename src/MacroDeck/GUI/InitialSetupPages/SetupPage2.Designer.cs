
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.InitialSetupPages
{
    partial class SetupPage2
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SetupPage2));
            lblConfigureNetwork = new Label();
            lblPort = new Label();
            port = new NumericUpDown();
            groupInfo = new GroupBox();
            lblInfo = new Label();
            ((ISupportInitialize)port).BeginInit();
            groupInfo.SuspendLayout();
            SuspendLayout();
            //
            // lblConfigureNetwork
            //
            lblConfigureNetwork.Font = new Font("Tahoma", 24F);
            lblConfigureNetwork.ForeColor = Color.White;
            lblConfigureNetwork.ImageAlign = ContentAlignment.BottomCenter;
            lblConfigureNetwork.Location = new Point(3, 0);
            lblConfigureNetwork.Name = "lblConfigureNetwork";
            lblConfigureNetwork.Size = new Size(685, 73);
            lblConfigureNetwork.TabIndex = 3;
            lblConfigureNetwork.Text = "Configure your network settings\r\n";
            lblConfigureNetwork.TextAlign = ContentAlignment.TopCenter;
            lblConfigureNetwork.UseMnemonic = false;
            //
            // lblPort
            //
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Tahoma", 12F);
            lblPort.ForeColor = Color.White;
            lblPort.Location = new Point(3, 107);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(64, 29);
            lblPort.TabIndex = 4;
            lblPort.Text = "Port:";
            lblPort.UseMnemonic = false;
            //
            // port
            //
            port.BackColor = Color.FromArgb(55, 55, 55);
            port.Font = new Font("Tahoma", 14.25F);
            port.ForeColor = Color.White;
            port.Location = new Point(227, 100);
            port.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            port.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            port.Name = "port";
            port.Size = new Size(120, 42);
            port.TabIndex = 5;
            port.Value = new decimal(new int[] { 8191, 0, 0, 0 });
            port.ValueChanged += port_ValueChanged;
            //
            // groupInfo
            //
            groupInfo.Controls.Add(lblInfo);
            groupInfo.Font = new Font("Tahoma", 14.25F);
            groupInfo.ForeColor = Color.White;
            groupInfo.Location = new Point(3, 180);
            groupInfo.Name = "groupInfo";
            groupInfo.Size = new Size(685, 244);
            groupInfo.TabIndex = 6;
            groupInfo.TabStop = false;
            groupInfo.Text = "Info";
            //
            // lblInfo
            //
            lblInfo.Font = new Font("Tahoma", 11.25F);
            lblInfo.Location = new Point(6, 50);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(673, 167);
            lblInfo.TabIndex = 0;
            lblInfo.Text = resources.GetString("lblInfo.Text");
            lblInfo.UseMnemonic = false;
            //
            // SetupPage2
            //
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(groupInfo);
            Controls.Add(port);
            Controls.Add(lblPort);
            Controls.Add(lblConfigureNetwork);
            Name = "SetupPage2";
            Size = new Size(691, 440);
            ((ISupportInitialize)port).EndInit();
            groupInfo.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label lblConfigureNetwork;
        private Label lblPort;
        private NumericUpDown port;
        private GroupBox groupInfo;
        private Label lblInfo;
    }
}
