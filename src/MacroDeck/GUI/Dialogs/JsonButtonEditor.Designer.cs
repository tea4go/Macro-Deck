
using System.ComponentModel;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class JsonButtonEditor
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
            jsonTextBox = new RoundedTextBox();
            btnApply = new ButtonPrimary();
            SuspendLayout();
            // 
            // jsonTextBox
            // 
            jsonTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            jsonTextBox.AutoScroll = true;
            jsonTextBox.BackColor = Color.FromArgb(65, 65, 65);
            jsonTextBox.Cursor = Cursors.Hand;
            jsonTextBox.Font = new Font("Tahoma", 9F);
            jsonTextBox.Icon = null;
            jsonTextBox.Location = new Point(15, 26);
            jsonTextBox.Margin = new Padding(4, 4, 4, 4);
            jsonTextBox.MaxCharacters = 32767;
            jsonTextBox.Multiline = true;
            jsonTextBox.Name = "jsonTextBox";
            jsonTextBox.Padding = new Padding(12, 8, 12, 8);
            jsonTextBox.PasswordChar = false;
            jsonTextBox.PlaceHolderColor = Color.Gray;
            jsonTextBox.PlaceHolderText = "";
            jsonTextBox.ReadOnly = false;
            jsonTextBox.ScrollBars = ScrollBars.Both;
            jsonTextBox.SelectionStart = 0;
            jsonTextBox.Size = new Size(1575, 735);
            jsonTextBox.TabIndex = 2;
            jsonTextBox.TextAlignment = HorizontalAlignment.Left;
            // 
            // btnApply
            // 
            btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnApply.BorderRadius = 8;
            btnApply.Cursor = Cursors.Hand;
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.Font = new Font("Tahoma", 9.75F);
            btnApply.ForeColor = Color.White;
            btnApply.HoverColor = Color.Empty;
            btnApply.Icon = null;
            btnApply.Location = new Point(1440, 787);
            btnApply.Margin = new Padding(4, 4, 4, 4);
            btnApply.Name = "btnApply";
            btnApply.Progress = 0;
            btnApply.ProgressColor = Color.FromArgb(0, 103, 225);
            btnApply.Size = new Size(150, 56);
            btnApply.TabIndex = 3;
            btnApply.Text = "Apply";
            btnApply.UseMnemonic = false;
            btnApply.UseVisualStyleBackColor = true;
            btnApply.UseWindowsAccentColor = true;
            btnApply.WriteProgress = true;
            btnApply.Click += BtnApply_Click;
            // 
            // JsonButtonEditor
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1605, 866);
            Controls.Add(btnApply);
            Controls.Add(jsonTextBox);
            Margin = new Padding(6, 9, 6, 9);
            Name = "JsonButtonEditor";
            Padding = new Padding(3, 3, 3, 3);
            Text = "Json editor";
            ResumeLayout(false);
        }

        #endregion

        private RoundedTextBox jsonTextBox;
        private ButtonPrimary btnApply;
    }
}