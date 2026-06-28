
using System.ComponentModel;
using System.Windows.Forms;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class IconImportQuality
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
            qualityLowest = new RadioButton();
            label1 = new Label();
            qualityLow = new RadioButton();
            qualityNormal = new RadioButton();
            qualityHigh = new RadioButton();
            qualityOriginal = new RadioButton();
            lblInfo = new Label();
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
            btnOk.HoverColor = Color.Empty;
            btnOk.Icon = null;
            btnOk.Location = new Point(408, 282);
            btnOk.Margin = new Padding(4, 4, 4, 4);
            btnOk.Name = "btnOk";
            btnOk.Progress = 0;
            btnOk.ProgressColor = Color.FromArgb(0, 103, 205);
            btnOk.Size = new Size(136, 56);
            btnOk.TabIndex = 3;
            btnOk.Text = "Ok";
            btnOk.UseMnemonic = false;
            btnOk.UseVisualStyleBackColor = false;
            btnOk.UseWindowsAccentColor = true;
            btnOk.WriteProgress = true;
            btnOk.Click += BtnOk_Click;
            // 
            // qualityLowest
            // 
            qualityLowest.AutoSize = true;
            qualityLowest.Location = new Point(18, 216);
            qualityLowest.Margin = new Padding(4, 4, 4, 4);
            qualityLowest.Name = "qualityLowest";
            qualityLowest.Size = new Size(174, 28);
            qualityLowest.TabIndex = 4;
            qualityLowest.Text = "Lowest (100px)";
            qualityLowest.UseMnemonic = false;
            qualityLowest.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Tahoma", 12F);
            label1.Location = new Point(66, 18);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(86, 29);
            label1.TabIndex = 5;
            label1.Text = "Quality";
            label1.UseMnemonic = false;
            // 
            // qualityLow
            // 
            qualityLow.AutoSize = true;
            qualityLow.Location = new Point(18, 177);
            qualityLow.Margin = new Padding(4, 4, 4, 4);
            qualityLow.Name = "qualityLow";
            qualityLow.Size = new Size(147, 28);
            qualityLow.TabIndex = 6;
            qualityLow.Text = "Low (150px)";
            qualityLow.UseMnemonic = false;
            qualityLow.UseVisualStyleBackColor = true;
            // 
            // qualityNormal
            // 
            qualityNormal.AutoSize = true;
            qualityNormal.Checked = true;
            qualityNormal.Location = new Point(18, 138);
            qualityNormal.Margin = new Padding(4, 4, 4, 4);
            qualityNormal.Name = "qualityNormal";
            qualityNormal.Size = new Size(175, 28);
            qualityNormal.TabIndex = 7;
            qualityNormal.TabStop = true;
            qualityNormal.Text = "Normal (200px)";
            qualityNormal.UseVisualStyleBackColor = true;
            // 
            // qualityHigh
            // 
            qualityHigh.AutoSize = true;
            qualityHigh.Location = new Point(18, 99);
            qualityHigh.Margin = new Padding(4, 4, 4, 4);
            qualityHigh.Name = "qualityHigh";
            qualityHigh.Size = new Size(152, 28);
            qualityHigh.TabIndex = 8;
            qualityHigh.Text = "High (350px)";
            qualityHigh.UseMnemonic = false;
            qualityHigh.UseVisualStyleBackColor = true;
            // 
            // qualityOriginal
            // 
            qualityOriginal.AutoSize = true;
            qualityOriginal.Location = new Point(18, 60);
            qualityOriginal.Margin = new Padding(4, 4, 4, 4);
            qualityOriginal.Name = "qualityOriginal";
            qualityOriginal.Size = new Size(104, 28);
            qualityOriginal.TabIndex = 9;
            qualityOriginal.Text = "Original";
            qualityOriginal.UseMnemonic = false;
            qualityOriginal.UseVisualStyleBackColor = true;
            // 
            // lblInfo
            // 
            lblInfo.Location = new Point(222, 60);
            lblInfo.Margin = new Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(322, 218);
            lblInfo.TabIndex = 10;
            lblInfo.Text = "High quality icons can lead to an increase of memory usage and loading times especially when using many big animated gifs.\r\n\r\nFor gifs it is recommended to use the low or the lowest preset.";
            lblInfo.UseMnemonic = false;
            // 
            // IconImportQuality
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(564, 371);
            Controls.Add(lblInfo);
            Controls.Add(qualityOriginal);
            Controls.Add(qualityHigh);
            Controls.Add(qualityNormal);
            Controls.Add(qualityLow);
            Controls.Add(label1);
            Controls.Add(qualityLowest);
            Controls.Add(btnOk);
            Margin = new Padding(6, 9, 6, 9);
            Name = "IconImportQuality";
            Padding = new Padding(3, 3, 3, 3);
            Text = "Icon import";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ButtonPrimary btnOk;
        private RadioButton qualityLowest;
        private Label label1;
        private RadioButton qualityLow;
        private RadioButton qualityNormal;
        private RadioButton qualityHigh;
        private RadioButton qualityOriginal;
        private Label lblInfo;
    }
}