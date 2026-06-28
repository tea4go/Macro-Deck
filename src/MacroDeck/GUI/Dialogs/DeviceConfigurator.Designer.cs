
using System.ComponentModel;
using System.Windows.Forms;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class DeviceConfigurator
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
            btnOk = new ButtonPrimary();
            lblBrightness = new Label();
            brightness = new TrackBar();
            checkAutoConnect = new CheckBox();
            lblKeepWake = new Label();
            radioKeepAwakeNever = new TabRadioButton();
            flowLayoutPanel1 = new FlowLayoutPanel();
            radioKeepAwakeConnected = new TabRadioButton();
            radioKeepAwakeAlways = new TabRadioButton();
            ((ISupportInitialize)brightness).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOk
            // 
            btnOk.BorderRadius = 8;
            btnOk.Cursor = Cursors.Hand;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Tahoma", 9.75F);
            btnOk.ForeColor = Color.White;
            btnOk.HoverColor = Color.FromArgb(0, 89, 184);
            btnOk.Icon = null;
            btnOk.Location = new Point(425, 164);
            btnOk.Name = "btnOk";
            btnOk.Progress = 0;
            btnOk.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOk.Size = new Size(133, 49);
            btnOk.TabIndex = 2;
            btnOk.Text = "Ok";
            btnOk.UseMnemonic = false;
            btnOk.UseVisualStyleBackColor = false;
            btnOk.UseWindowsAccentColor = true;
            btnOk.WriteProgress = true;
            btnOk.Click += BtnOk_Click;
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Font = new Font("Tahoma", 12F);
            lblBrightness.Location = new Point(17, 27);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(123, 29);
            lblBrightness.TabIndex = 3;
            lblBrightness.Text = "Brightness";
            lblBrightness.UseMnemonic = false;
            // 
            // brightness
            // 
            brightness.Location = new Point(299, 27);
            brightness.Minimum = 1;
            brightness.Name = "brightness";
            brightness.Size = new Size(259, 69);
            brightness.TabIndex = 4;
            brightness.TickStyle = TickStyle.None;
            brightness.Value = 10;
            brightness.Scroll += Brightness_Scroll;
            // 
            // checkAutoConnect
            // 
            checkAutoConnect.AutoSize = true;
            checkAutoConnect.CheckAlign = ContentAlignment.MiddleRight;
            checkAutoConnect.Font = new Font("Tahoma", 12F);
            checkAutoConnect.Location = new Point(17, 71);
            checkAutoConnect.Name = "checkAutoConnect";
            checkAutoConnect.Size = new Size(271, 33);
            checkAutoConnect.TabIndex = 5;
            checkAutoConnect.Text = "Connect automatically";
            checkAutoConnect.UseMnemonic = false;
            checkAutoConnect.UseVisualStyleBackColor = true;
            checkAutoConnect.CheckedChanged += CheckAutoConnect_CheckedChanged;
            // 
            // lblKeepWake
            // 
            lblKeepWake.AutoSize = true;
            lblKeepWake.Font = new Font("Tahoma", 12F);
            lblKeepWake.Location = new Point(17, 121);
            lblKeepWake.Name = "lblKeepWake";
            lblKeepWake.Size = new Size(143, 29);
            lblKeepWake.TabIndex = 6;
            lblKeepWake.Text = "Keep awake";
            lblKeepWake.UseMnemonic = false;
            // 
            // radioKeepAwakeNever
            // 
            radioKeepAwakeNever.AutoSize = true;
            radioKeepAwakeNever.Cursor = Cursors.Hand;
            radioKeepAwakeNever.Location = new Point(3, 0);
            radioKeepAwakeNever.Margin = new Padding(3, 0, 3, 0);
            radioKeepAwakeNever.Name = "radioKeepAwakeNever";
            radioKeepAwakeNever.Size = new Size(87, 28);
            radioKeepAwakeNever.TabIndex = 0;
            radioKeepAwakeNever.Text = "Never";
            radioKeepAwakeNever.UseMnemonic = false;
            radioKeepAwakeNever.UseVisualStyleBackColor = true;
            radioKeepAwakeNever.CheckedChanged += RadioKeepAwakeNever_CheckedChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(radioKeepAwakeNever);
            flowLayoutPanel1.Controls.Add(radioKeepAwakeConnected);
            flowLayoutPanel1.Controls.Add(radioKeepAwakeAlways);
            flowLayoutPanel1.Location = new Point(192, 121);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(385, 29);
            flowLayoutPanel1.TabIndex = 7;
            // 
            // radioKeepAwakeConnected
            // 
            radioKeepAwakeConnected.AutoSize = true;
            radioKeepAwakeConnected.Checked = true;
            radioKeepAwakeConnected.Cursor = Cursors.Hand;
            radioKeepAwakeConnected.Location = new Point(96, 0);
            radioKeepAwakeConnected.Margin = new Padding(3, 0, 3, 0);
            radioKeepAwakeConnected.Name = "radioKeepAwakeConnected";
            radioKeepAwakeConnected.Size = new Size(183, 28);
            radioKeepAwakeConnected.TabIndex = 1;
            radioKeepAwakeConnected.TabStop = true;
            radioKeepAwakeConnected.Text = "When connected";
            radioKeepAwakeConnected.UseMnemonic = false;
            radioKeepAwakeConnected.UseVisualStyleBackColor = true;
            radioKeepAwakeConnected.CheckedChanged += RadioKeepAwakeConnected_CheckedChanged;
            // 
            // radioKeepAwakeAlways
            // 
            radioKeepAwakeAlways.AutoSize = true;
            radioKeepAwakeAlways.Cursor = Cursors.Hand;
            radioKeepAwakeAlways.Location = new Point(285, 0);
            radioKeepAwakeAlways.Margin = new Padding(3, 0, 3, 0);
            radioKeepAwakeAlways.Name = "radioKeepAwakeAlways";
            radioKeepAwakeAlways.Size = new Size(97, 28);
            radioKeepAwakeAlways.TabIndex = 2;
            radioKeepAwakeAlways.Text = "Always";
            radioKeepAwakeAlways.UseMnemonic = false;
            radioKeepAwakeAlways.UseVisualStyleBackColor = true;
            radioKeepAwakeAlways.CheckedChanged += RadioKeepAwakeAlways_CheckedChanged;
            // 
            // DeviceConfigurator
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(573, 233);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(lblKeepWake);
            Controls.Add(checkAutoConnect);
            Controls.Add(brightness);
            Controls.Add(lblBrightness);
            Controls.Add(btnOk);
            Name = "DeviceConfigurator";
            Text = "DeviceConfigurator";
            Load += DeviceConfigurator_Load;
            ((ISupportInitialize)brightness).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private ButtonPrimary btnOk;
        private Label lblBrightness;
        private TrackBar brightness;
        private CheckBox checkAutoConnect;
        private Label lblKeepWake;
        private TabRadioButton radioKeepAwakeNever;
        private FlowLayoutPanel flowLayoutPanel1;
        private TabRadioButton radioKeepAwakeConnected;
        private TabRadioButton radioKeepAwakeAlways;
    }
}