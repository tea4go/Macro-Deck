using System.ComponentModel;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI
{
    partial class ActionConfigurator
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
            btnApply = new ButtonPrimary();
            label2 = new Label();
            labelDescription = new Label();
            configurationPanel = new Panel();
            lblSelectToBegin = new Label();
            pluginSearch = new RoundedTextBox();
            pluginsList = new FlowLayoutPanel();
            selectedPluginIcon = new PictureBox();
            lblSelectedActionName = new Label();
            configurationPanel.SuspendLayout();
            ((ISupportInitialize)selectedPluginIcon).BeginInit();
            SuspendLayout();
            // 
            // btnApply
            // 
            btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnApply.BorderRadius = 8;
            btnApply.Cursor = Cursors.Hand;
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.Font = new Font("Tahoma", 9.75F);
            btnApply.ForeColor = Color.White;
            btnApply.HoverColor = Color.FromArgb(0, 89, 184);
            btnApply.Icon = null;
            btnApply.Location = new Point(1693, 1198);
            btnApply.Name = "btnApply";
            btnApply.Progress = 0;
            btnApply.ProgressColor = Color.FromArgb(0, 46, 94);
            btnApply.Size = new Size(126, 62);
            btnApply.TabIndex = 1;
            btnApply.Text = "Ok";
            btnApply.UseMnemonic = false;
            btnApply.UseVisualStyleBackColor = true;
            btnApply.UseWindowsAccentColor = true;
            btnApply.WriteProgress = true;
            btnApply.Click += BtnApply_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(270, 33);
            label2.Name = "label2";
            label2.Size = new Size(58, 21);
            label2.TabIndex = 2;
            label2.Text = "Action";
            label2.UseMnemonic = false;
            // 
            // labelDescription
            // 
            labelDescription.Font = new Font("Tahoma", 9F);
            labelDescription.ForeColor = Color.White;
            labelDescription.Location = new Point(518, 90);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(854, 58);
            labelDescription.TabIndex = 3;
            labelDescription.UseMnemonic = false;
            // 
            // configurationPanel
            // 
            configurationPanel.BackColor = Color.FromArgb(45, 45, 145);
            configurationPanel.Controls.Add(lblSelectToBegin);
            configurationPanel.Font = new Font("Tahoma", 14.25F);
            configurationPanel.Location = new Point(518, 148);
            configurationPanel.Name = "configurationPanel";
            configurationPanel.Size = new Size(857, 424);
            configurationPanel.TabIndex = 9;
            // 
            // lblSelectToBegin
            // 
            lblSelectToBegin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblSelectToBegin.Location = new Point(0, 496);
            lblSelectToBegin.Name = "lblSelectToBegin";
            lblSelectToBegin.Size = new Size(1301, 531);
            lblSelectToBegin.TabIndex = 0;
            lblSelectToBegin.Text = "Select a plugin and a action to begin";
            lblSelectToBegin.TextAlign = ContentAlignment.MiddleCenter;
            lblSelectToBegin.UseMnemonic = false;
            // 
            // pluginSearch
            // 
            pluginSearch.BackColor = Color.FromArgb(65, 65, 65);
            pluginSearch.Cursor = Cursors.Hand;
            pluginSearch.Font = new Font("Tahoma", 12F);
            pluginSearch.ForeColor = Color.White;
            pluginSearch.Icon = Properties.Resources.magnifying_glass;
            pluginSearch.Location = new Point(11, 16);
            pluginSearch.MaxCharacters = 32767;
            pluginSearch.Multiline = false;
            pluginSearch.Name = "pluginSearch";
            pluginSearch.Padding = new Padding(30, 5, 5, 8);
            pluginSearch.PasswordChar = false;
            pluginSearch.PlaceHolderColor = Color.Gray;
            pluginSearch.PlaceHolderText = "";
            pluginSearch.ReadOnly = false;
            pluginSearch.ScrollBars = ScrollBars.None;
            pluginSearch.SelectionStart = 0;
            pluginSearch.Size = new Size(487, 43);
            pluginSearch.TabIndex = 10;
            pluginSearch.TabStop = false;
            pluginSearch.TextAlignment = HorizontalAlignment.Left;
            pluginSearch.TextChanged += PluginSearch_TextChanged;
            // 
            // pluginsList
            // 
            pluginsList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pluginsList.AutoScroll = true;
            pluginsList.BackColor = Color.FromArgb(65, 65, 65);
            pluginsList.Location = new Point(11, 79);
            pluginsList.Name = "pluginsList";
            pluginsList.Size = new Size(487, 1181);
            pluginsList.TabIndex = 11;
            // 
            // selectedPluginIcon
            // 
            selectedPluginIcon.BackgroundImageLayout = ImageLayout.Stretch;
            selectedPluginIcon.Location = new Point(518, 16);
            selectedPluginIcon.Name = "selectedPluginIcon";
            selectedPluginIcon.Size = new Size(59, 50);
            selectedPluginIcon.TabIndex = 12;
            selectedPluginIcon.TabStop = false;
            // 
            // lblSelectedActionName
            // 
            lblSelectedActionName.AutoSize = true;
            lblSelectedActionName.Font = new Font("Tahoma", 15.75F);
            lblSelectedActionName.Location = new Point(612, 20);
            lblSelectedActionName.Name = "lblSelectedActionName";
            lblSelectedActionName.Size = new Size(0, 39);
            lblSelectedActionName.TabIndex = 13;
            lblSelectedActionName.TextAlign = ContentAlignment.MiddleCenter;
            lblSelectedActionName.UseMnemonic = false;
            // 
            // ActionConfigurator
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            ClientSize = new Size(1840, 1285);
            Controls.Add(lblSelectedActionName);
            Controls.Add(selectedPluginIcon);
            Controls.Add(pluginsList);
            Controls.Add(pluginSearch);
            Controls.Add(configurationPanel);
            Controls.Add(labelDescription);
            Controls.Add(btnApply);
            Name = "ActionConfigurator";
            Text = "Macro Deck :: Configure action";
            configurationPanel.ResumeLayout(false);
            ((ISupportInitialize)selectedPluginIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private ButtonPrimary btnApply;
        private Label label2;
        private Label labelDescription;
        private Panel configurationPanel;
        private Label lblSelectToBegin;
        private RoundedTextBox pluginSearch;
        private FlowLayoutPanel pluginsList;
        private PictureBox selectedPluginIcon;
        private Label lblSelectedActionName;
    }
}