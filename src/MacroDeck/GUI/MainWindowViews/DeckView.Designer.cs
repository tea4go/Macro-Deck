using System.ComponentModel;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.GUI.MainWindowViews
{
    partial class DeckView
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
            try
            {
                foreach (RoundedButton roundedButton in this.buttonPanel.Controls)
                {
                    ActionButton.ActionButton actionButton = this._currentFolder.ActionButtons.Find(aB => aB.Position_X == roundedButton.Column && aB.Position_Y == roundedButton.Row);
                    if (actionButton != null)
                    {
                        actionButton.StateChanged -= this.ButtonStateChanged;
                        actionButton.LabelOff.LabelBase64Changed -= this.LabelChanged;
                        actionButton.LabelOn.LabelBase64Changed -= this.LabelChanged;
                    }
                    roundedButton.MouseDown -= this.ActionButton_Down;
                    roundedButton.DragDrop -= Button_DragDrop;
                    roundedButton.DragEnter -= Button_DragEnter;
                    this.Invoke(new Action(() => roundedButton.Dispose()));

                }
            } catch { }
            if (disposing)
            {
                qrCodeBox.BackgroundImage?.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
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
            components = new Container();
            foldersView = new TreeView();
            buttonPanel = new BufferedPanel();
            actionButtonContextMenuItemEdit = new ToolStripMenuItem();
            actionButtonContextMenuItemDelete = new ToolStripMenuItem();
            foldersContextMenuNew = new ToolStripMenuItem();
            foldersContextMenuEdit = new ToolStripMenuItem();
            foldersContextMenuDelete = new ToolStripMenuItem();
            foldersContextMenu = new ContextMenuStrip(components);
            actionButtonContextMenu = new ContextMenuStrip(components);
            toolStripSeparator2 = new ToolStripSeparator();
            actionButtonContextMenuItemSimulatePress = new ToolStripMenuItem();
            actionButtonContextMenuItemSimulateRelease = new ToolStripMenuItem();
            actionButtonContextMenuItemSimulateLongPress = new ToolStripMenuItem();
            actionButtonContextMenuItemSimulateLongPressRelease = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            actionButtonContextMenuItemCopy = new ToolStripMenuItem();
            actionButtonContextMenuItemPaste = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            boxProfiles = new RoundedComboBox();
            btnAddProfile = new PictureButton();
            btnDeleteProfile = new PictureButton();
            buttonColumns = new NumericUpDown();
            buttonRows = new NumericUpDown();
            lblColumns = new Label();
            lblRows = new Label();
            lblSpacing = new Label();
            buttonSpacing = new NumericUpDown();
            lblCornerRadius = new Label();
            cornerRadius = new NumericUpDown();
            checkButtonBackground = new CheckBox();
            btnEditProfile = new PictureButton();
            panel1 = new Panel();
            checkQrAndNetwork = new CheckBox();
            qrCodeBox = new Panel();
            networkInformationPanel = new FlowLayoutPanel();
            lblNetworkInterfaces = new Label();
            lblPort = new Label();
            folderHeader = new Panel();
            lblFolders = new Label();
            btnCreateFolderHeader = new PictureButton();
            foldersContextMenu.SuspendLayout();
            actionButtonContextMenu.SuspendLayout();
            ((ISupportInitialize)btnAddProfile).BeginInit();
            ((ISupportInitialize)btnDeleteProfile).BeginInit();
            ((ISupportInitialize)buttonColumns).BeginInit();
            ((ISupportInitialize)buttonRows).BeginInit();
            ((ISupportInitialize)buttonSpacing).BeginInit();
            ((ISupportInitialize)cornerRadius).BeginInit();
            ((ISupportInitialize)btnEditProfile).BeginInit();
            panel1.SuspendLayout();
            networkInformationPanel.SuspendLayout();
            folderHeader.SuspendLayout();
            ((ISupportInitialize)btnCreateFolderHeader).BeginInit();
            SuspendLayout();
            // 
            // foldersView
            // 
            foldersView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            foldersView.BackColor = Color.FromArgb(38, 38, 38);
            foldersView.BorderStyle = BorderStyle.None;
            foldersView.Cursor = Cursors.Hand;
            foldersView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            foldersView.Font = new Font("Tahoma", 11.25F);
            foldersView.ForeColor = Color.FromArgb(220, 220, 220);
            foldersView.FullRowSelect = true;
            foldersView.HotTracking = true;
            foldersView.ItemHeight = 30;
            foldersView.Location = new Point(0, 84);
            foldersView.Margin = new Padding(0);
            foldersView.Name = "foldersView";
            foldersView.PathSeparator = "/";
            foldersView.ShowLines = false;
            foldersView.ShowPlusMinus = false;
            foldersView.ShowRootLines = false;
            foldersView.Size = new Size(227, 399);
            foldersView.TabIndex = 6;
            foldersView.DrawNode += FoldersView_DrawNode;
            foldersView.AfterSelect += FoldersView_AfterSelect;
            foldersView.MouseDown += FoldersView_MouseDown;
            // 
            // buttonPanel
            // 
            buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonPanel.Location = new Point(237, 84);
            buttonPanel.Margin = new Padding(10);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(747, 648);
            buttonPanel.TabIndex = 5;
            // 
            // actionButtonContextMenuItemEdit
            // 
            actionButtonContextMenuItemEdit.ForeColor = Color.White;
            actionButtonContextMenuItemEdit.Name = "actionButtonContextMenuItemEdit";
            actionButtonContextMenuItemEdit.Size = new Size(484, 42);
            actionButtonContextMenuItemEdit.Text = "Edit";
            actionButtonContextMenuItemEdit.Click += ContextMenuEditItemClick;
            // 
            // actionButtonContextMenuItemDelete
            // 
            actionButtonContextMenuItemDelete.ForeColor = Color.White;
            actionButtonContextMenuItemDelete.Name = "actionButtonContextMenuItemDelete";
            actionButtonContextMenuItemDelete.Size = new Size(484, 42);
            actionButtonContextMenuItemDelete.Text = "Delete";
            actionButtonContextMenuItemDelete.Click += ContextMenuDeleteItemClick;
            // 
            // foldersContextMenuNew
            // 
            foldersContextMenuNew.ForeColor = Color.White;
            foldersContextMenuNew.Name = "foldersContextMenuNew";
            foldersContextMenuNew.Size = new Size(203, 42);
            foldersContextMenuNew.Text = "New folder";
            foldersContextMenuNew.Click += BtnCreateFolder_Click;
            // 
            // foldersContextMenuEdit
            // 
            foldersContextMenuEdit.ForeColor = Color.White;
            foldersContextMenuEdit.Name = "foldersContextMenuEdit";
            foldersContextMenuEdit.Size = new Size(203, 42);
            foldersContextMenuEdit.Text = "Edit";
            foldersContextMenuEdit.Click += BtnRenameFolder_Click;
            // 
            // foldersContextMenuDelete
            // 
            foldersContextMenuDelete.ForeColor = Color.White;
            foldersContextMenuDelete.Name = "foldersContextMenuDelete";
            foldersContextMenuDelete.Size = new Size(203, 42);
            foldersContextMenuDelete.Text = "Delete";
            foldersContextMenuDelete.Click += BtnDeleteFolder_Click;
            // 
            // foldersContextMenu
            // 
            foldersContextMenu.BackColor = Color.FromArgb(50, 50, 50);
            foldersContextMenu.Font = new Font("Tahoma", 14.25F);
            foldersContextMenu.ImageScalingSize = new Size(24, 24);
            foldersContextMenu.Items.AddRange(new ToolStripItem[] { foldersContextMenuNew, foldersContextMenuEdit, foldersContextMenuDelete });
            foldersContextMenu.Name = "foldersContextMenu";
            foldersContextMenu.ShowImageMargin = false;
            foldersContextMenu.Size = new Size(204, 130);
            // 
            // actionButtonContextMenu
            // 
            actionButtonContextMenu.BackColor = Color.FromArgb(50, 50, 50);
            actionButtonContextMenu.Font = new Font("Tahoma", 14.25F);
            actionButtonContextMenu.ImageScalingSize = new Size(24, 24);
            actionButtonContextMenu.Items.AddRange(new ToolStripItem[] { actionButtonContextMenuItemEdit, toolStripSeparator2, actionButtonContextMenuItemSimulatePress, actionButtonContextMenuItemSimulateRelease, actionButtonContextMenuItemSimulateLongPress, actionButtonContextMenuItemSimulateLongPressRelease, toolStripSeparator3, actionButtonContextMenuItemCopy, actionButtonContextMenuItemPaste, toolStripSeparator1, actionButtonContextMenuItemDelete });
            actionButtonContextMenu.Name = "actionButtonContextMenu";
            actionButtonContextMenu.ShowImageMargin = false;
            actionButtonContextMenu.Size = new Size(485, 358);
            actionButtonContextMenu.Opening += ActionButtonContextMenuOpened;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(481, 6);
            // 
            // actionButtonContextMenuItemSimulatePress
            // 
            actionButtonContextMenuItemSimulatePress.ForeColor = Color.White;
            actionButtonContextMenuItemSimulatePress.Name = "actionButtonContextMenuItemSimulatePress";
            actionButtonContextMenuItemSimulatePress.Size = new Size(484, 42);
            actionButtonContextMenuItemSimulatePress.Text = "Simulate \"On press\"";
            actionButtonContextMenuItemSimulatePress.Click += ActionButtonContextMenuItemSimulatePress_Click;
            // 
            // actionButtonContextMenuItemSimulateRelease
            // 
            actionButtonContextMenuItemSimulateRelease.ForeColor = Color.White;
            actionButtonContextMenuItemSimulateRelease.Name = "actionButtonContextMenuItemSimulateRelease";
            actionButtonContextMenuItemSimulateRelease.Size = new Size(484, 42);
            actionButtonContextMenuItemSimulateRelease.Text = "Simulate \"On release\"";
            actionButtonContextMenuItemSimulateRelease.Click += ActionButtonContextMenuItemSimulateRelease_Click;
            // 
            // actionButtonContextMenuItemSimulateLongPress
            // 
            actionButtonContextMenuItemSimulateLongPress.ForeColor = Color.White;
            actionButtonContextMenuItemSimulateLongPress.Name = "actionButtonContextMenuItemSimulateLongPress";
            actionButtonContextMenuItemSimulateLongPress.Size = new Size(484, 42);
            actionButtonContextMenuItemSimulateLongPress.Text = "Simulate \"On long press\"";
            actionButtonContextMenuItemSimulateLongPress.Click += ActionButtonContextMenuItemSimulateLongPress_Click;
            // 
            // actionButtonContextMenuItemSimulateLongPressRelease
            // 
            actionButtonContextMenuItemSimulateLongPressRelease.ForeColor = Color.White;
            actionButtonContextMenuItemSimulateLongPressRelease.Name = "actionButtonContextMenuItemSimulateLongPressRelease";
            actionButtonContextMenuItemSimulateLongPressRelease.Size = new Size(484, 42);
            actionButtonContextMenuItemSimulateLongPressRelease.Text = "Simulate \"On long press release\"";
            actionButtonContextMenuItemSimulateLongPressRelease.Click += ActionButtonContextMenuItemSimulateLongPressRelease_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(481, 6);
            // 
            // actionButtonContextMenuItemCopy
            // 
            actionButtonContextMenuItemCopy.ForeColor = Color.White;
            actionButtonContextMenuItemCopy.Name = "actionButtonContextMenuItemCopy";
            actionButtonContextMenuItemCopy.Size = new Size(484, 42);
            actionButtonContextMenuItemCopy.Text = "Copy";
            actionButtonContextMenuItemCopy.Click += ActionButtonContextMenuItemCopy_Click;
            // 
            // actionButtonContextMenuItemPaste
            // 
            actionButtonContextMenuItemPaste.Enabled = false;
            actionButtonContextMenuItemPaste.ForeColor = Color.White;
            actionButtonContextMenuItemPaste.Name = "actionButtonContextMenuItemPaste";
            actionButtonContextMenuItemPaste.Size = new Size(484, 42);
            actionButtonContextMenuItemPaste.Text = "Paste";
            actionButtonContextMenuItemPaste.Click += ActionButtonContextMenuItemPaste_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(481, 6);
            // 
            // boxProfiles
            // 
            boxProfiles.Anchor = AnchorStyles.Top;
            boxProfiles.BackColor = Color.FromArgb(62, 62, 62);
            boxProfiles.Cursor = Cursors.Hand;
            boxProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
            boxProfiles.Font = new Font("Tahoma", 11.25F);
            boxProfiles.ForeColor = Color.White;
            boxProfiles.Icon = null;
            boxProfiles.Location = new Point(341, 17);
            boxProfiles.Margin = new Padding(0);
            boxProfiles.Name = "boxProfiles";
            boxProfiles.SelectedIndex = -1;
            boxProfiles.SelectedItem = null;
            boxProfiles.Size = new Size(285, 36);
            boxProfiles.TabIndex = 10;
            boxProfiles.SelectedIndexChanged += BoxProfiles_SelectedIndexChanged;
            // 
            // btnAddProfile
            // 
            btnAddProfile.Anchor = AnchorStyles.Top;
            btnAddProfile.BackColor = Color.Transparent;
            btnAddProfile.BackgroundImage = Resources.Create_Normal;
            btnAddProfile.BackgroundImageLayout = ImageLayout.Stretch;
            btnAddProfile.Cursor = Cursors.Hand;
            btnAddProfile.Font = new Font("Tahoma", 9.75F);
            btnAddProfile.ForeColor = Color.White;
            btnAddProfile.HoverImage = Resources.Create_Hover;
            btnAddProfile.Location = new Point(291, 15);
            btnAddProfile.Name = "btnAddProfile";
            btnAddProfile.Size = new Size(38, 38);
            btnAddProfile.TabIndex = 12;
            btnAddProfile.TabStop = false;
            btnAddProfile.Text = "+";
            btnAddProfile.Click += BtnAddProfile_Click;
            // 
            // btnDeleteProfile
            // 
            btnDeleteProfile.Anchor = AnchorStyles.Top;
            btnDeleteProfile.BackColor = Color.Transparent;
            btnDeleteProfile.BackgroundImage = Resources.Delete_Normal;
            btnDeleteProfile.BackgroundImageLayout = ImageLayout.Stretch;
            btnDeleteProfile.Cursor = Cursors.Hand;
            btnDeleteProfile.Font = new Font("Tahoma", 9.75F);
            btnDeleteProfile.ForeColor = Color.White;
            btnDeleteProfile.HoverImage = Resources.Delete_Hover;
            btnDeleteProfile.Location = new Point(692, 15);
            btnDeleteProfile.Name = "btnDeleteProfile";
            btnDeleteProfile.Size = new Size(38, 38);
            btnDeleteProfile.TabIndex = 13;
            btnDeleteProfile.TabStop = false;
            btnDeleteProfile.Click += BtnDeleteProfile_Click;
            // 
            // buttonColumns
            // 
            buttonColumns.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonColumns.BackColor = Color.FromArgb(50, 50, 50);
            buttonColumns.BorderStyle = BorderStyle.None;
            buttonColumns.Font = new Font("Tahoma", 11.25F);
            buttonColumns.ForeColor = Color.White;
            buttonColumns.Location = new Point(15, 549);
            buttonColumns.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            buttonColumns.Name = "buttonColumns";
            buttonColumns.Size = new Size(66, 31);
            buttonColumns.TabIndex = 14;
            buttonColumns.Value = new decimal(new int[] { 5, 0, 0, 0 });
            buttonColumns.ValueChanged += ButtonSettingsChanged;
            // 
            // buttonRows
            // 
            buttonRows.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonRows.BackColor = Color.FromArgb(50, 50, 50);
            buttonRows.BorderStyle = BorderStyle.None;
            buttonRows.Font = new Font("Tahoma", 11.25F);
            buttonRows.ForeColor = Color.White;
            buttonRows.Location = new Point(15, 505);
            buttonRows.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            buttonRows.Name = "buttonRows";
            buttonRows.Size = new Size(66, 31);
            buttonRows.TabIndex = 15;
            buttonRows.Value = new decimal(new int[] { 3, 0, 0, 0 });
            buttonRows.ValueChanged += ButtonSettingsChanged;
            // 
            // lblColumns
            // 
            lblColumns.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblColumns.AutoSize = true;
            lblColumns.BackColor = Color.Transparent;
            lblColumns.Font = new Font("Tahoma", 11.25F);
            lblColumns.ForeColor = Color.White;
            lblColumns.Location = new Point(92, 547);
            lblColumns.Name = "lblColumns";
            lblColumns.Size = new Size(98, 28);
            lblColumns.TabIndex = 16;
            lblColumns.Text = "Columns";
            lblColumns.TextAlign = ContentAlignment.MiddleLeft;
            lblColumns.UseMnemonic = false;
            // 
            // lblRows
            // 
            lblRows.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblRows.AutoSize = true;
            lblRows.BackColor = Color.Transparent;
            lblRows.Font = new Font("Tahoma", 11.25F);
            lblRows.ForeColor = Color.White;
            lblRows.Location = new Point(92, 505);
            lblRows.Name = "lblRows";
            lblRows.Size = new Size(65, 28);
            lblRows.TabIndex = 17;
            lblRows.Text = "Rows";
            lblRows.TextAlign = ContentAlignment.MiddleLeft;
            lblRows.UseMnemonic = false;
            // 
            // lblSpacing
            // 
            lblSpacing.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSpacing.AutoSize = true;
            lblSpacing.BackColor = Color.Transparent;
            lblSpacing.Font = new Font("Tahoma", 11.25F);
            lblSpacing.ForeColor = Color.White;
            lblSpacing.Location = new Point(92, 591);
            lblSpacing.Name = "lblSpacing";
            lblSpacing.Size = new Size(92, 28);
            lblSpacing.TabIndex = 19;
            lblSpacing.Text = "Spacing";
            lblSpacing.TextAlign = ContentAlignment.MiddleLeft;
            lblSpacing.UseMnemonic = false;
            // 
            // buttonSpacing
            // 
            buttonSpacing.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonSpacing.BackColor = Color.FromArgb(50, 50, 50);
            buttonSpacing.BorderStyle = BorderStyle.None;
            buttonSpacing.Font = new Font("Tahoma", 11.25F);
            buttonSpacing.ForeColor = Color.White;
            buttonSpacing.Location = new Point(15, 593);
            buttonSpacing.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            buttonSpacing.Name = "buttonSpacing";
            buttonSpacing.Size = new Size(66, 31);
            buttonSpacing.TabIndex = 18;
            buttonSpacing.Value = new decimal(new int[] { 10, 0, 0, 0 });
            buttonSpacing.ValueChanged += ButtonSettingsChanged;
            // 
            // lblCornerRadius
            // 
            lblCornerRadius.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCornerRadius.AutoSize = true;
            lblCornerRadius.BackColor = Color.Transparent;
            lblCornerRadius.Font = new Font("Tahoma", 11.25F);
            lblCornerRadius.ForeColor = Color.White;
            lblCornerRadius.Location = new Point(92, 636);
            lblCornerRadius.Name = "lblCornerRadius";
            lblCornerRadius.Size = new Size(147, 28);
            lblCornerRadius.TabIndex = 21;
            lblCornerRadius.Text = "Corner radius";
            lblCornerRadius.TextAlign = ContentAlignment.MiddleLeft;
            lblCornerRadius.UseMnemonic = false;
            // 
            // cornerRadius
            // 
            cornerRadius.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cornerRadius.BackColor = Color.FromArgb(50, 50, 50);
            cornerRadius.BorderStyle = BorderStyle.None;
            cornerRadius.Font = new Font("Tahoma", 11.25F);
            cornerRadius.ForeColor = Color.White;
            cornerRadius.Location = new Point(15, 636);
            cornerRadius.Name = "cornerRadius";
            cornerRadius.Size = new Size(66, 31);
            cornerRadius.TabIndex = 20;
            cornerRadius.Value = new decimal(new int[] { 40, 0, 0, 0 });
            cornerRadius.ValueChanged += ButtonSettingsChanged;
            // 
            // checkButtonBackground
            // 
            checkButtonBackground.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkButtonBackground.AutoSize = true;
            checkButtonBackground.BackColor = Color.Transparent;
            checkButtonBackground.Checked = true;
            checkButtonBackground.CheckState = CheckState.Checked;
            checkButtonBackground.Font = new Font("Tahoma", 11.25F);
            checkButtonBackground.ForeColor = Color.White;
            checkButtonBackground.Location = new Point(9, 696);
            checkButtonBackground.Name = "checkButtonBackground";
            checkButtonBackground.Size = new Size(233, 32);
            checkButtonBackground.TabIndex = 22;
            checkButtonBackground.Text = "Button Background";
            checkButtonBackground.UseMnemonic = false;
            checkButtonBackground.UseVisualStyleBackColor = false;
            checkButtonBackground.CheckedChanged += ButtonSettingsChanged;
            // 
            // btnEditProfile
            // 
            btnEditProfile.Anchor = AnchorStyles.Top;
            btnEditProfile.BackColor = Color.Transparent;
            btnEditProfile.BackgroundImage = Resources.Edit_Normal;
            btnEditProfile.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditProfile.Cursor = Cursors.Hand;
            btnEditProfile.Font = new Font("Tahoma", 9.75F);
            btnEditProfile.ForeColor = Color.White;
            btnEditProfile.HoverImage = Resources.Edit_Hover;
            btnEditProfile.Location = new Point(642, 15);
            btnEditProfile.Name = "btnEditProfile";
            btnEditProfile.Size = new Size(38, 38);
            btnEditProfile.TabIndex = 23;
            btnEditProfile.TabStop = false;
            btnEditProfile.Click += BtnEditProfile_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.FromArgb(38, 38, 38);
            panel1.Controls.Add(checkQrAndNetwork);
            panel1.Controls.Add(boxProfiles);
            panel1.Controls.Add(btnAddProfile);
            panel1.Controls.Add(btnDeleteProfile);
            panel1.Controls.Add(btnEditProfile);
            panel1.Location = new Point(237, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1002, 74);
            panel1.TabIndex = 24;
            // 
            // checkQrAndNetwork
            // 
            checkQrAndNetwork.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkQrAndNetwork.Appearance = Appearance.Button;
            checkQrAndNetwork.BackgroundImage = Resources.qrcode_custom;
            checkQrAndNetwork.BackgroundImageLayout = ImageLayout.Stretch;
            checkQrAndNetwork.Checked = true;
            checkQrAndNetwork.CheckState = CheckState.Checked;
            checkQrAndNetwork.Cursor = Cursors.Hand;
            checkQrAndNetwork.FlatAppearance.BorderColor = Color.White;
            checkQrAndNetwork.FlatAppearance.BorderSize = 0;
            checkQrAndNetwork.FlatAppearance.CheckedBackColor = Color.Transparent;
            checkQrAndNetwork.FlatStyle = FlatStyle.Flat;
            checkQrAndNetwork.ForeColor = Color.Black;
            checkQrAndNetwork.Location = new Point(928, 7);
            checkQrAndNetwork.Margin = new Padding(5);
            checkQrAndNetwork.Name = "checkQrAndNetwork";
            checkQrAndNetwork.Size = new Size(66, 59);
            checkQrAndNetwork.TabIndex = 45;
            checkQrAndNetwork.Text = "\r\n";
            checkQrAndNetwork.UseVisualStyleBackColor = true;
            checkQrAndNetwork.CheckedChanged += checkQrAndNetwork_CheckedChanged;
            // 
            // qrCodeBox
            // 
            qrCodeBox.BackgroundImageLayout = ImageLayout.Stretch;
            qrCodeBox.Location = new Point(5, 5);
            qrCodeBox.Margin = new Padding(5);
            qrCodeBox.Name = "qrCodeBox";
            qrCodeBox.Size = new Size(240, 240);
            qrCodeBox.TabIndex = 44;
            // 
            // networkInformationPanel
            // 
            networkInformationPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            networkInformationPanel.BackColor = Color.FromArgb(38, 38, 38);
            networkInformationPanel.Controls.Add(qrCodeBox);
            networkInformationPanel.Controls.Add(lblNetworkInterfaces);
            networkInformationPanel.Controls.Add(lblPort);
            networkInformationPanel.FlowDirection = FlowDirection.TopDown;
            networkInformationPanel.Location = new Point(994, 84);
            networkInformationPanel.Margin = new Padding(0);
            networkInformationPanel.Name = "networkInformationPanel";
            networkInformationPanel.Size = new Size(250, 651);
            networkInformationPanel.TabIndex = 46;
            // 
            // lblNetworkInterfaces
            // 
            lblNetworkInterfaces.AutoSize = true;
            lblNetworkInterfaces.ForeColor = Color.White;
            lblNetworkInterfaces.Location = new Point(5, 255);
            lblNetworkInterfaces.Margin = new Padding(5);
            lblNetworkInterfaces.Name = "lblNetworkInterfaces";
            lblNetworkInterfaces.Padding = new Padding(0, 5, 0, 5);
            lblNetworkInterfaces.Size = new Size(162, 54);
            lblNetworkInterfaces.TabIndex = 46;
            lblNetworkInterfaces.Text = "Network interfaces:\r\n- 127.0.0.1";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.ForeColor = Color.White;
            lblPort.Location = new Point(5, 319);
            lblPort.Margin = new Padding(5);
            lblPort.Name = "lblPort";
            lblPort.Padding = new Padding(0, 5, 0, 5);
            lblPort.Size = new Size(94, 32);
            lblPort.TabIndex = 47;
            lblPort.Text = "Port: 8191";
            // 
            // folderHeader
            // 
            folderHeader.BackColor = Color.FromArgb(38, 38, 38);
            folderHeader.Controls.Add(lblFolders);
            folderHeader.Controls.Add(btnCreateFolderHeader);
            folderHeader.Location = new Point(0, 0);
            folderHeader.Margin = new Padding(0);
            folderHeader.Name = "folderHeader";
            folderHeader.Size = new Size(227, 74);
            folderHeader.TabIndex = 50;
            // 
            // lblFolders
            // 
            lblFolders.BackColor = Color.Transparent;
            lblFolders.Font = new Font("Tahoma", 9F);
            lblFolders.ForeColor = Color.FromArgb(160, 160, 160);
            lblFolders.Location = new Point(12, 21);
            lblFolders.Name = "lblFolders";
            lblFolders.Size = new Size(160, 36);
            lblFolders.TabIndex = 0;
            lblFolders.Text = "Folders";
            lblFolders.TextAlign = ContentAlignment.MiddleLeft;
            lblFolders.UseMnemonic = false;
            // 
            // btnCreateFolderHeader
            // 
            btnCreateFolderHeader.BackColor = Color.Transparent;
            btnCreateFolderHeader.BackgroundImage = Resources.Create_Normal;
            btnCreateFolderHeader.BackgroundImageLayout = ImageLayout.Stretch;
            btnCreateFolderHeader.Cursor = Cursors.Hand;
            btnCreateFolderHeader.Font = new Font("Tahoma", 9.75F);
            btnCreateFolderHeader.ForeColor = Color.White;
            btnCreateFolderHeader.HoverImage = Resources.Create_Hover;
            btnCreateFolderHeader.Location = new Point(178, 22);
            btnCreateFolderHeader.Name = "btnCreateFolderHeader";
            btnCreateFolderHeader.Size = new Size(36, 36);
            btnCreateFolderHeader.TabIndex = 1;
            btnCreateFolderHeader.TabStop = false;
            btnCreateFolderHeader.Click += BtnCreateFolder_Click;
            // 
            // DeckView
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            BackColor = Color.FromArgb(28, 28, 28);
            Controls.Add(networkInformationPanel);
            Controls.Add(panel1);
            Controls.Add(checkButtonBackground);
            Controls.Add(lblCornerRadius);
            Controls.Add(cornerRadius);
            Controls.Add(lblSpacing);
            Controls.Add(buttonSpacing);
            Controls.Add(lblRows);
            Controls.Add(lblColumns);
            Controls.Add(buttonRows);
            Controls.Add(buttonColumns);
            Controls.Add(folderHeader);
            Controls.Add(foldersView);
            Controls.Add(buttonPanel);
            Font = new Font("Tahoma", 9F);
            Name = "DeckView";
            Size = new Size(1244, 735);
            Load += Deck_Load;
            foldersContextMenu.ResumeLayout(false);
            actionButtonContextMenu.ResumeLayout(false);
            ((ISupportInitialize)btnAddProfile).EndInit();
            ((ISupportInitialize)btnDeleteProfile).EndInit();
            ((ISupportInitialize)buttonColumns).EndInit();
            ((ISupportInitialize)buttonRows).EndInit();
            ((ISupportInitialize)buttonSpacing).EndInit();
            ((ISupportInitialize)cornerRadius).EndInit();
            ((ISupportInitialize)btnEditProfile).EndInit();
            panel1.ResumeLayout(false);
            networkInformationPanel.ResumeLayout(false);
            networkInformationPanel.PerformLayout();
            folderHeader.ResumeLayout(false);
            ((ISupportInitialize)btnCreateFolderHeader).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TreeView foldersView;
        private BufferedPanel buttonPanel;
        private ContextMenuStrip actionButtonContextMenu;
        private ToolStripMenuItem actionButtonContextMenuItemEdit;
        private ToolStripMenuItem actionButtonContextMenuItemDelete;
        private ContextMenuStrip foldersContextMenu;
        private ToolStripMenuItem foldersContextMenuEdit;
        private ToolStripMenuItem foldersContextMenuDelete;
        private ToolStripMenuItem foldersContextMenuNew;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem actionButtonContextMenuItemCopy;
        private ToolStripMenuItem actionButtonContextMenuItemPaste;
        private ToolStripSeparator toolStripSeparator1;
        private RoundedComboBox boxProfiles;
        private PictureButton btnAddProfile;
        private PictureButton btnDeleteProfile;
        private NumericUpDown buttonColumns;
        private NumericUpDown buttonRows;
        private Label lblColumns;
        private Label lblRows;
        private Label lblSpacing;
        private NumericUpDown buttonSpacing;
        private Label lblCornerRadius;
        private NumericUpDown cornerRadius;
        private CheckBox checkButtonBackground;
        private PictureButton btnEditProfile;
        private Panel panel1;
        private ToolStripMenuItem actionButtonContextMenuItemSimulatePress;
        private ToolStripMenuItem actionButtonContextMenuItemSimulateRelease;
        private ToolStripMenuItem actionButtonContextMenuItemSimulateLongPress;
        private ToolStripMenuItem actionButtonContextMenuItemSimulateLongPressRelease;
        private ToolStripSeparator toolStripSeparator3;
        private Panel qrCodeBox;
#pragma warning disable CS0169 // 设计器保留字段，当前未使用
        private ButtonPrimary buttonPrimary1;
        private ButtonPrimary buttonPrimary2;
        private ButtonPrimary buttonPrimary3;
#pragma warning restore CS0169
        private FlowLayoutPanel networkInformationPanel;
        private CheckBox checkQrAndNetwork;
        private Label lblNetworkInterfaces;
        private Label lblPort;
        private Panel folderHeader;
        private Label lblFolders;
        private PictureButton btnCreateFolderHeader;
    }
}
