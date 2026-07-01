using System.Runtime.InteropServices;

namespace SuchByte.MacroDeck.GUI.CustomControls;

internal class BorderlessComboBox : System.Windows.Forms.ComboBox
{
    private const int WM_PAINT = 0xF;
    private const int WM_CTLCOLOREDIT = 0x0133;
    private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
    private EditSubclass _editSubclass;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct COMBOBOXINFO
    {
        public int cbSize;
        public RECT rcItem;
        public RECT rcButton;
        public int stateButton;
        public IntPtr hwndCombo;
        public IntPtr hwndItem;
        public IntPtr hwndList;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    public BorderlessComboBox()
    {
        DrawMode = DrawMode.OwnerDrawFixed;
    }

    /// <summary>
    /// 在 DropDown 模式下 subclass 内嵌的 EDIT 子窗口，
    /// 拦截它的 WM_PAINT 自绘文字到 EDIT 的垂直中心位置。
    /// 这是绕过 Win32 ComboBox 内嵌 EDIT 限制的唯一可靠方案：
    /// EM_SETRECT 会被 ComboBox 反复重置；MoveWindow EDIT 也会被覆盖。
    /// 只有重写 EDIT 自身的绘制才能完全控制。
    /// </summary>
    private void AttachEditSubclassIfNeeded()
    {
        if (DropDownStyle != ComboBoxStyle.DropDown || !IsHandleCreated)
        {
            return;
        }

        var info = new COMBOBOXINFO { cbSize = Marshal.SizeOf<COMBOBOXINFO>() };
        if (!GetComboBoxInfo(Handle, ref info) || info.hwndItem == IntPtr.Zero || info.hwndItem == Handle)
        {
            return;
        }

        if (_editSubclass != null && _editSubclass.Handle == info.hwndItem)
        {
            return;
        }

        _editSubclass?.ReleaseHandle();
        _editSubclass = new EditSubclass(this);
        _editSubclass.AssignHandle(info.hwndItem);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        // 关闭 ComboBox 自身及内嵌 EDIT 的视觉样式，避免 Win11 themed 在 hover 时
        // 绘制白色 hot-track 高亮、移动 EDIT 位置等行为。
        SetWindowTheme(Handle, string.Empty, string.Empty);
        var info = new COMBOBOXINFO { cbSize = Marshal.SizeOf<COMBOBOXINFO>() };
        if (GetComboBoxInfo(Handle, ref info) && info.hwndItem != IntPtr.Zero && info.hwndItem != Handle)
        {
            SetWindowTheme(info.hwndItem, string.Empty, string.Empty);
        }

        BeginInvoke(new Action(AttachEditSubclassIfNeeded));
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _editSubclass?.ReleaseHandle();
        _editSubclass = null;
        base.OnHandleDestroyed(e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (IsHandleCreated)
        {
            BeginInvoke(new Action(AttachEditSubclassIfNeeded));
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        if (IsHandleCreated)
        {
            BeginInvoke(new Action(AttachEditSubclassIfNeeded));
        }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0)
        {
            base.OnDrawItem(e);
            return;
        }

        var isClosedDisplay = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;
        var isSelected = !isClosedDisplay
                         && (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        using (var bg = new SolidBrush(isSelected ? SystemColors.Highlight : BackColor))
        {
            e.Graphics.FillRectangle(bg, e.Bounds);
        }

        var text = Items[e.Index]?.ToString() ?? string.Empty;
        var fg = Enabled
            ? (isSelected ? SystemColors.HighlightText : Color.White)
            : Color.FromArgb(95, 95, 95);
        var textSize = TextRenderer.MeasureText(text, Font);
        var y = e.Bounds.Y + Math.Max(0, (e.Bounds.Height - textSize.Height) / 2);
        TextRenderer.DrawText(e.Graphics, text, Font, new Point(e.Bounds.X + 2, y), fg);

        if (isClosedDisplay)
        {
            DrawDropDownArrow(e.Graphics);
        }
    }

    private void DrawDropDownArrow(Graphics g)
    {
        var btnLeft = Width - buttonWidth - 5;
        var btnRect = new Rectangle(btnLeft, 0, buttonWidth + 5, Height);
        using (var bg = new SolidBrush(BackColor))
        {
            g.FillRectangle(bg, btnRect);
        }

        using var arrowBrush = new SolidBrush(Enabled ? Color.White : Color.FromArgb(95, 95, 95));
        g.FillPolygon(arrowBrush, new Point[]
        {
            new(Width - 5, Height / 2 - 2),
            new(Width - 15, Height / 2 - 2),
            new(Width - 10, Height / 2 + 3),
        });
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        // EDIT 子窗口可能在多种时机出现，多个 hook 点重试 attach
        if (m.Msg == WM_CTLCOLOREDIT && (_editSubclass == null || !_editSubclass.IsAttached))
        {
            AttachEditSubclassIfNeeded();
        }

        if (m.Msg == WM_PAINT)
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                using (var bg = new SolidBrush(BackColor))
                {
                    g.FillRectangle(bg, 0, 0, Width, Height);
                }

                using (var p = new Pen(Parent?.BackColor ?? BackColor, 1))
                {
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }

                if (!Enabled)
                {
                    using var p = new Pen(Parent?.BackColor ?? BackColor, 5);
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }

                if (DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    string text = null;
                    if (SelectedIndex >= 0)
                    {
                        text = Items[SelectedIndex]?.ToString();
                    }
                    else if (!string.IsNullOrEmpty(Text))
                    {
                        text = Text;
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        var fg = Enabled ? Color.White : Color.FromArgb(95, 95, 95);
                        var textSize = TextRenderer.MeasureText(text, Font);
                        var y = Math.Max(0, (Height - textSize.Height) / 2);
                        TextRenderer.DrawText(g, text, Font, new Point(2, y), fg);
                    }
                }

                using (var brush = new SolidBrush(Enabled ? Color.White : Color.FromArgb(95, 95, 95)))
                {
                    g.FillPolygon(brush,
                        new Point[]
                        {
                            new(Width - 5, Height / 2 - 2),
                            new(Width - 15, Height / 2 - 2),
                            new(Width - 10, Height / 2 + 3),
                        });
                }
            }

            try
            {
                SelectionLength = 0;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Subclass 内嵌 EDIT 子窗口。截获 WM_PAINT 让 base 先画（背景+文字贴顶），
    /// 然后用 BackColor 把整个区域覆盖，再在垂直中心位置自己重画文字。
    /// EDIT 没有光标显示时这是完全的视觉控制；带光标时仍会闪烁，但本场景下 EDIT
    /// 在 valueToCompare 这类下拉框里通常只是显示+选择。
    /// </summary>
    private class EditSubclass : NativeWindow
    {
        private const int WM_PAINT = 0xF;
        private const int WM_ERASEBKGND = 0x14;

        [StructLayout(LayoutKind.Sequential)]
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public int rcPaintLeft, rcPaintTop, rcPaintRight, rcPaintBottom;
            public bool fRestore, fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT ps);
        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        private readonly BorderlessComboBox _owner;
        public bool IsAttached => Handle != IntPtr.Zero;

        public EditSubclass(BorderlessComboBox owner) => _owner = owner;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                // 阻止 EDIT 擦除背景（避免闪烁）
                m.Result = (IntPtr)1;
                return;
            }

            if (m.Msg == WM_PAINT)
            {
                // 完全接管 WM_PAINT，不调 base：
                // base.WndProc 会用 BeginPaint 提交文字到屏幕（贴顶），
                // 之后无法覆盖。必须在同一个 BeginPaint/EndPaint 周期里自绘。
                var ps = new PAINTSTRUCT();
                var hdc = BeginPaint(Handle, out ps);
                if (hdc != IntPtr.Zero)
                {
                    try
                    {
                        using var g = Graphics.FromHdc(hdc);
                        GetClientRect(Handle, out var rc);
                        var bounds = new Rectangle(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top);

                        using (var bg = new SolidBrush(_owner.BackColor))
                        {
                            g.FillRectangle(bg, bounds);
                        }

                        var text = _owner.Text ?? string.Empty;
                        if (!string.IsNullOrEmpty(text))
                        {
                            var fg = _owner.Enabled ? Color.White : Color.FromArgb(95, 95, 95);
                            var textSize = TextRenderer.MeasureText(text, _owner.Font);
                            var y = Math.Max(0, (bounds.Height - textSize.Height) / 2);
                            TextRenderer.DrawText(g, text, _owner.Font, new Point(2, y), fg);
                        }
                    }
                    finally
                    {
                        EndPaint(Handle, ref ps);
                    }
                }
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }
}
