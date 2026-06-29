
using System;
using System.ComponentModel;
#pragma warning disable CS0618 // Designer 中使用旧版 ComboBox，待设计器迁移
using System.Drawing;
using System.Windows.Forms;
using ComboBox = SuchByte.MacroDeck.GUI.CustomControls.ComboBox;

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
            adapter = new ComboBox();
            lblNetworkAdapter = new Label();
            iPAddress = new Label();
            lblIpAddress = new Label();
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
            // adapter
            // 
            adapter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            adapter.BackColor = Color.FromArgb(55, 55, 55);
            adapter.BorderRadius = 8;
            adapter.Cursor = Cursors.Hand;
            adapter.DropDownStyle = ComboBoxStyle.DropDownList;
            adapter.Font = new Font("Tahoma", 14.25F);
            adapter.ForeColor = Color.White;
            adapter.FormattingEnabled = true;
            adapter.Location = new Point(227, 102);
            adapter.Name = "adapter";
            adapter.Size = new Size(455, 27);
            adapter.TabIndex = 4;
            adapter.SelectedIndexChanged += Adapter_SelectedIndexChanged;
            // 
            // lblNetworkAdapter
            // 
            lblNetworkAdapter.AutoSize = true;
            lblNetworkAdapter.Font = new Font("Tahoma", 12F);
            lblNetworkAdapter.ForeColor = Color.White;
            lblNetworkAdapter.Location = new Point(3, 100);
            lblNetworkAdapter.Name = "lblNetworkAdapter";
            lblNetworkAdapter.Size = new Size(200, 29);
            lblNetworkAdapter.TabIndex = 5;
            lblNetworkAdapter.Text = "Network adapter:";
            lblNetworkAdapter.UseMnemonic = false;
            // 
            // iPAddress
            // 
            iPAddress.AutoSize = true;
            iPAddress.Font = new Font("Tahoma", 12F);
            iPAddress.ForeColor = Color.White;
            iPAddress.Location = new Point(227, 152);
            iPAddress.Name = "iPAddress";
            iPAddress.Size = new Size(0, 29);
            iPAddress.TabIndex = 6;
            iPAddress.UseMnemonic = false;
            iPAddress.Click += lblIpAddress_Click;
            // 
            // lblIpAddress
            // 
            lblIpAddress.AutoSize = true;
            lblIpAddress.Font = new Font("Tahoma", 12F);
            lblIpAddress.ForeColor = Color.White;
            lblIpAddress.Location = new Point(3, 152);
            lblIpAddress.Name = "lblIpAddress";
            lblIpAddress.Size = new Size(134, 29);
            lblIpAddress.TabIndex = 7;
            lblIpAddress.Text = "IP address:";
            lblIpAddress.UseMnemonic = false;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Tahoma", 12F);
            lblPort.ForeColor = Color.White;
            lblPort.Location = new Point(3, 229);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(64, 29);
            lblPort.TabIndex = 8;
            lblPort.Text = "Port:";
            lblPort.UseMnemonic = false;
            // 
            // port
            // 
            port.BackColor = Color.FromArgb(55, 55, 55);
            port.Font = new Font("Tahoma", 14.25F);
            port.ForeColor = Color.White;
            port.Location = new Point(227, 222);
            port.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            port.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            port.Name = "port";
            port.Size = new Size(120, 42);
            port.TabIndex = 9;
            port.Value = new decimal(new int[] { 8191, 0, 0, 0 });
            port.ValueChanged += port_ValueChanged;
            // 
            // groupInfo
            // 
            groupInfo.Controls.Add(lblInfo);
            groupInfo.Font = new Font("Tahoma", 14.25F);
            groupInfo.ForeColor = Color.White;
            groupInfo.Location = new Point(3, 311);
            groupInfo.Name = "groupInfo";
            groupInfo.Size = new Size(685, 244);
            groupInfo.TabIndex = 10;
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
            Controls.Add(lblIpAddress);
            Controls.Add(iPAddress);
            Controls.Add(lblNetworkAdapter);
            Controls.Add(adapter);
            Controls.Add(lblConfigureNetwork);
            Name = "SetupPage2";
            Size = new Size(691, 571);
            Load += SetupPage2_Load;
            ((ISupportInitialize)port).EndInit();
            groupInfo.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label lblConfigureNetwork;
        private ComboBox adapter;
        private Label lblNetworkAdapter;
        private Label iPAddress;
        private Label lblIpAddress;
        private Label lblPort;
        private NumericUpDown port;
        private GroupBox groupInfo;
        private Label lblInfo;
    }
}
