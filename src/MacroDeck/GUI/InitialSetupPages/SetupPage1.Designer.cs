
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.InitialSetupPages
{
    partial class SetupPage1
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
            lblWelcome = new Label();
            pictureBox1 = new PictureBox();
            lblLetsConfigure = new Label();
            languages = new ListBox();
            lblSelectLanguage = new Label();
            ((ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lblWelcome
            // 
            lblWelcome.Font = new Font("Tahoma", 26.25F);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(129, 3);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(559, 109);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Welcome to Macro Deck 2!";
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            lblWelcome.UseMnemonic = false;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Image = Resources.Macro_Deck_2021;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(120, 112);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // lblLetsConfigure
            // 
            lblLetsConfigure.Font = new Font("Tahoma", 18F);
            lblLetsConfigure.ForeColor = Color.White;
            lblLetsConfigure.Location = new Point(3, 118);
            lblLetsConfigure.Name = "lblLetsConfigure";
            lblLetsConfigure.Size = new Size(685, 67);
            lblLetsConfigure.TabIndex = 2;
            lblLetsConfigure.Text = "Let's configure your Macro Deck experience";
            lblLetsConfigure.TextAlign = ContentAlignment.MiddleCenter;
            lblLetsConfigure.UseMnemonic = false;
            // 
            // languages
            // 
            languages.BackColor = Color.FromArgb(55, 55, 55);
            languages.BorderStyle = BorderStyle.FixedSingle;
            languages.Font = new Font("Tahoma", 11.25F);
            languages.ForeColor = Color.White;
            languages.FormattingEnabled = true;
            languages.Location = new Point(231, 297);
            languages.Name = "languages";
            languages.Size = new Size(228, 198);
            languages.TabIndex = 4;
            languages.SelectedIndexChanged += Languages_SelectedIndexChanged;
            // 
            // lblSelectLanguage
            // 
            lblSelectLanguage.Font = new Font("Tahoma", 12F);
            lblSelectLanguage.ForeColor = Color.White;
            lblSelectLanguage.Location = new Point(231, 244);
            lblSelectLanguage.Name = "lblSelectLanguage";
            lblSelectLanguage.Size = new Size(228, 46);
            lblSelectLanguage.TabIndex = 5;
            lblSelectLanguage.Text = "Select your language";
            lblSelectLanguage.TextAlign = ContentAlignment.MiddleCenter;
            lblSelectLanguage.UseMnemonic = false;
            // 
            // SetupPage1
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(lblSelectLanguage);
            Controls.Add(languages);
            Controls.Add(lblLetsConfigure);
            Controls.Add(pictureBox1);
            Controls.Add(lblWelcome);
            Font = new Font("Tahoma", 9F);
            Name = "SetupPage1";
            Size = new Size(691, 571);
            Load += SetupPage1_Load;
            ((ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Label lblWelcome;
        private PictureBox pictureBox1;
        private Label lblLetsConfigure;
        private ListBox languages;
        private Label lblSelectLanguage;
    }
}
