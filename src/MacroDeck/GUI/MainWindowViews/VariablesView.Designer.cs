using System.ComponentModel;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Variables;

namespace SuchByte.MacroDeck.GUI.MainWindowViews
{
    partial class VariablesView
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
            VariableManager.OnVariableChanged -= this.VariableChanged;
            VariableManager.OnVariableRemoved -= this.VariableRemoved;
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
            creatorFilter = new FlowLayoutPanel();
            variablesPanel = new FlowLayoutPanel();
            lblCreator = new Label();
            lblValue = new Label();
            lblType = new Label();
            lblName = new Label();
            btnCreateVariable = new ButtonPrimary();
            SuspendLayout();
            // 
            // creatorFilter
            // 
            creatorFilter.AutoScroll = true;
            creatorFilter.Cursor = Cursors.Hand;
            creatorFilter.Dock = DockStyle.Left;
            creatorFilter.Location = new Point(0, 0);
            creatorFilter.Name = "creatorFilter";
            creatorFilter.Size = new Size(224, 540);
            creatorFilter.TabIndex = 17;
            // 
            // variablesPanel
            // 
            variablesPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            variablesPanel.AutoScroll = true;
            variablesPanel.Location = new Point(240, 70);
            variablesPanel.Name = "variablesPanel";
            variablesPanel.Size = new Size(897, 385);
            variablesPanel.TabIndex = 11;
            // 
            // lblCreator
            // 
            lblCreator.Font = new Font("Tahoma", 12F);
            lblCreator.Location = new Point(860, 11);
            lblCreator.Name = "lblCreator";
            lblCreator.Size = new Size(163, 50);
            lblCreator.TabIndex = 15;
            lblCreator.Text = "Creator";
            lblCreator.TextAlign = ContentAlignment.MiddleLeft;
            lblCreator.UseMnemonic = false;
            // 
            // lblValue
            // 
            lblValue.Font = new Font("Tahoma", 12F);
            lblValue.Location = new Point(596, 11);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(258, 50);
            lblValue.TabIndex = 14;
            lblValue.Text = "Value";
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
            lblValue.UseMnemonic = false;
            // 
            // lblType
            // 
            lblType.Font = new Font("Tahoma", 12F);
            lblType.Location = new Point(476, 11);
            lblType.Name = "lblType";
            lblType.Size = new Size(114, 50);
            lblType.TabIndex = 13;
            lblType.Text = "Type";
            lblType.TextAlign = ContentAlignment.MiddleLeft;
            lblType.UseMnemonic = false;
            // 
            // lblName
            // 
            lblName.Font = new Font("Tahoma", 12F);
            lblName.Location = new Point(247, 11);
            lblName.Name = "lblName";
            lblName.Size = new Size(223, 50);
            lblName.TabIndex = 12;
            lblName.Text = "Name";
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.UseMnemonic = false;
            // 
            // btnCreateVariable
            // 
            btnCreateVariable.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCreateVariable.BorderRadius = 8;
            btnCreateVariable.Cursor = Cursors.Hand;
            btnCreateVariable.FlatAppearance.BorderSize = 0;
            btnCreateVariable.FlatStyle = FlatStyle.Flat;
            btnCreateVariable.Font = new Font("Tahoma", 9.75F);
            btnCreateVariable.ForeColor = Color.White;
            btnCreateVariable.HoverColor = Color.FromArgb(0, 89, 184);
            btnCreateVariable.Icon = null;
            btnCreateVariable.Location = new Point(905, 480);
            btnCreateVariable.Name = "btnCreateVariable";
            btnCreateVariable.Progress = 0;
            btnCreateVariable.ProgressColor = Color.FromArgb(0, 46, 94);
            btnCreateVariable.Size = new Size(215, 42);
            btnCreateVariable.TabIndex = 16;
            btnCreateVariable.Text = "Create variable";
            btnCreateVariable.UseMnemonic = false;
            btnCreateVariable.UseVisualStyleBackColor = false;
            btnCreateVariable.UseWindowsAccentColor = true;
            btnCreateVariable.WriteProgress = true;
            btnCreateVariable.Click += BtnCreateVariable_Click;
            // 
            // VariablesView
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 45);
            Controls.Add(creatorFilter);
            Controls.Add(btnCreateVariable);
            Controls.Add(lblCreator);
            Controls.Add(lblValue);
            Controls.Add(lblType);
            Controls.Add(lblName);
            Controls.Add(variablesPanel);
            Font = new Font("Tahoma", 9F);
            ForeColor = Color.White;
            Name = "VariablesView";
            Size = new Size(1137, 540);
            Load += VariablesPage_Load;
            ResumeLayout(false);

        }

        #endregion
        private FlowLayoutPanel variablesPanel;
        private Label lblCreator;
        private Label lblValue;
        private Label lblType;
        private Label lblName;
        private ButtonPrimary btnCreateVariable;
        private FlowLayoutPanel creatorFilter;
    }
}
