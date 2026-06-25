using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Enums;
using SuchByte.MacroDeck.Events;
using SuchByte.MacroDeck.Folders;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.GUI.Dialogs;
using Serilog;
using SuchByte.MacroDeck.Icons;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.Services;
using SuchByte.MacroDeck.Utils;
using Clipboard = SuchByte.MacroDeck.Utils.Clipboard;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.GUI.MainWindowViews;

/// <summary>
/// 主面板视图（Deck 视图），是 Macro Deck 的核心操作界面。
/// 负责管理按钮网格布局、文件夹树、配置文件切换、拖放操作、按钮编辑以及上下文菜单等功能。
/// </summary>
public partial class DeckView : UserControl
{
    private static readonly ILogger Logger = Log.ForContext(typeof(DeckView));

    /// <summary>
    /// 当前选中的文件夹。
    /// </summary>
    private MacroDeckFolder _currentFolder;

    /// <summary>
    /// 右键点击时被选中的按钮控件引用。
    /// </summary>
    private Control _buttonClicked;

    private readonly MainWindow _mainWindow;

    /// <summary>
    /// 获取当前选中的文件夹。
    /// </summary>
    public MacroDeckFolder CurrentFolder => _currentFolder;

    /// <summary>
    /// 初始化 Deck 视图，加载当前配置文件的第一个文件夹，初始化网络信息和 QR 码显示。
    /// </summary>
    public DeckView(MainWindow mainWindow)
    {
        InitializeComponent();
        Dock = DockStyle.Fill;
        UpdateTranslation();
        _currentFolder = ProfileManager.CurrentProfile.Folders.FirstOrDefault();
        qrCodeBox.BackgroundImage = QrCodeService.Instance.GetQuickSetupQrCode();
        checkQrAndNetwork.Checked = Settings.Default.ShowQrCodeAndNetwork;
        lblNetworkInterfaces.Text = GetNetworkInterfacesString();
        lblPort.Text = $"{LanguageManager.Strings.Port}: {MacroDeck.Configuration.HostPort}";
        _mainWindow = mainWindow;
        // 为 TreeView 启用双缓冲，减少闪烁
        typeof(TreeView)
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(foldersView, true);
        HandleCreated += DeckView_HandleCreated;
        HandleDestroyed += DeckView_HandleDestroyed;
    }

