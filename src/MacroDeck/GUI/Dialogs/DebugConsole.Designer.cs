
using System.ComponentModel;
using System.Windows.Forms;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class DebugConsole
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
            components = new Container();
            logOutput = new RichTextBox();
            btnClear = new ButtonPrimary();
            btnRestartMacroDeck = new ButtonPrimary();
            btnExit = new ButtonPrimary();
            btnOpenUser = new ButtonPrimary();
            label1 = new Label();
            logLevel = new RoundedComboBox();
            btnExportOutput = new ButtonPrimary();
            label3 = new Label();
            filter = new RoundedTextBox();
            btnAddFilter = new ButtonPrimary();
            filtersList = new ContextMenuStrip(components);
            btnRemoveFilters = new ButtonPrimary();
            btnTestNotification = new ButtonPrimary();
            btnOpenLogs = new ButtonPrimary();
            SuspendLayout();
            // 
            // logOutput
            // 
            logOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logOutput.BackColor = Color.FromArgb(65, 65, 65);
            logOutput.BorderStyle = BorderStyle.None;
            logOutput.ForeColor = Color.White;
            logOutput.Location = new Point(15, 90);
            logOutput.Margin = new Padding(4);
            logOutput.Name = "logOutput";
            logOutput.ReadOnly = true;
            logOutput.Size = new Size(1170, 504);
            logOutput.TabIndex = 1;
            logOutput.Text = "";
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClear.BorderRadius = 8;
            btnClear.Cursor = Cursors.Hand;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Tahoma", 9.75F);
            btnClear.ForeColor = Color.White;
            btnClear.HoverColor = Color.FromArgb(0, 89, 184);
            btnClear.Icon = null;
            btnClear.Location = new Point(1005, 611);
            btnClear.Margin = new Padding(4);
            btnClear.Name = "btnClear";
            btnClear.Progress = 0;
            btnClear.ProgressColor = Color.FromArgb(0, 46, 94);
            btnClear.Size = new Size(183, 56);
            btnClear.TabIndex = 2;
            btnClear.Text = "Clear";
            btnClear.UseMnemonic = false;
            btnClear.UseVisualStyleBackColor = false;
            btnClear.UseWindowsAccentColor = true;
            btnClear.WriteProgress = true;
            btnClear.Click += BtnClear_Click;
            // 
            // btnRestartMacroDeck
            // 
            btnRestartMacroDeck.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRestartMacroDeck.BorderRadius = 8;
            btnRestartMacroDeck.Cursor = Cursors.Hand;
            btnRestartMacroDeck.FlatAppearance.BorderSize = 0;
            btnRestartMacroDeck.FlatStyle = FlatStyle.Flat;
            btnRestartMacroDeck.Font = new Font("Tahoma", 9.75F);
            btnRestartMacroDeck.ForeColor = Color.White;
            btnRestartMacroDeck.HoverColor = Color.FromArgb(0, 89, 184);
            btnRestartMacroDeck.Icon = null;
            btnRestartMacroDeck.Location = new Point(195, 611);
            btnRestartMacroDeck.Margin = new Padding(4);
            btnRestartMacroDeck.Name = "btnRestartMacroDeck";
            btnRestartMacroDeck.Progress = 0;
            btnRestartMacroDeck.ProgressColor = Color.FromArgb(0, 46, 94);
            btnRestartMacroDeck.Size = new Size(183, 56);
            btnRestartMacroDeck.TabIndex = 3;
            btnRestartMacroDeck.Text = "Restart Macro Deck";
            btnRestartMacroDeck.UseMnemonic = false;
            btnRestartMacroDeck.UseVisualStyleBackColor = false;
            btnRestartMacroDeck.UseWindowsAccentColor = true;
            btnRestartMacroDeck.WriteProgress = true;
            btnRestartMacroDeck.Click += BtnRestartMacroDeck_Click;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExit.BorderRadius = 8;
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Tahoma", 9.75F);
            btnExit.ForeColor = Color.White;
            btnExit.HoverColor = Color.FromArgb(0, 89, 184);
            btnExit.Icon = null;
            btnExit.Location = new Point(3, 611);
            btnExit.Margin = new Padding(4);
            btnExit.Name = "btnExit";
            btnExit.Progress = 0;
            btnExit.ProgressColor = Color.FromArgb(0, 46, 94);
            btnExit.Size = new Size(183, 56);
            btnExit.TabIndex = 4;
            btnExit.Text = "Exit Macro Deck";
            btnExit.UseMnemonic = false;
            btnExit.UseVisualStyleBackColor = false;
            btnExit.UseWindowsAccentColor = true;
            btnExit.WriteProgress = true;
            btnExit.Click += BtnExit_Click;
            // 
            // btnOpenUser
            // 
            btnOpenUser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenUser.BorderRadius = 8;
            btnOpenUser.Cursor = Cursors.Hand;
            btnOpenUser.FlatAppearance.BorderSize = 0;
            btnOpenUser.FlatStyle = FlatStyle.Flat;
            btnOpenUser.Font = new Font("Tahoma", 9.75F);
            btnOpenUser.ForeColor = Color.White;
            btnOpenUser.HoverColor = Color.FromArgb(0, 89, 184);
            btnOpenUser.Icon = null;
            btnOpenUser.Location = new Point(387, 611);
            btnOpenUser.Margin = new Padding(4);
            btnOpenUser.Name = "btnOpenUser";
            btnOpenUser.Progress = 0;
            btnOpenUser.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOpenUser.Size = new Size(183, 56);
            btnOpenUser.TabIndex = 5;
            btnOpenUser.Text = "Open user directory";
            btnOpenUser.UseMnemonic = false;
            btnOpenUser.UseVisualStyleBackColor = false;
            btnOpenUser.UseWindowsAccentColor = true;
            btnOpenUser.WriteProgress = true;
            btnOpenUser.Click += BtnOpenUser_Click;
            // 
            // label1
            // 
            label1.Font = new Font("Tahoma", 11.25F);
            label1.Location = new Point(10, 23);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(142, 39);
            label1.TabIndex = 6;
            label1.Text = "Log level:";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            label1.UseMnemonic = false;
            // 
            // logLevel
            // 
            logLevel.BackColor = Color.FromArgb(65, 65, 65);
            logLevel.Cursor = Cursors.Hand;
            logLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            logLevel.Font = new Font("Tahoma", 9F);
            logLevel.Icon = null;
            logLevel.Location = new Point(162, 27);
            logLevel.Margin = new Padding(4);
            logLevel.Name = "logLevel";
            logLevel.SelectedIndex = -1;
            logLevel.SelectedItem = null;
            logLevel.Size = new Size(254, 30);
            logLevel.TabIndex = 7;
            logLevel.SelectedIndexChanged += LogLevel_SelectedIndexChanged;
            // 
            // btnExportOutput
            // 
            btnExportOutput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExportOutput.BorderRadius = 8;
            btnExportOutput.Cursor = Cursors.Hand;
            btnExportOutput.FlatAppearance.BorderSize = 0;
            btnExportOutput.FlatStyle = FlatStyle.Flat;
            btnExportOutput.Font = new Font("Tahoma", 9.75F);
            btnExportOutput.ForeColor = Color.White;
            btnExportOutput.HoverColor = Color.FromArgb(0, 89, 184);
            btnExportOutput.Icon = null;
            btnExportOutput.Location = new Point(813, 611);
            btnExportOutput.Margin = new Padding(4);
            btnExportOutput.Name = "btnExportOutput";
            btnExportOutput.Progress = 0;
            btnExportOutput.ProgressColor = Color.FromArgb(0, 46, 94);
            btnExportOutput.Size = new Size(183, 56);
            btnExportOutput.TabIndex = 8;
            btnExportOutput.Text = "Export output";
            btnExportOutput.UseMnemonic = false;
            btnExportOutput.UseVisualStyleBackColor = false;
            btnExportOutput.UseWindowsAccentColor = true;
            btnExportOutput.WriteProgress = true;
            btnExportOutput.Click += BtnExportOutput_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.Font = new Font("Tahoma", 11.25F);
            label3.Location = new Point(588, 23);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(160, 39);
            label3.TabIndex = 9;
            label3.Text = "Filter:";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            label3.UseMnemonic = false;
            // 
            // filter
            // 
            filter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            filter.BackColor = Color.FromArgb(65, 65, 65);
            filter.Cursor = Cursors.Hand;
            filter.Font = new Font("Tahoma", 9F);
            filter.Icon = null;
            filter.Location = new Point(756, 23);
            filter.Margin = new Padding(4);
            filter.MaxCharacters = 32767;
            filter.Multiline = false;
            filter.Name = "filter";
            filter.Padding = new Padding(12, 8, 12, 8);
            filter.PasswordChar = false;
            filter.PlaceHolderColor = Color.Gray;
            filter.PlaceHolderText = "";
            filter.ReadOnly = false;
            filter.ScrollBars = ScrollBars.None;
            filter.SelectionStart = 0;
            filter.Size = new Size(352, 39);
            filter.TabIndex = 10;
            filter.TextAlignment = HorizontalAlignment.Left;
            // 
            // btnAddFilter
            // 
            btnAddFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddFilter.BorderRadius = 8;
            btnAddFilter.Cursor = Cursors.Hand;
            btnAddFilter.FlatAppearance.BorderSize = 0;
            btnAddFilter.FlatStyle = FlatStyle.Flat;
            btnAddFilter.Font = new Font("Tahoma", 9.75F);
            btnAddFilter.ForeColor = Color.White;
            btnAddFilter.HoverColor = Color.FromArgb(0, 89, 184);
            btnAddFilter.Icon = Properties.Resources.Create_Hover;
            btnAddFilter.Location = new Point(1112, 14);
            btnAddFilter.Margin = new Padding(4);
            btnAddFilter.Name = "btnAddFilter";
            btnAddFilter.Progress = 0;
            btnAddFilter.ProgressColor = Color.FromArgb(0, 46, 94);
            btnAddFilter.Size = new Size(38, 56);
            btnAddFilter.TabIndex = 11;
            btnAddFilter.UseMnemonic = false;
            btnAddFilter.UseVisualStyleBackColor = false;
            btnAddFilter.UseWindowsAccentColor = true;
            btnAddFilter.WriteProgress = true;
            btnAddFilter.Click += BtnAddFilter_Click;
            // 
            // filtersList
            // 
            filtersList.BackColor = Color.FromArgb(45, 45, 45);
            filtersList.Font = new Font("Tahoma", 11.25F);
            filtersList.ImageScalingSize = new Size(24, 24);
            filtersList.Name = "filtersList";
            filtersList.ShowImageMargin = false;
            filtersList.Size = new Size(36, 4);
            // 
            // btnRemoveFilters
            // 
            btnRemoveFilters.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveFilters.BorderRadius = 8;
            btnRemoveFilters.Cursor = Cursors.Hand;
            btnRemoveFilters.FlatAppearance.BorderSize = 0;
            btnRemoveFilters.FlatStyle = FlatStyle.Flat;
            btnRemoveFilters.Font = new Font("Tahoma", 9.75F);
            btnRemoveFilters.ForeColor = Color.White;
            btnRemoveFilters.HoverColor = Color.FromArgb(152, 0, 0);
            btnRemoveFilters.Icon = Properties.Resources.Delete_Hover;
            btnRemoveFilters.Location = new Point(1155, 14);
            btnRemoveFilters.Margin = new Padding(4);
            btnRemoveFilters.Name = "btnRemoveFilters";
            btnRemoveFilters.Progress = 0;
            btnRemoveFilters.ProgressColor = Color.FromArgb(0, 46, 94);
            btnRemoveFilters.Size = new Size(38, 56);
            btnRemoveFilters.TabIndex = 12;
            btnRemoveFilters.UseMnemonic = false;
            btnRemoveFilters.UseVisualStyleBackColor = false;
            btnRemoveFilters.UseWindowsAccentColor = false;
            btnRemoveFilters.WriteProgress = true;
            btnRemoveFilters.Click += btnRemoveFilters_Click;
            // 
            // btnTestNotification
            // 
            btnTestNotification.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnTestNotification.BorderRadius = 8;
            btnTestNotification.Cursor = Cursors.Hand;
            btnTestNotification.FlatAppearance.BorderSize = 0;
            btnTestNotification.FlatStyle = FlatStyle.Flat;
            btnTestNotification.Font = new Font("Tahoma", 9.75F);
            btnTestNotification.ForeColor = Color.White;
            btnTestNotification.HoverColor = Color.FromArgb(0, 89, 184);
            btnTestNotification.Icon = null;
            btnTestNotification.Location = new Point(579, 611);
            btnTestNotification.Margin = new Padding(4);
            btnTestNotification.Name = "btnTestNotification";
            btnTestNotification.Progress = 0;
            btnTestNotification.ProgressColor = Color.FromArgb(0, 46, 94);
            btnTestNotification.Size = new Size(183, 56);
            btnTestNotification.TabIndex = 13;
            btnTestNotification.Text = "Test notification";
            btnTestNotification.UseMnemonic = false;
            btnTestNotification.UseVisualStyleBackColor = false;
            btnTestNotification.UseWindowsAccentColor = true;
            btnTestNotification.WriteProgress = true;
            btnTestNotification.Click += btnTestNotification_Click;
            // 
            // btnOpenLogs
            // 
            btnOpenLogs.BorderRadius = 8;
            btnOpenLogs.Cursor = Cursors.Hand;
            btnOpenLogs.FlatAppearance.BorderSize = 0;
            btnOpenLogs.FlatStyle = FlatStyle.Flat;
            btnOpenLogs.Font = new Font("Tahoma", 9.75F);
            btnOpenLogs.ForeColor = Color.White;
            btnOpenLogs.HoverColor = Color.FromArgb(0, 89, 184);
            btnOpenLogs.Icon = null;
            btnOpenLogs.Location = new Point(424, 14);
            btnOpenLogs.Margin = new Padding(4);
            btnOpenLogs.Name = "btnOpenLogs";
            btnOpenLogs.Progress = 0;
            btnOpenLogs.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOpenLogs.Size = new Size(151, 56);
            btnOpenLogs.TabIndex = 14;
            btnOpenLogs.Text = "Open logs";
            btnOpenLogs.UseMnemonic = false;
            btnOpenLogs.UseVisualStyleBackColor = false;
            btnOpenLogs.UseWindowsAccentColor = true;
            btnOpenLogs.WriteProgress = true;
            btnOpenLogs.Click += BtnOpenLogs_Click;
            // 
            // DebugConsole
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1200, 689);
            Controls.Add(btnTestNotification);
            Controls.Add(btnRemoveFilters);
            Controls.Add(btnAddFilter);
            Controls.Add(filter);
            Controls.Add(label3);
            Controls.Add(btnExportOutput);
            Controls.Add(logLevel);
            Controls.Add(label1);
            Controls.Add(btnOpenUser);
            Controls.Add(btnExit);
            Controls.Add(btnRestartMacroDeck);
            Controls.Add(btnClear);
            Controls.Add(logOutput);
            Controls.Add(btnOpenLogs);
            Margin = new Padding(9, 6, 9, 6);
            MinimumSize = new Size(1189, 649);
            Name = "DebugConsole";
            Padding = new Padding(3);
            StartPosition = FormStartPosition.WindowsDefaultLocation;
            Load += DebugConsole_Load;
            ResumeLayout(false);

        }

        #endregion

        private RichTextBox logOutput;
        private ButtonPrimary btnClear;
        private ButtonPrimary btnRestartMacroDeck;
        private ButtonPrimary btnExit;
        private ButtonPrimary btnOpenUser;
        private Label label1;
        private RoundedComboBox logLevel;
        private ButtonPrimary btnExportOutput;
        private Label label3;
        private RoundedTextBox filter;
        private ButtonPrimary btnAddFilter;
        private ContextMenuStrip filtersList;
        private ButtonPrimary btnRemoveFilters;
        private ButtonPrimary btnTestNotification;
        private ButtonPrimary btnOpenLogs;
    }
}