using System.ComponentModel;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.CustomControls.Notifications
{
    partial class NotificationItem
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
            pluginIcon = new PictureBox();
            lblPluginName = new Label();
            lblTitle = new Label();
            lblMessage = new Label();
            additionalControls = new FlowLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            lblDateTime = new Label();
            btnRemove = new PictureButton();
            ((ISupportInitialize)pluginIcon).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((ISupportInitialize)btnRemove).BeginInit();
            SuspendLayout();
            // 
            // pluginIcon
            // 
            pluginIcon.BackgroundImageLayout = ImageLayout.Stretch;
            pluginIcon.Location = new Point(8, 42);
            pluginIcon.Name = "pluginIcon";
            pluginIcon.Size = new Size(50, 50);
            pluginIcon.TabIndex = 0;
            pluginIcon.TabStop = false;
            // 
            // lblPluginName
            // 
            lblPluginName.AutoSize = true;
            lblPluginName.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblPluginName.ForeColor = Color.Silver;
            lblPluginName.Location = new Point(64, 17);
            lblPluginName.Name = "lblPluginName";
            lblPluginName.Size = new Size(64, 22);
            lblPluginName.TabIndex = 1;
            lblPluginName.Text = "label1";
            lblPluginName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Tahoma", 12F);
            lblTitle.Location = new Point(63, 51);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(75, 29);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "label1";
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Tahoma", 9.75F);
            lblMessage.Location = new Point(0, 0);
            lblMessage.Margin = new Padding(0);
            lblMessage.MaximumSize = new Size(400, 0);
            lblMessage.MinimumSize = new Size(400, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(400, 24);
            lblMessage.TabIndex = 3;
            // 
            // additionalControls
            // 
            additionalControls.AutoSize = true;
            additionalControls.Location = new Point(0, 34);
            additionalControls.Margin = new Padding(0, 10, 0, 0);
            additionalControls.MaximumSize = new Size(400, 0);
            additionalControls.MinimumSize = new Size(400, 0);
            additionalControls.Name = "additionalControls";
            additionalControls.Size = new Size(400, 0);
            additionalControls.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(lblMessage);
            flowLayoutPanel1.Controls.Add(additionalControls);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(64, 89);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(400, 34);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // lblDateTime
            // 
            lblDateTime.AutoSize = true;
            lblDateTime.Font = new Font("Tahoma", 9F);
            lblDateTime.ForeColor = Color.Silver;
            lblDateTime.Location = new Point(398, 11);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Size = new Size(191, 22);
            lblDateTime.TabIndex = 6;
            lblDateTime.Text = "00.00.0000 - 00:00:00";
            lblDateTime.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemove.BackColor = Color.Transparent;
            btnRemove.BackgroundImageLayout = ImageLayout.Stretch;
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.HoverImage = Resources.Delete_Hover;
            btnRemove.Image = Resources.Delete_Normal;
            btnRemove.Location = new Point(608, 24);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(30, 30);
            btnRemove.TabIndex = 7;
            btnRemove.TabStop = false;
            btnRemove.Click += BtnRemove_Click;
            // 
            // NotificationItem
            // 
            AutoScaleDimensions = new SizeF(10F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(btnRemove);
            Controls.Add(lblDateTime);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(lblTitle);
            Controls.Add(lblPluginName);
            Controls.Add(pluginIcon);
            Font = new Font("Tahoma", 9F);
            ForeColor = Color.White;
            MinimumSize = new Size(520, 0);
            Name = "NotificationItem";
            Size = new Size(650, 135);
            Load += NotificationItem_Load;
            ((ISupportInitialize)pluginIcon).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((ISupportInitialize)btnRemove).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private PictureBox pluginIcon;
        private Label lblPluginName;
        private Label lblTitle;
        private Label lblMessage;
        private FlowLayoutPanel additionalControls;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label lblDateTime;
        private PictureButton btnRemove;
    }
}
