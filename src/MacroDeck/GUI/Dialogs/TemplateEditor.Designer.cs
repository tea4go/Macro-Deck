
using System.ComponentModel;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace SuchByte.MacroDeck.GUI.Dialogs
{
    partial class TemplateEditor
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(TemplateEditor));
            template = new FastColoredTextBox();
            btnOk = new ButtonPrimary();
            lblResultLabel = new Label();
            lblResult = new Label();
            btnVariables = new ButtonPrimary();
            lblTemplateEngineInfo = new LinkLabel();
            variablesContextMenu = new ContextMenuStrip(components);
            btnIf = new ButtonPrimary();
            btnAnd = new ButtonPrimary();
            btnOr = new ButtonPrimary();
            btnNot = new ButtonPrimary();
            checkTrimBlankLines = new CheckBox();
            ((ISupportInitialize)template).BeginInit();
            SuspendLayout();
            // 
            // template
            // 
            template.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            template.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            template.AutoScrollMinSize = new Size(33, 20);
            template.BackBrush = null;
            template.BackColor = Color.FromArgb(65, 65, 65);
            template.CaretColor = Color.White;
            template.CharHeight = 20;
            template.CharWidth = 11;
            template.CommentPrefix = "{_";
            template.Cursor = Cursors.IBeam;
            template.DefaultMarkerSize = 8;
            template.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            template.FindForm = null;
            template.Font = new Font("Courier New", 9F);
            template.GoToForm = null;
            template.Hotkeys = resources.GetString("template.Hotkeys");
            template.IsReplaceMode = false;
            template.Location = new Point(18, 150);
            template.Margin = new Padding(4);
            template.Name = "template";
            template.Paddings = new Padding(0);
            template.ReplaceForm = null;
            template.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            template.ServiceColors = null;
            template.Size = new Size(1114, 398);
            template.TabIndex = 2;
            template.TabStop = false;
            template.Zoom = 100;
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
            btnOk.Location = new Point(1020, 815);
            btnOk.Margin = new Padding(4);
            btnOk.Name = "btnOk";
            btnOk.Progress = 0;
            btnOk.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOk.Size = new Size(112, 56);
            btnOk.TabIndex = 3;
            btnOk.TabStop = false;
            btnOk.Text = "Ok";
            btnOk.UseMnemonic = false;
            btnOk.UseVisualStyleBackColor = false;
            btnOk.UseWindowsAccentColor = true;
            btnOk.WriteProgress = true;
            btnOk.Click += BtnOk_Click;
            // 
            // lblResultLabel
            // 
            lblResultLabel.Font = new Font("Tahoma", 11.25F, FontStyle.Bold);
            lblResultLabel.Location = new Point(23, 629);
            lblResultLabel.Margin = new Padding(4, 0, 4, 0);
            lblResultLabel.Name = "lblResultLabel";
            lblResultLabel.Size = new Size(434, 33);
            lblResultLabel.TabIndex = 4;
            lblResultLabel.Text = "Result";
            lblResultLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblResult
            // 
            lblResult.Location = new Point(23, 676);
            lblResult.Margin = new Padding(4, 0, 4, 0);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(981, 190);
            lblResult.TabIndex = 5;
            lblResult.UseMnemonic = false;
            // 
            // btnVariables
            // 
            btnVariables.BorderRadius = 8;
            btnVariables.Cursor = Cursors.Hand;
            btnVariables.FlatAppearance.BorderSize = 0;
            btnVariables.FlatStyle = FlatStyle.Flat;
            btnVariables.Font = new Font("Tahoma", 9.75F);
            btnVariables.ForeColor = Color.White;
            btnVariables.HoverColor = Color.FromArgb(0, 89, 184);
            btnVariables.Icon = null;
            btnVariables.Location = new Point(14, 74);
            btnVariables.Margin = new Padding(4);
            btnVariables.Name = "btnVariables";
            btnVariables.Progress = 0;
            btnVariables.ProgressColor = Color.FromArgb(0, 46, 94);
            btnVariables.Size = new Size(174, 56);
            btnVariables.TabIndex = 6;
            btnVariables.Text = "Variables";
            btnVariables.UseMnemonic = false;
            btnVariables.UseVisualStyleBackColor = false;
            btnVariables.UseWindowsAccentColor = true;
            btnVariables.WriteProgress = true;
            btnVariables.Click += BtnVariables_Click;
            // 
            // lblTemplateEngineInfo
            // 
            lblTemplateEngineInfo.ActiveLinkColor = Color.FromArgb(0, 46, 94);
            lblTemplateEngineInfo.LinkColor = Color.FromArgb(0, 123, 255);
            lblTemplateEngineInfo.Location = new Point(14, 26);
            lblTemplateEngineInfo.Margin = new Padding(4, 0, 4, 0);
            lblTemplateEngineInfo.Name = "lblTemplateEngineInfo";
            lblTemplateEngineInfo.Size = new Size(1042, 44);
            lblTemplateEngineInfo.TabIndex = 11;
            lblTemplateEngineInfo.TabStop = true;
            lblTemplateEngineInfo.Text = "Macro Deck uses the Cottle template engine. Click here for more information.";
            lblTemplateEngineInfo.UseMnemonic = false;
            lblTemplateEngineInfo.VisitedLinkColor = Color.FromArgb(0, 123, 255);
            lblTemplateEngineInfo.LinkClicked += LblTemplateEngineInfo_LinkClicked;
            // 
            // variablesContextMenu
            // 
            variablesContextMenu.BackColor = Color.FromArgb(45, 45, 45);
            variablesContextMenu.Font = new Font("Tahoma", 9F);
            variablesContextMenu.ImageScalingSize = new Size(24, 24);
            variablesContextMenu.Name = "variablesContextMenu";
            variablesContextMenu.ShowImageMargin = false;
            variablesContextMenu.Size = new Size(36, 4);
            // 
            // btnIf
            // 
            btnIf.BorderRadius = 8;
            btnIf.Cursor = Cursors.Hand;
            btnIf.FlatAppearance.BorderSize = 0;
            btnIf.FlatStyle = FlatStyle.Flat;
            btnIf.Font = new Font("Tahoma", 9.75F);
            btnIf.ForeColor = Color.White;
            btnIf.HoverColor = Color.FromArgb(0, 89, 184);
            btnIf.Icon = null;
            btnIf.Location = new Point(196, 74);
            btnIf.Margin = new Padding(4);
            btnIf.Name = "btnIf";
            btnIf.Progress = 0;
            btnIf.ProgressColor = Color.FromArgb(0, 46, 94);
            btnIf.Size = new Size(112, 56);
            btnIf.TabIndex = 7;
            btnIf.Text = "If";
            btnIf.UseMnemonic = false;
            btnIf.UseVisualStyleBackColor = false;
            btnIf.UseWindowsAccentColor = true;
            btnIf.WriteProgress = true;
            btnIf.Click += BtnIf_Click;
            // 
            // btnAnd
            // 
            btnAnd.BorderRadius = 8;
            btnAnd.Cursor = Cursors.Hand;
            btnAnd.FlatAppearance.BorderSize = 0;
            btnAnd.FlatStyle = FlatStyle.Flat;
            btnAnd.Font = new Font("Tahoma", 9.75F);
            btnAnd.ForeColor = Color.White;
            btnAnd.HoverColor = Color.FromArgb(0, 89, 184);
            btnAnd.Icon = null;
            btnAnd.Location = new Point(318, 74);
            btnAnd.Margin = new Padding(4);
            btnAnd.Name = "btnAnd";
            btnAnd.Progress = 0;
            btnAnd.ProgressColor = Color.FromArgb(0, 46, 94);
            btnAnd.Size = new Size(112, 56);
            btnAnd.TabIndex = 8;
            btnAnd.Text = "And";
            btnAnd.UseMnemonic = false;
            btnAnd.UseVisualStyleBackColor = false;
            btnAnd.UseWindowsAccentColor = true;
            btnAnd.WriteProgress = true;
            btnAnd.Click += BtnAnd_Click;
            // 
            // btnOr
            // 
            btnOr.BorderRadius = 8;
            btnOr.Cursor = Cursors.Hand;
            btnOr.FlatAppearance.BorderSize = 0;
            btnOr.FlatStyle = FlatStyle.Flat;
            btnOr.Font = new Font("Tahoma", 9.75F);
            btnOr.ForeColor = Color.White;
            btnOr.HoverColor = Color.FromArgb(0, 89, 184);
            btnOr.Icon = null;
            btnOr.Location = new Point(440, 74);
            btnOr.Margin = new Padding(4);
            btnOr.Name = "btnOr";
            btnOr.Progress = 0;
            btnOr.ProgressColor = Color.FromArgb(0, 46, 94);
            btnOr.Size = new Size(112, 56);
            btnOr.TabIndex = 9;
            btnOr.Text = "Or";
            btnOr.UseMnemonic = false;
            btnOr.UseVisualStyleBackColor = false;
            btnOr.UseWindowsAccentColor = true;
            btnOr.WriteProgress = true;
            btnOr.Click += BtnOr_Click;
            // 
            // btnNot
            // 
            btnNot.BorderRadius = 8;
            btnNot.Cursor = Cursors.Hand;
            btnNot.FlatAppearance.BorderSize = 0;
            btnNot.FlatStyle = FlatStyle.Flat;
            btnNot.Font = new Font("Tahoma", 9.75F);
            btnNot.ForeColor = Color.White;
            btnNot.HoverColor = Color.FromArgb(0, 89, 184);
            btnNot.Icon = null;
            btnNot.Location = new Point(561, 74);
            btnNot.Margin = new Padding(4);
            btnNot.Name = "btnNot";
            btnNot.Progress = 0;
            btnNot.ProgressColor = Color.FromArgb(0, 46, 94);
            btnNot.Size = new Size(112, 56);
            btnNot.TabIndex = 10;
            btnNot.Text = "Not";
            btnNot.UseMnemonic = false;
            btnNot.UseVisualStyleBackColor = false;
            btnNot.UseWindowsAccentColor = true;
            btnNot.WriteProgress = true;
            btnNot.Click += BtnNot_Click;
            // 
            // checkTrimBlankLines
            // 
            checkTrimBlankLines.Location = new Point(23, 563);
            checkTrimBlankLines.Margin = new Padding(4);
            checkTrimBlankLines.Name = "checkTrimBlankLines";
            checkTrimBlankLines.Size = new Size(507, 40);
            checkTrimBlankLines.TabIndex = 12;
            checkTrimBlankLines.Text = "Trim blank lines";
            checkTrimBlankLines.UseVisualStyleBackColor = true;
            checkTrimBlankLines.CheckedChanged += CheckTrimBlankLines_CheckedChanged;
            // 
            // TemplateEditor
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            ClientSize = new Size(1149, 878);
            Controls.Add(checkTrimBlankLines);
            Controls.Add(lblTemplateEngineInfo);
            Controls.Add(btnNot);
            Controls.Add(btnOr);
            Controls.Add(btnAnd);
            Controls.Add(btnIf);
            Controls.Add(btnVariables);
            Controls.Add(lblResult);
            Controls.Add(lblResultLabel);
            Controls.Add(btnOk);
            Controls.Add(template);
            Margin = new Padding(6, 9, 6, 9);
            Name = "TemplateEditor";
            Padding = new Padding(3);
            Text = "Edit template";
            ((ISupportInitialize)template).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private FastColoredTextBox template;
        private ButtonPrimary btnOk;
        private Label lblResultLabel;
        private Label lblResult;
        private ButtonPrimary btnVariables;
        private LinkLabel lblTemplateEngineInfo;
        private ContextMenuStrip variablesContextMenu;
        private ButtonPrimary btnIf;
        private ButtonPrimary btnAnd;
        private ButtonPrimary btnOr;
        private ButtonPrimary btnNot;
        private CheckBox checkTrimBlankLines;
    }
}