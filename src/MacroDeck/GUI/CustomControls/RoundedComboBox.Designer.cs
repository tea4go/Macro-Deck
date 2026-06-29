
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.GUI.CustomControls
{
    partial class RoundedComboBox
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
            borderlessComboBox1 = new BorderlessComboBox();
            SuspendLayout();
            // 
            // borderlessComboBox1
            // 
            borderlessComboBox1.BackColor = Color.FromArgb(65, 65, 65);
            borderlessComboBox1.Dock = DockStyle.Fill;
            borderlessComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            borderlessComboBox1.FlatStyle = FlatStyle.Popup;
            borderlessComboBox1.ForeColor = Color.White;
            borderlessComboBox1.FormattingEnabled = true;
            borderlessComboBox1.Location = new Point(0, 0);
            borderlessComboBox1.Margin = new Padding(0);
            borderlessComboBox1.Name = "borderlessComboBox1";
            borderlessComboBox1.Size = new Size(250, 30);
            borderlessComboBox1.TabIndex = 0;
            borderlessComboBox1.Tag = "no-font";
            borderlessComboBox1.SelectedIndexChanged += BorderlessComboBox1_SelectedIndexChanged;
            borderlessComboBox1.TextChanged += BorderlessComboBox1_TextChanged;
            borderlessComboBox1.Click += BorderlessComboBox1_Click;
            borderlessComboBox1.Enter += BorderlessComboBox1_Enter;
            borderlessComboBox1.GotFocus += BorderlessComboBox1_GotFocus;
            borderlessComboBox1.KeyPress += BorderlessComboBox1_KeyPress;
            borderlessComboBox1.LostFocus += BorderlessComboBox1_LostFocus;
            // 
            // RoundedComboBox
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(65, 65, 65);
            Controls.Add(borderlessComboBox1);
            Font = new Font("Tahoma", 9F);
            Name = "RoundedComboBox";
            Size = new Size(250, 38);
            ResumeLayout(false);

        }




        #endregion

        private BorderlessComboBox borderlessComboBox1;
    }
}