    /// <summary>
    /// 获取本地网络接口列表字符串，用于在界面显示可用的网络地址。
    /// </summary>
    private string GetNetworkInterfacesString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Network interfaces:");
        foreach (var networkInterface in NetworkUtils.GetNetworkInterfaces())
        {
            stringBuilder.AppendLine($"- {networkInterface}");
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 控件句柄销毁时，注销主窗口大小变更事件。
    /// </summary>
    private void DeckView_HandleDestroyed(object? sender, EventArgs e)
    {
        _mainWindow.ResizeEnd -= MainWindow_ResizeEnd;
        _mainWindow.FormWindowStateChanged -= MainWindow_FormWindowStateChanged;
    }

    /// <summary>
    /// 控件句柄创建时，注册主窗口大小变更事件以同步刷新按钮布局。
    /// </summary>
    private void DeckView_HandleCreated(object? sender, EventArgs e)
    {
        _mainWindow.ResizeEnd += MainWindow_ResizeEnd;
        _mainWindow.FormWindowStateChanged += MainWindow_FormWindowStateChanged;
    }

    /// <summary>
    /// 主窗口状态变更（最大化/还原等）时刷新按钮布局。
    /// </summary>
    private void MainWindow_FormWindowStateChanged(object? sender, EventArgs e)
    {
        UpdateButtons();
    }

    /// <summary>
    /// 主窗口大小调整结束时刷新按钮布局。
    /// </summary>
    private void MainWindow_ResizeEnd(object sender, EventArgs e)
    {
        UpdateButtons();
    }

    /// <summary>
    /// 更新界面文本翻译，将所有 UI 元素的文本设置为当前语言。
    /// </summary>
    public void UpdateTranslation()
    {
        Name = LanguageManager.Strings.DeckTitle;
        lblColumns.Text = LanguageManager.Strings.Columns;
        lblRows.Text = LanguageManager.Strings.Rows;
        lblSpacing.Text = LanguageManager.Strings.Spacing;
        lblCornerRadius.Text = LanguageManager.Strings.CornerRadius;
        checkButtonBackground.Text = LanguageManager.Strings.ButtonBackGround;
        lblFolders.Text = LanguageManager.Strings.Folders;
        foldersContextMenuNew.Text = LanguageManager.Strings.Create;
        foldersContextMenuEdit.Text = LanguageManager.Strings.Edit;
        foldersContextMenuDelete.Text = LanguageManager.Strings.Delete;
        actionButtonContextMenuItemEdit.Text = LanguageManager.Strings.Edit;
        actionButtonContextMenuItemCopy.Text = LanguageManager.Strings.Copy;
        actionButtonContextMenuItemPaste.Text = LanguageManager.Strings.Paste;
        actionButtonContextMenuItemDelete.Text = LanguageManager.Strings.Delete;
        actionButtonContextMenuItemSimulatePress.Text = LanguageManager.Strings.SimulateOnPress;
        actionButtonContextMenuItemSimulateRelease.Text = LanguageManager.Strings.SimulateOnRelease;
        actionButtonContextMenuItemSimulateLongPress.Text = LanguageManager.Strings.SimulateOnLongPress;
        actionButtonContextMenuItemSimulateLongPressRelease.Text = LanguageManager.Strings.SimulateOnLongPressRelease;
    }

    /// <summary>
    /// 刷新文件夹树视图，使用栈遍历方式构建完整的文件夹层级结构。
    /// </summary>
    public void UpdateFolders()
    {
        foldersView.Nodes.Clear();

        var stack = new Stack<TreeNode>();
        var rootDirectory = ProfileManager.CurrentProfile.Folders.FirstOrDefault();
        var node = new TreeNode(rootDirectory.DisplayName) { Tag = rootDirectory };
        stack.Push(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            var directoryInfo = (MacroDeckFolder)currentNode.Tag;
            foreach (var directoryId in directoryInfo.Childs.ToList())
            {
                try
                {
                    var directory = ProfileManager.FindFolderById(directoryId, ProfileManager.CurrentProfile);
                    var childDirectoryNode = new TreeNode(directory.DisplayName) { Tag = directory };
                    Invoke(new Action(() => currentNode.Nodes.Add(childDirectoryNode)));
                    stack.Push(childDirectoryNode);
                }
                catch
                {
                }
            }
        }

        foldersView.Nodes.Add(node);
        foldersView.ExpandAll();
    }


    /// <summary>
    /// 刷新按钮布局。根据当前配置文件的列数、行数、间距和圆角半径计算每个按钮的大小和位置。
    /// 窗口最小化时跳过布局计算（面板尺寸为零会导致计算异常）。
    /// </summary>
    /// <param name="clear">是否在刷新前清空现有按钮控件并注销事件。</param>
    public void UpdateButtons(bool clear = false)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        // 窗口最小化时按钮面板会收缩到零尺寸，此时跳过布局计算；
        // 窗口恢复后会再次触发此方法。
        if (buttonPanel.Width <= 0 || buttonPanel.Height <= 0)
        {
            return;
        }

        if (clear)
        {
            // 清理现有按钮：注销事件并释放控件
            foreach (RoundedButton roundedButton in buttonPanel.Controls)
            {
                var actionButton = _currentFolder.ActionButtons.Find(aB =>
                    aB.Position_X == roundedButton.Column && aB.Position_Y == roundedButton.Row);
                if (actionButton != null)
                {
                    actionButton.StateChanged -= ButtonStateChanged;
                    actionButton.IconChanged -= ActionButton_IconChanged;
                    actionButton.LabelOff.LabelBase64Changed -= LabelChanged;
                    actionButton.LabelOn.LabelBase64Changed -= LabelChanged;
                }

                roundedButton.MouseDown -= ActionButton_Down;
                roundedButton.DragDrop -= Button_DragDrop;
                roundedButton.DragEnter -= Button_DragEnter;
                Invoke(roundedButton.Dispose);
            }

            Invoke(buttonPanel.Controls.Clear);
        }

        var buttonSize = 100;
        int rows = ProfileManager.CurrentProfile.Rows,
            columns = ProfileManager.CurrentProfile.Columns,
            spacing = ProfileManager.CurrentProfile.ButtonSpacing; // 来自配置文件的行列和间距
        int width = buttonPanel.Width, height = buttonPanel.Height; // 来自面板的实际尺寸
        int buttonSizeX, buttonSizeY;

        // 根据可用空间和行列数计算按钮尺寸
        buttonSizeX = width / columns;
        buttonSizeY = height / rows;
        buttonSize = Math.Min(buttonSizeX, buttonSizeY) - spacing;


        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                LoadButton(row, column, buttonSize);
            }
        }
    }

    /// <summary>
    /// 按钮标签 Base64 内容变更时，更新对应按钮图标。
    /// </summary>
    private void LabelChanged(object sender, EventArgs e)
    {
        var actionButton = _currentFolder.ActionButtons.Find(aB =>
            (aB.LabelOff != null && aB.LabelOff.Equals(sender)) || (aB.LabelOn != null && aB.LabelOn.Equals(sender)));
        UpdateButtonIcon(actionButton);
    }

    /// <summary>
    /// 按钮状态（开/关）切换时，更新对应按钮图标。
    /// </summary>
    private void ButtonStateChanged(object sender, EventArgs e)
    {
        var actionButton = (ActionButton.ActionButton)sender;
        UpdateButtonIcon(actionButton);
    }

    /// <summary>
    /// 更新按钮的背景图标和前景标签图片。
    /// 根据按钮当前状态（开/关）分别设置不同的图标、标签和背景色。
    /// </summary>
    /// <param name="actionButton">要更新图标的动作按钮数据。</param>
    /// <param name="button">对应的圆角按钮控件，为 null 时自动查找。</param>
    private void UpdateButtonIcon(ActionButton.ActionButton actionButton, RoundedButton button = null)
    {
        if (IsDisposed || !IsHandleCreated || actionButton == null)
        {
            return;
        }

        if (InvokeRequired)
        {
            Invoke(() => UpdateButtonIcon(actionButton, button));
            return;
        }

        button ??= buttonPanel.Controls.Cast<RoundedButton>()
            .Where(x => x.Row.Equals(actionButton.Position_Y) && x.Column.Equals(actionButton.Position_X))
            .FirstOrDefault();

        // 按钮为"开"状态时使用 On 系列的图标、标签和背景色
        if (actionButton.State)
        {
            if (actionButton.LabelOn != null && !string.IsNullOrWhiteSpace(actionButton.LabelOn.LabelBase64))
            {
                var labelImage = Base64.GetImageFromBase64(actionButton.LabelOn.LabelBase64);
                button.ForegroundImage = labelImage;
            }

            if (!string.IsNullOrWhiteSpace(actionButton.IconOn))
            {
                var icon = IconManager.GetIconByString(actionButton.IconOn);
                if (icon != null)
                {
                    button.BackgroundImage?.Dispose();
                    button.BackgroundImage = icon.IconImage;
                }
            }

            button.BackColor = ProfileManager.CurrentProfile.ButtonBackground
                ? actionButton.BackColorOn
                : Color.Transparent;
        }
        else
        {
            if (actionButton.LabelOff != null && !string.IsNullOrWhiteSpace(actionButton.LabelOff.LabelBase64))
            {
                var labelImage = Base64.GetImageFromBase64(actionButton.LabelOff.LabelBase64);
                button.ForegroundImage = labelImage;
            }

            if (!string.IsNullOrWhiteSpace(actionButton.IconOff))
            {
                var icon = IconManager.GetIconByString(actionButton.IconOff);
                if (icon != null)
                {
                    button.BackgroundImage?.Dispose();
                    button.BackgroundImage = icon.IconImage;
                }
            }

            button.BackColor = ProfileManager.CurrentProfile.ButtonBackground
                ? actionButton.BackColorOff
                : Color.Transparent;
        }
    }

    /// <summary>
    /// 加载或更新指定行列位置上的按钮控件。
    /// 如果按钮已存在则更新其大小和位置；如果不存在则创建新的圆角按钮并绑定事件。
    /// </summary>
    private void LoadButton(int row, int column, int buttonSize)
    {
        var button = buttonPanel.Controls.Cast<RoundedButton>().Where(x => x.Row.Equals(row) && x.Column.Equals(column))
            .FirstOrDefault();

        if (button == null)
        {
            button = new RoundedButton
            {
                BackgroundImageLayout = ImageLayout.Stretch,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Row = row,
                Column = column,
                ForeColor = Color.White
            };
            button.MouseDown += ActionButton_Down;
            button.DragDrop += Button_DragDrop;
            button.DragEnter += Button_DragEnter;
            button.AllowDrop = true;
            button.Cursor = Cursors.Hand;
        }

        button.ShowKeyboardHotkeyIndicator = false;
        button.KeyboardHotkeyIndicatorText = string.Empty;
        button.Width = buttonSize;
        button.Height = buttonSize;
        button.Radius = ProfileManager.CurrentProfile.ButtonRadius;
        button.Location = new Point(column * buttonSize + column * ProfileManager.CurrentProfile.ButtonSpacing,
            row * buttonSize + row * ProfileManager.CurrentProfile.ButtonSpacing);
        button.BackColor = ProfileManager.CurrentProfile.ButtonBackground
            ? Color.FromArgb(65, 65, 65)
            : Color.Transparent;

        // 清理旧的图片资源
        if (button.BackgroundImage != null)
        {
            button.BackgroundImage.Dispose();
            button.BackgroundImage = null;
        }

        if (button.ForegroundImage != null)
        {
            button.ForegroundImage = null;
        }

        if (button.Image != null)
        {
            button.Image = null;
        }

        var actionButton = _currentFolder.ActionButtons.Find(aB => aB.Position_X == column && aB.Position_Y == row);

        if (actionButton != null)
        {
            // 先移除已有的事件处理器，防止重复绑定
            actionButton.StateChanged -= ButtonStateChanged;
            actionButton.LabelOff.LabelBase64Changed -= LabelChanged;
            actionButton.LabelOn.LabelBase64Changed -= LabelChanged;
            actionButton.IconChanged -= ActionButton_IconChanged;
            // 绑定事件处理器
            actionButton.StateChanged += ButtonStateChanged;
            actionButton.LabelOff.LabelBase64Changed += LabelChanged;
            actionButton.LabelOn.LabelBase64Changed += LabelChanged;
            actionButton.IconChanged += ActionButton_IconChanged;

            button.ShowKeyboardHotkeyIndicator = actionButton.KeyCode != Keys.None;
            if (button.ShowKeyboardHotkeyIndicator)
            {
                button.KeyboardHotkeyIndicatorText
                    = actionButton.ModifierKeyCodes.ToString().Replace("Control", "CTRL").Replace("None", string.Empty)
                        .Replace(", ", "  + ") +
                    (!actionButton.ModifierKeyCodes.ToString().Equals("None") ? " + " : string.Empty) +
                    actionButton.KeyCode;
            }

            UpdateButtonIcon(actionButton, button);
        }

        if (!buttonPanel.Controls.Contains(button))
        {
            Invoke(() => buttonPanel.Controls.Add(button));
        }
    }

    /// <summary>
    /// 按钮图标变更时，如果该按钮属于当前文件夹则刷新显示。
    /// </summary>
    private void ActionButton_IconChanged(object sender, EventArgs e)
    {
        var actionButton = sender as ActionButton.ActionButton;
        if (actionButton != null)
        {
            if (_currentFolder.ActionButtons.Contains(actionButton))
            {
                UpdateButtonIcon(actionButton);
            }
        }
    }

    /// <summary>
    /// 拖放操作完成时，交换源按钮和目标按钮的位置，并保存配置。
    /// 同时更新被交换按钮的所有动作和事件监听器的所属按钮引用。
    /// </summary>
    private void Button_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) != null)
        {
        }
        else
        {
            Task.Run(() =>
            {
                var draggedButton = (RoundedButton)e.Data.GetData(typeof(RoundedButton));
                var targetButton = (RoundedButton)sender;

                var targetActionButton
                    = ProfileManager.FindActionButton(_currentFolder, targetButton.Row, targetButton.Column);
                var draggedActionButton
                    = ProfileManager.FindActionButton(_currentFolder, draggedButton.Row, draggedButton.Column);

                if (draggedActionButton == null)
                {
                    return;
                }

                // 将被拖拽的按钮移动到目标位置
                var newActionButton = draggedActionButton;
                newActionButton.Position_Y = targetButton.Row;
                newActionButton.Position_X = targetButton.Column;

                _currentFolder.ActionButtons.Remove(draggedActionButton);
                _currentFolder.ActionButtons.Add(newActionButton);

                // 如果目标位置已有按钮，则将其交换到源位置
                if (targetActionButton != null)
                {
                    var newTargetButton = targetActionButton;
                    newTargetButton.Position_Y = draggedButton.Row;
                    newTargetButton.Position_X = draggedButton.Column;
                    _currentFolder.ActionButtons.Remove(targetActionButton);
                    _currentFolder.ActionButtons.Add(newTargetButton);

                    // 更新被交换按钮的所有动作引用
                    foreach (var pluginAction in newTargetButton.Actions)
                    {
                        pluginAction.SetActionButton(newTargetButton);
                    }

                    foreach (var pluginAction in newTargetButton.ActionsRelease)
                    {
                        pluginAction.SetActionButton(newTargetButton);
                    }

                    foreach (var pluginAction in newTargetButton.ActionsLongPress)
                    {
                        pluginAction.SetActionButton(newTargetButton);
                    }

                    foreach (var pluginAction in newTargetButton.ActionsLongPressRelease)
                    {
                        pluginAction.SetActionButton(newTargetButton);
                    }

                    newTargetButton.EventListeners ??= new List<EventListener>();

                    foreach (var eventListener in newTargetButton.EventListeners)
                    {
                        foreach (var pluginAction in eventListener.Actions)
                        {
                            pluginAction.SetActionButton(newTargetButton);
                        }
                    }
                }
            });

            ProfileManager.Save();
            UpdateButtons();
            MacroDeckServer.UpdateFolder(_currentFolder);
        }
    }

    /// <summary>
    /// 拖放进入按钮时，判断拖放数据是否为按钮以决定效果类型（移动或复制）。
    /// </summary>
    private void Button_DragEnter(object sender, DragEventArgs e)
    {
        var button = (RoundedButton)e.Data.GetData(typeof(RoundedButton));
        if (button != null)
        {
            if (button.Equals((RoundedButton)sender))
            {
                return;
            }

            e.Effect = DragDropEffects.Move;
        }
        else
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    /// <summary>
    /// 按钮鼠标按下事件：左键双击打开编辑器，左键单击开始拖放，右键显示上下文菜单。
    /// </summary>
    private void ActionButton_Down(object sender, MouseEventArgs e)
    {
        var button = (RoundedButton)sender;

        _buttonClicked = button;
        switch (e.Button)
        {
            case MouseButtons.Left:
                if (e.Clicks == 2)
                {
                    // 双击打开按钮编辑器
                    OpenButtonEditor(button);
                }
                else if (e.Clicks == 1)
                {
                    // 单击开始拖放
                    button.DoDragDrop(button, DragDropEffects.Move);
                }

                break;
            case MouseButtons.Right:
                actionButtonContextMenu.Show(button, button.PointToClient(MousePosition));
                break;
        }
    }

    /// <summary>
    /// 上下文菜单"编辑"项：打开按钮编辑器。
    /// </summary>
    public void ContextMenuEditItemClick(object sender, EventArgs e)
    {
        OpenButtonEditor((RoundedButton)_buttonClicked);
    }

    /// <summary>
    /// 上下文菜单"删除"项：移除按钮并保存配置。
    /// </summary>
    public void ContextMenuDeleteItemClick(object sender, EventArgs e)
    {
        var row = ((RoundedButton)_buttonClicked).Row;
        var col = ((RoundedButton)_buttonClicked).Column;
        var actionButton = _currentFolder.ActionButtons.Find(aB => aB.Position_X == col && aB.Position_Y == row);
        if (actionButton != null)
        {
            actionButton.Dispose();
            _currentFolder.ActionButtons.Remove(actionButton);
            ProfileManager.Save();
            UpdateButtons();
            MacroDeckServer.UpdateFolder(_currentFolder);
        }
    }

    /// <summary>
    /// 打开按钮编辑器对话框。如果目标位置尚无按钮则创建新的空按钮。
    /// </summary>
    public void OpenButtonEditor(RoundedButton button)
    {
        var row = button.Row;
        var column = button.Column;

        var actionButton = _currentFolder.ActionButtons.Find(aB => aB.Position_X == column && aB.Position_Y == row);
        actionButton ??= new ActionButton.ActionButton
        {
            Actions = new List<PluginAction>(),
            EventListeners = new List<EventListener>(),
            Position_Y = row,
            Position_X = column,
            IconOff = "",
            IconOn = "",
            LabelOff = new ButtonLabel(),
            LabelOn = new ButtonLabel()
        };


        using var buttonEditor = new ButtonEditor(actionButton, _currentFolder);
        buttonEditor.ShowDialog();
        ProfileManager.Save();
        UpdateButtons();
    }

    /// <summary>
    /// 切换当前显示的文件夹，并刷新按钮布局。
    /// </summary>
    public void SetFolder(MacroDeckFolder macroDeckFolder)
    {
        if (macroDeckFolder == null || !ProfileManager.CurrentProfile.Folders.Contains(macroDeckFolder))
        {
            return;
        }

        _currentFolder = macroDeckFolder;
        UpdateButtons();
    }

    /// <summary>
    /// 文件夹树节点选中后，切换到对应的文件夹视图。
    /// </summary>
    private void FoldersView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        SetFolder(ProfileManager.FindFolderByDisplayName(foldersView.SelectedNode.Text, ProfileManager.CurrentProfile));
    }

    /// <summary>
    /// 文件夹树鼠标按下事件：右键显示上下文菜单，左键点击展开/折叠三角区域时切换节点。
    /// </summary>
    private void FoldersView_MouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Right:
                foldersView.SelectedNode = foldersView.GetNodeAt(e.X, e.Y);
                if (foldersView.SelectedNode == null)
                {
                    foldersView.SelectedNode = foldersView.Nodes[0];
                }

                foldersContextMenu.Show(foldersView, foldersView.PointToClient(MousePosition));
                break;
            case MouseButtons.Left:
                var node = foldersView.GetNodeAt(e.X, e.Y);
                if (node != null && node.Nodes.Count > 0)
                {
                    // 判断是否点击在展开/折叠三角图标区域内
                    var triangleStart = node.Level * foldersView.Indent + 6;
                    if (e.X >= triangleStart && e.X <= triangleStart + 16)
                    {
                        node.Toggle();
                    }
                }

                break;
        }
    }

    /// <summary>
    /// 自定义绘制文件夹树节点，支持深色主题的选中/悬停高亮和展开三角图标。
    /// </summary>
    private void FoldersView_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
        if (e.Bounds.Width == 0 || e.Bounds.Height == 0)
        {
            e.DrawDefault = true;
            return;
        }

        e.DrawDefault = false;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var selected = (e.State & TreeNodeStates.Selected) != 0;
        var hot = (e.State & TreeNodeStates.Hot) != 0;

        // 根据状态选择背景色
        var rowRect = new Rectangle(0, e.Bounds.Y, foldersView.Width, e.Bounds.Height);
        var rowBg = selected ? Colors.AccentColor : hot ? Colors.Surface3 : Colors.Surface;
        using (var bgBrush = new SolidBrush(rowBg))
        {
            g.FillRectangle(bgBrush, rowRect);
        }

        var indentX = e.Node.Level * foldersView.Indent + 6;
        if (e.Node.Nodes.Count > 0)
        {
            var triColor = selected ? Color.White : Color.FromArgb(160, 160, 160);
            DrawExpandTriangle(g, indentX, e.Bounds.Y, e.Bounds.Height, e.Node.IsExpanded, triColor);
        }

        var textColor = selected ? Color.White : Color.FromArgb(220, 220, 220);
        var textRect = new Rectangle(indentX + 20, e.Bounds.Y, foldersView.Width - indentX - 24, e.Bounds.Height);
        TextRenderer.DrawText(g,
            e.Node.Text,
            foldersView.Font,
            textRect,
            textColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }

    /// <summary>
    /// 绘制文件夹树的展开/折叠三角图标。展开时为向下的三角，折叠时为向右的三角。
    /// </summary>
    private static void DrawExpandTriangle(Graphics g, int x, int nodeY, int nodeH, bool expanded, Color color)
    {
        int cx = x + 6, cy = nodeY + nodeH / 2;
        using var brush = new SolidBrush(color);
        if (expanded)
        {
            g.FillPolygon(brush,
                new Point[]
                {
                    new(cx - 5, cy - 2),
                    new(cx + 5, cy - 2),
                    new(cx, cy + 4)
                });
        }
        else
        {
            g.FillPolygon(brush,
                new Point[]
                {
                    new(cx - 2, cy - 5),
                    new(cx + 4, cy),
                    new(cx - 2, cy + 5)
                });
        }
    }

    /// <summary>
    /// Deck 视图加载时，异步加载配置文件列表和配置设置。
    /// </summary>
    private void Deck_Load(object sender, EventArgs e)
    {
        Task.Run(() =>
        {
            LoadProfiles();
            LoadProfileSettings();
            UpdateButtons();
        });
    }

    /// <summary>
    /// 加载配置文件列表到下拉框。
    /// </summary>
    private void LoadProfiles()
    {
        if (InvokeRequired)
        {
            Invoke(() => LoadProfiles());
            return;
        }

        boxProfiles.Items.Clear();
        foreach (var macroDeckProfile in ProfileManager.Profiles)
        {
            boxProfiles.Items.Add(macroDeckProfile.DisplayName);
        }

        boxProfiles.Text = ProfileManager.CurrentProfile.DisplayName;
    }

    /// <summary>
    /// 点击"返回根文件夹"按钮。
    /// </summary>
    private void BtnGoToRoot_Click(object sender, EventArgs e)
    {
        foldersView.SelectedNode = foldersView.Nodes[0];
    }

    /// <summary>
    /// 点击"创建文件夹"按钮，打开创建文件夹对话框。
    /// </summary>
    private void BtnCreateFolder_Click(object sender, EventArgs e)
    {
        var selectedFolder
            = ProfileManager.FindFolderByDisplayName(foldersView.SelectedNode.Text, ProfileManager.CurrentProfile);
        selectedFolder ??= ProfileManager.CurrentProfile.Folders[0];

        using var addFolder = new AddFolder(selectedFolder);
        if (addFolder.ShowDialog() == DialogResult.OK)
        {
            UpdateFolders();
        }
    }

    /// <summary>
    /// 点击"重命名文件夹"按钮，打开重命名对话框。
    /// </summary>
    private void BtnRenameFolder_Click(object sender, EventArgs e)
    {
        var selectedFolder
            = ProfileManager.FindFolderByDisplayName(foldersView.SelectedNode.Text, ProfileManager.CurrentProfile);
        selectedFolder ??= ProfileManager.CurrentProfile.Folders[0];

        using var addFolder = new AddFolder(selectedFolder, true);
        if (addFolder.ShowDialog() == DialogResult.OK)
        {
            UpdateFolders();
        }
    }

    /// <summary>
    /// 点击"删除文件夹"按钮，弹出确认对话框后删除（根文件夹不可删除）。
    /// </summary>
    private void BtnDeleteFolder_Click(object sender, EventArgs e)
    {
        var selectedFolder
            = ProfileManager.FindFolderByDisplayName(foldersView.SelectedNode.Text, ProfileManager.CurrentProfile);
        if (selectedFolder.Equals(ProfileManager.CurrentProfile.Folders[0]))
        {
            return;
        }

        using var msgBox = new MessageBox();
        if (msgBox.ShowDialog(LanguageManager.Strings.AreYouSure,
                string.Format(LanguageManager.Strings.TheFolderWillBeDeleted, selectedFolder.DisplayName),
                MessageBoxButtons.YesNo) ==
            DialogResult.Yes)
        {
            if (_currentFolder == selectedFolder)
            {
                _currentFolder = ProfileManager.CurrentProfile.Folders[0];
                UpdateButtons();
            }

            ProfileManager.DeleteFolder(selectedFolder, ProfileManager.CurrentProfile);
            UpdateFolders();
        }
    }

    /// <summary>
    /// 上下文菜单"复制"项：将当前按钮复制到剪贴板。
    /// </summary>
    private void ActionButtonContextMenuItemCopy_Click(object sender, EventArgs e)
    {
        var button = (RoundedButton)_buttonClicked;
        var row = button.Row;
        var col = button.Column;
        var actionButton = _currentFolder.ActionButtons.Find(aB => aB.Position_X == col && aB.Position_Y == row);
        if (actionButton != null)
        {
            Clipboard.CopyActionButton(actionButton);
        }
    }

    /// <summary>
    /// 上下文菜单打开时，根据当前按钮状态启用/禁用菜单项。
    /// </summary>
    private void ActionButtonContextMenuOpened(object sender, EventArgs e)
    {
        var button = (RoundedButton)_buttonClicked;
        var row = button.Row;
        var col = button.Column;
        var actionButton = _currentFolder.ActionButtons.Find(aB => aB.Position_X == col && aB.Position_Y == row);
        actionButtonContextMenuItemSimulatePress.Enabled = !(actionButton == null);
        actionButtonContextMenuItemSimulateRelease.Enabled = !(actionButton == null);
        actionButtonContextMenuItemSimulateLongPress.Enabled = !(actionButton == null);
        actionButtonContextMenuItemSimulateLongPressRelease.Enabled = !(actionButton == null);
        actionButtonContextMenuItemCopy.Enabled = !(actionButton == null);
        actionButtonContextMenuItemPaste.Enabled = !(Clipboard.GetActionButtonCopy() == null);
        actionButtonContextMenuItemDelete.Enabled = !(actionButton == null);
    }

    /// <summary>
    /// 上下文菜单"粘贴"项：将剪贴板中的按钮粘贴到当前位置，更新所有动作引用并保存。
    /// </summary>
    private void ActionButtonContextMenuItemPaste_Click(object sender, EventArgs e)
    {
        var button = (RoundedButton)_buttonClicked;
        var row = button.Row;
        var col = button.Column;
        var actionButtonOld = _currentFolder.ActionButtons.Find(aB => aB.Position_X == col && aB.Position_Y == row);

        if (_currentFolder != null && _currentFolder.ActionButtons != null && actionButtonOld != null)
        {
            _currentFolder.ActionButtons.Remove(actionButtonOld);
        }

        if (Clipboard.GetActionButtonCopy() == null)
        {
            return;
        }

        var actionButtonNew = Clipboard.GetActionButtonCopy();
        foreach (var pluginAction in actionButtonNew.Actions)
        {
            pluginAction.SetActionButton(actionButtonNew);
        }

        foreach (var pluginAction in actionButtonNew.ActionsRelease)
        {
            pluginAction.SetActionButton(actionButtonNew);
        }

        foreach (var pluginAction in actionButtonNew.ActionsLongPress)
        {
            pluginAction.SetActionButton(actionButtonNew);
        }

        foreach (var pluginAction in actionButtonNew.ActionsLongPressRelease)
        {
            pluginAction.SetActionButton(actionButtonNew);
        }

        foreach (var eventListener in actionButtonNew.EventListeners)
        {
            foreach (var pluginAction in eventListener.Actions)
            {
                pluginAction.SetActionButton(actionButtonNew);
            }
        }

        actionButtonNew.Position_X = col;
        actionButtonNew.Position_Y = row;

        _currentFolder.ActionButtons.Add(actionButtonNew);

        ProfileManager.Save();
        UpdateButtons();
        UpdateButtonIcon(actionButtonNew);
        MacroDeckServer.UpdateFolder(_currentFolder);
    }

    /// <summary>
    /// 配置文件下拉框选择变更时，切换到选中的配置文件。
    /// </summary>
    private void BoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        var profile = ProfileManager.FindProfileByDisplayName(boxProfiles.Text) ??
            ProfileManager.Profiles.FirstOrDefault();
        SetProfile(profile);
    }

    /// <summary>
    /// 切换到指定的配置文件，保存选择并刷新按钮、文件夹和设置显示。
    /// </summary>
    public void SetProfile(MacroDeckProfile profile)
    {
        Settings.Default.SelectedProfile = profile.ProfileId;
        Settings.Default.Save();
        ProfileManager.CurrentProfile = profile;
        _currentFolder = profile.Folders.FirstOrDefault();
        UpdateButtons(true);
        Invoke(() =>
        {
            boxProfiles.Text = profile.DisplayName;
            UpdateFolders();
            LoadProfileSettings();
        });
    }

    /// <summary>
    /// 加载当前配置文件的按钮设置到界面控件（行数、列数、间距、圆角等）。
    /// 先移除事件防止触发保存，设置完值后重新绑定。
    /// </summary>
    private void LoadProfileSettings()
    {
        if (InvokeRequired)
        {
            Invoke(() => LoadProfileSettings());
            return;
        }

        buttonRows.ValueChanged -= ButtonSettingsChanged;
        buttonColumns.ValueChanged -= ButtonSettingsChanged;
        buttonSpacing.ValueChanged -= ButtonSettingsChanged;
        cornerRadius.ValueChanged -= ButtonSettingsChanged;
        checkButtonBackground.CheckedChanged -= ButtonSettingsChanged;

        buttonRows.Value = ProfileManager.CurrentProfile.Rows;
        buttonColumns.Value = ProfileManager.CurrentProfile.Columns;
        buttonSpacing.Value = ProfileManager.CurrentProfile.ButtonSpacing;
        cornerRadius.Value = ProfileManager.CurrentProfile.ButtonRadius;
        checkButtonBackground.Checked = ProfileManager.CurrentProfile.ButtonBackground;
        buttonRows.ValueChanged += ButtonSettingsChanged;
        buttonColumns.ValueChanged += ButtonSettingsChanged;
        buttonSpacing.ValueChanged += ButtonSettingsChanged;
        cornerRadius.ValueChanged += ButtonSettingsChanged;
        checkButtonBackground.CheckedChanged += ButtonSettingsChanged;

        buttonRows.Enabled = ProfileManager.CurrentProfile.ButtonsCustomizable;
        buttonColumns.Enabled = ProfileManager.CurrentProfile.ButtonsCustomizable;
        buttonSpacing.Enabled = ProfileManager.CurrentProfile.ButtonsCustomizable;
        cornerRadius.Enabled = ProfileManager.CurrentProfile.ButtonsCustomizable;
        checkButtonBackground.Enabled = ProfileManager.CurrentProfile.ButtonsCustomizable;
    }

    /// <summary>
    /// 点击"添加配置文件"按钮，创建新配置文件并切换过去。
    /// </summary>
    private void BtnAddProfile_Click(object sender, EventArgs e)
    {
        using var createProfileDialog = new CreateProfileDialog();
        if (createProfileDialog.ShowDialog() == DialogResult.OK)
        {
            ProfileManager.CurrentProfile = createProfileDialog.Profile;
            LoadProfiles();
            _currentFolder = ProfileManager.CurrentProfile.Folders[0];
            UpdateButtons(true);
            LoadProfileSettings();
            UpdateFolders();
        }
    }

    /// <summary>
    /// 点击"编辑配置文件"按钮，打开编辑对话框。
    /// </summary>
    private void BtnEditProfile_Click(object sender, EventArgs e)
    {
        using var createProfileDialog = new CreateProfileDialog(ProfileManager.CurrentProfile);
        if (createProfileDialog.ShowDialog() == DialogResult.OK)
        {
            LoadProfiles();
        }
    }

    /// <summary>
    /// 点击"删除配置文件"按钮，确认后删除（至少保留一个配置文件）。
    /// </summary>
    private void BtnDeleteProfile_Click(object sender, EventArgs e)
    {
        if (ProfileManager.Profiles.Count < 2)
        {
            return;
        }

        using var msgBox = new MessageBox();
        if (msgBox.ShowDialog(LanguageManager.Strings.AreYouSure,
                string.Format(LanguageManager.Strings.TheProfileWillBeDeleted,
                    ProfileManager.CurrentProfile.DisplayName),
                MessageBoxButtons.YesNo) ==
            DialogResult.Yes)
        {
            ProfileManager.DeleteProfile(ProfileManager.CurrentProfile);
            ProfileManager.CurrentProfile = ProfileManager.Profiles[0];
            LoadProfiles();
            _currentFolder = ProfileManager.CurrentProfile.Folders[0];
            UpdateButtons(true);
            LoadProfileSettings();
            UpdateFolders();
        }
    }

    /// <summary>
    /// 按钮布局设置变更时，保存配置、刷新按钮并同步更新所有使用此配置文件的客户端。
    /// </summary>
    private void ButtonSettingsChanged(object sender, EventArgs e)
    {
        ProfileManager.CurrentProfile.Rows = (int)buttonRows.Value;
        ProfileManager.CurrentProfile.Columns = (int)buttonColumns.Value;
        ProfileManager.CurrentProfile.ButtonSpacing = (int)buttonSpacing.Value;
        ProfileManager.CurrentProfile.ButtonRadius = (int)cornerRadius.Value;
        ProfileManager.CurrentProfile.ButtonBackground = checkButtonBackground.Checked;
        ProfileManager.Save();
        Logger.Information(
            string.Format("Updated profile settings of {0}:", ProfileManager.CurrentProfile.DisplayName) +
            Environment.NewLine +
            string.Format("Rows: {0}", ProfileManager.CurrentProfile.Rows) +
            Environment.NewLine +
            string.Format("Columns: {0}", ProfileManager.CurrentProfile.Columns) +
            Environment.NewLine +
            string.Format("ButtonSpacing: {0}", ProfileManager.CurrentProfile.ButtonSpacing) +
            Environment.NewLine +
            string.Format("ButtonRadius: {0}", ProfileManager.CurrentProfile.ButtonRadius) +
            Environment.NewLine +
            string.Format("ButtonBackground: {0}", ProfileManager.CurrentProfile.ButtonBackground));
        UpdateButtons(true);
        foreach (var macroDeckClient in MacroDeckServer.Clients.FindAll(macroDeckClient =>
            macroDeckClient.Profile.ProfileId.Equals(ProfileManager.CurrentProfile.ProfileId)))
        {
            MacroDeckServer.SetProfile(macroDeckClient, ProfileManager.CurrentProfile);
        }
    }

    /// <summary>
    /// 模拟短按按钮。
    /// </summary>
    private void ActionButtonContextMenuItemSimulatePress_Click(object sender, EventArgs e)
    {
        SimulatePress(ButtonPressType.SHORT);
    }

    /// <summary>
    /// 模拟短按释放按钮。
    /// </summary>
    private void ActionButtonContextMenuItemSimulateRelease_Click(object sender, EventArgs e)
    {
        SimulatePress(ButtonPressType.SHORT_RELEASE);
    }

    /// <summary>
    /// 模拟长按按钮。
    /// </summary>
    private void ActionButtonContextMenuItemSimulateLongPress_Click(object sender, EventArgs e)
    {
        SimulatePress(ButtonPressType.LONG);
    }

    /// <summary>
    /// 模拟长按释放按钮。
    /// </summary>
    private void ActionButtonContextMenuItemSimulateLongPressRelease_Click(object sender, EventArgs e)
    {
        SimulatePress(ButtonPressType.LONG_RELEASE);
    }

    /// <summary>
    /// 以指定按压类型模拟按钮动作，通过服务器执行对应动作。
    /// </summary>
    private void SimulatePress(ButtonPressType buttonPressType)
    {
        var row = ((RoundedButton)_buttonClicked).Row;
        var column = ((RoundedButton)_buttonClicked).Column;
        var actionButton = _currentFolder.ActionButtons.Find(aB => aB.Position_X == column && aB.Position_Y == row);
        MacroDeckServer.Execute(actionButton, "", buttonPressType);
    }

    /// <summary>
    /// 打开捐赠页面。
    /// </summary>
    private void btnDonate_Click(object sender, EventArgs e)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("https://ko-fi.com/manuelmayer")
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }

    /// <summary>
    /// 打开 Discord 社区页面。
    /// </summary>
    private void btnDiscord_Click(object sender, EventArgs e)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("https://discord.macro-deck.app")
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }

    /// <summary>
    /// 切换 QR 码和网络信息面板的显示/隐藏，并调整按钮面板大小。
    /// </summary>
    private void checkQrAndNetwork_CheckedChanged(object sender, EventArgs e)
    {
        if (networkInformationPanel.Visible)
        {
            buttonPanel.Size = new Size(buttonPanel.Width + networkInformationPanel.Width, buttonPanel.Height);
        }
        else
        {
            buttonPanel.Size = new Size(buttonPanel.Width - networkInformationPanel.Width, buttonPanel.Height);
        }

        networkInformationPanel.Visible = checkQrAndNetwork.Checked;
        Settings.Default.ShowQrCodeAndNetwork = checkQrAndNetwork.Checked;
        Settings.Default.Save();

        UpdateButtons();
    }
}
