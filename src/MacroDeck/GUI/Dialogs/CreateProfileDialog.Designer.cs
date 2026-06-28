
using System.ComponentModel;
using System.Windows.Forms;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class CreateProfileDialog
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
            lblName = new Label();
            profileName = new RoundedTextBox();
            btnOk = new ButtonPrimary();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Tahoma", 11.25F);
            lblName.ForeColor = Color.White;
            lblName.Location = new Point(18, 48);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(78, 28);
            lblName.TabIndex = 4;
            lblName.Text = "Name:";
            lblName.UseMnemonic = false;
            // 
            // profileName
            // 
            profileName.BackColor = Color.FromArgb(65, 65, 65);
            profileName.Font = new Font("Tahoma", 9F);
            profileName.ForeColor = Color.White;
            profileName.Icon = null;
            profileName.Location = new Point(126, 45);
            profileName.Margin = new Padding(4, 4, 4, 4);
            profileName.MaxCharacters = 32767;
            profileName.Multiline = false;
            profileName.Name = "profileName";
            profileName.Padding = new Padding(12, 8, 12, 8);
            profileName.PasswordChar = false;
            profileName.PlaceHolderColor = Color.Gray;
            profileName.PlaceHolderText = "";
            profileName.ReadOnly = false;
            profileName.ScrollBars = ScrollBars.None;
            profileName.SelectionStart = 0;
            profileName.Size = new Size(326, 39);
            profileName.TabIndex = 3;
            profileName.TextAlignment = HorizontalAlignment.Left;
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
            btnOk.Location = new Point(339, 92);
            btnOk.Margin = new Padding(4, 4, 4, 4);
            btnOk.Name = "btnOk";
            btnOk.Progress = 0;
            btnOk.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOk.Size = new Size(112, 56);
            btnOk.TabIndex = 5;
            btnOk.Text = "Ok";
            btnOk.UseMnemonic = false;
            btnOk.UseVisualStyleBackColor = false;
            btnOk.UseWindowsAccentColor = true;
            btnOk.WriteProgress = true;
            btnOk.Click += BtnOk_Click;
            // 
            // CreateProfileDialog
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(474, 170);
            Controls.Add(btnOk);
            Controls.Add(lblName);
            Controls.Add(profileName);
            Margin = new Padding(6, 9, 6, 9);
            Name = "CreateProfileDialog";
            Padding = new Padding(3, 3, 3, 3);
            Text = "Create profile";
            Load += CreateProfileDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblName;
        private RoundedTextBox profileName;
        private ButtonPrimary btnOk;
    }
}