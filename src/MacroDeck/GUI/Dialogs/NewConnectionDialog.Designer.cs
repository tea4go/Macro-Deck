
using System.ComponentModel;
using System.Windows.Forms;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class NewConnectionDialog
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

            this._denyTimer?.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            lblNewConnectionRequest = new Label();
            lblClientId = new Label();
            lblType = new Label();
            type = new Label();
            clientId = new Label();
            btnAccept = new ButtonPrimary();
            btnDeny = new ButtonPrimary();
            checkBlockThisDevice = new CheckBox();
            ((ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Image = Properties.Resources.Macro_Deck_2021;
            pictureBox1.Location = new Point(16, 27);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(53, 53);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // lblNewConnectionRequest
            // 
            lblNewConnectionRequest.Font = new Font("Tahoma", 21.75F);
            lblNewConnectionRequest.Location = new Point(75, 27);
            lblNewConnectionRequest.Name = "lblNewConnectionRequest";
            lblNewConnectionRequest.Size = new Size(451, 53);
            lblNewConnectionRequest.TabIndex = 3;
            lblNewConnectionRequest.Text = "New connection request";
            lblNewConnectionRequest.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblClientId
            // 
            lblClientId.AutoSize = true;
            lblClientId.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblClientId.Location = new Point(32, 127);
            lblClientId.Name = "lblClientId";
            lblClientId.Size = new Size(116, 29);
            lblClientId.TabIndex = 4;
            lblClientId.Text = "Client Id";
            lblClientId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblType.Location = new Point(32, 181);
            lblType.Name = "lblType";
            lblType.Size = new Size(71, 29);
            lblType.TabIndex = 6;
            lblType.Text = "Type";
            lblType.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // type
            // 
            type.Font = new Font("Tahoma", 12F);
            type.Location = new Point(203, 181);
            type.Name = "type";
            type.Size = new Size(307, 27);
            type.TabIndex = 9;
            type.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // clientId
            // 
            clientId.Font = new Font("Tahoma", 12F);
            clientId.Location = new Point(203, 127);
            clientId.Name = "clientId";
            clientId.Size = new Size(307, 27);
            clientId.TabIndex = 7;
            clientId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnAccept
            // 
            btnAccept.BorderRadius = 8;
            btnAccept.Cursor = Cursors.Hand;
            btnAccept.FlatAppearance.BorderSize = 0;
            btnAccept.FlatStyle = FlatStyle.Flat;
            btnAccept.Font = new Font("Tahoma", 14.25F);
            btnAccept.ForeColor = Color.White;
            btnAccept.HoverColor = Color.Empty;
            btnAccept.Icon = null;
            btnAccept.Location = new Point(167, 256);
            btnAccept.Name = "btnAccept";
            btnAccept.Progress = 0;
            btnAccept.ProgressColor = Color.FromArgb(0, 103, 205);
            btnAccept.Size = new Size(209, 60);
            btnAccept.TabIndex = 10;
            btnAccept.Text = "Accept";
            btnAccept.UseVisualStyleBackColor = true;
            btnAccept.UseWindowsAccentColor = true;
            btnAccept.WriteProgress = true;
            btnAccept.Click += BtnAccept_Click;
            // 
            // btnDeny
            // 
            btnDeny.BorderRadius = 8;
            btnDeny.Cursor = Cursors.Hand;
            btnDeny.FlatAppearance.BorderSize = 0;
            btnDeny.FlatStyle = FlatStyle.Flat;
            btnDeny.Font = new Font("Tahoma", 14.25F);
            btnDeny.ForeColor = Color.White;
            btnDeny.HoverColor = Color.Empty;
            btnDeny.Icon = null;
            btnDeny.Location = new Point(167, 322);
            btnDeny.Name = "btnDeny";
            btnDeny.Progress = 0;
            btnDeny.ProgressColor = Color.FromArgb(0, 103, 205);
            btnDeny.Size = new Size(209, 60);
            btnDeny.TabIndex = 11;
            btnDeny.Text = "Deny";
            btnDeny.UseVisualStyleBackColor = true;
            btnDeny.UseWindowsAccentColor = false;
            btnDeny.WriteProgress = true;
            btnDeny.Click += BtnDeny_Click;
            // 
            // checkBlockThisDevice
            // 
            checkBlockThisDevice.CheckAlign = ContentAlignment.TopLeft;
            checkBlockThisDevice.Location = new Point(172, 388);
            checkBlockThisDevice.Name = "checkBlockThisDevice";
            checkBlockThisDevice.Size = new Size(204, 42);
            checkBlockThisDevice.TabIndex = 12;
            checkBlockThisDevice.Text = "Block this device";
            checkBlockThisDevice.TextAlign = ContentAlignment.TopLeft;
            checkBlockThisDevice.UseVisualStyleBackColor = true;
            // 
            // NewConnectionDialog
            // 
            AcceptButton = btnAccept;
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(543, 441);
            Controls.Add(checkBlockThisDevice);
            Controls.Add(btnDeny);
            Controls.Add(btnAccept);
            Controls.Add(type);
            Controls.Add(clientId);
            Controls.Add(lblType);
            Controls.Add(lblClientId);
            Controls.Add(lblNewConnectionRequest);
            Controls.Add(pictureBox1);
            Name = "NewConnectionDialog";
            ShowIcon = true;
            ShowInTaskbar = true;
            Text = "Macro Deck :: New connection";
            Load += NewConnectionDialog_Load;
            ((ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblNewConnectionRequest;
        private Label lblClientId;
        private Label lblType;
        private Label type;
        private Label clientId;
        private ButtonPrimary btnAccept;
        private ButtonPrimary btnDeny;
        private CheckBox checkBlockThisDevice;
    }
}