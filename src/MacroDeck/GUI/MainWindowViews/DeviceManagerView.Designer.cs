using System.ComponentModel;
using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Server;

namespace SuchByte.MacroDeck.GUI.MainWindowViews
{
    partial class DeviceManagerView
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
            MacroDeckServer.OnDeviceConnectionStateChanged -= this.OnClientsChanged;
            DeviceManager.OnDevicesChange -= this.OnClientsChanged;
            
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
            devicesList = new FlowLayoutPanel();
            lblKnownDevices = new Label();
            lblBehaviour = new Label();
            panel1 = new Panel();
            radioBlockNew = new RadioButton();
            radioAllowAll = new RadioButton();
            radioAskNewConnections = new RadioButton();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // devicesList
            // 
            devicesList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            devicesList.AutoScroll = true;
            devicesList.Location = new Point(3, 65);
            devicesList.Name = "devicesList";
            devicesList.Size = new Size(1025, 936);
            devicesList.TabIndex = 12;
            // 
            // lblKnownDevices
            // 
            lblKnownDevices.AutoSize = true;
            lblKnownDevices.Font = new Font("Tahoma", 14.25F);
            lblKnownDevices.ForeColor = Color.White;
            lblKnownDevices.Location = new Point(9, 14);
            lblKnownDevices.Name = "lblKnownDevices";
            lblKnownDevices.Size = new Size(204, 35);
            lblKnownDevices.TabIndex = 13;
            lblKnownDevices.Text = "Known devices";
            lblKnownDevices.UseMnemonic = false;
            // 
            // lblBehaviour
            // 
            lblBehaviour.AutoSize = true;
            lblBehaviour.Font = new Font("Tahoma", 14.25F);
            lblBehaviour.ForeColor = Color.White;
            lblBehaviour.Location = new Point(1058, 14);
            lblBehaviour.Name = "lblBehaviour";
            lblBehaviour.Size = new Size(141, 35);
            lblBehaviour.TabIndex = 14;
            lblBehaviour.Text = "Behaviour";
            lblBehaviour.UseMnemonic = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(radioBlockNew);
            panel1.Controls.Add(radioAllowAll);
            panel1.Controls.Add(radioAskNewConnections);
            panel1.Location = new Point(1052, 65);
            panel1.Name = "panel1";
            panel1.Size = new Size(797, 135);
            panel1.TabIndex = 15;
            // 
            // radioBlockNew
            // 
            radioBlockNew.AutoSize = true;
            radioBlockNew.Font = new Font("Tahoma", 12F);
            radioBlockNew.ForeColor = Color.FromArgb(255, 128, 128);
            radioBlockNew.Location = new Point(27, 91);
            radioBlockNew.Name = "radioBlockNew";
            radioBlockNew.Size = new Size(308, 33);
            radioBlockNew.TabIndex = 2;
            radioBlockNew.TabStop = true;
            radioBlockNew.Text = "Block all new connections";
            radioBlockNew.UseMnemonic = false;
            radioBlockNew.UseVisualStyleBackColor = true;
            radioBlockNew.CheckedChanged += RadioBehaviour_CheckedChanged;
            // 
            // radioAllowAll
            // 
            radioAllowAll.AutoSize = true;
            radioAllowAll.Font = new Font("Tahoma", 12F);
            radioAllowAll.ForeColor = Color.White;
            radioAllowAll.Location = new Point(27, 49);
            radioAllowAll.Name = "radioAllowAll";
            radioAllowAll.Size = new Size(530, 33);
            radioAllowAll.TabIndex = 1;
            radioAllowAll.TabStop = true;
            radioAllowAll.Text = "Allow all new connections (Not recommended)";
            radioAllowAll.UseMnemonic = false;
            radioAllowAll.UseVisualStyleBackColor = true;
            radioAllowAll.CheckedChanged += RadioBehaviour_CheckedChanged;
            // 
            // radioAskNewConnections
            // 
            radioAskNewConnections.AutoSize = true;
            radioAskNewConnections.Font = new Font("Tahoma", 12F);
            radioAskNewConnections.ForeColor = Color.White;
            radioAskNewConnections.Location = new Point(27, 10);
            radioAskNewConnections.Name = "radioAskNewConnections";
            radioAskNewConnections.Size = new Size(293, 33);
            radioAskNewConnections.TabIndex = 0;
            radioAskNewConnections.TabStop = true;
            radioAskNewConnections.Text = "Ask on new connections";
            radioAskNewConnections.UseMnemonic = false;
            radioAskNewConnections.UseVisualStyleBackColor = true;
            radioAskNewConnections.CheckedChanged += RadioBehaviour_CheckedChanged;
            // 
            // DeviceManagerView
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(panel1);
            Controls.Add(lblBehaviour);
            Controls.Add(lblKnownDevices);
            Controls.Add(devicesList);
            Font = new Font("Tahoma", 9F);
            Name = "DeviceManagerView";
            Size = new Size(1865, 1014);
            Load += DeviceManagerPage_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private FlowLayoutPanel devicesList;
        private Label lblKnownDevices;
        private Label lblBehaviour;
        private Panel panel1;
        private RadioButton radioBlockNew;
        private RadioButton radioAllowAll;
        private RadioButton radioAskNewConnections;
    }
}
