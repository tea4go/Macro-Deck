using System.Runtime.InteropServices;

namespace SuchByte.MacroDeck.GUI.CustomControls;

internal class BorderlessComboBox : System.Windows.Forms.ComboBox
{
    private const int WM_PAINT = 0xF;
    private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;

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

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

    public BorderlessComboBox()
    {
        // 切换到 OwnerDrawFixed 后 ItemHeight 才能真正控制控件与下拉项的高度，
        // 否则 Win32 COMBOBOX 会按字体把高度强制压回到字号决定的值。
        DrawMode = DrawMode.OwnerDrawFixed;
    }

    /// <summary>
    /// DropDown 模式下，COMBOBOX 内部有独立的 EDIT 子窗口显示文字，
    /// 系统按字体高度放置并贴顶。当外层因 ItemHeight 被撑高时，文字看起来偏上。
    /// 这里拿到 EDIT 句柄，把它在控件中垂直居中。
    ///
    /// 注意：系统会在 WM_SIZE / 字体变化后重置 EDIT 位置，所以需要用 BeginInvoke
    /// 推迟到当前消息处理完成后再 MoveWindow，避免被系统覆盖。
    /// </summary>
    private void CenterEditChild()
    {
        if (DropDownStyle != ComboBoxStyle.DropDown || !IsHandleCreated)
        {
            return;
        }

        var info = new COMBOBOXINFO { cbSize = Marshal.SizeOf<COMBOBOXINFO>() };
        if (!GetComboBoxInfo(Handle, ref info) || info.hwndItem == IntPtr.Zero)
        {
            return;
        }

        var editWidth = info.rcItem.Right - info.rcItem.Left;
        var editHeight = info.rcItem.Bottom - info.rcItem.Top;
        if (editWidth <= 0 || editHeight <= 0)
        {
            return;
        }

        var newTop = Math.Max(0, (Height - editHeight) / 2);
        if (newTop == info.rcItem.Top)
        {
            return;
        }

        MoveWindow(info.hwndItem, info.rcItem.Left, newTop, editWidth, editHeight, true);
    }

    private void DeferCenterEditChild()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        // 延迟到当前消息循环之后，避开系统 WM_SIZE 内部对 EDIT 的重置
        BeginInvoke(new Action(CenterEditChild));
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        DeferCenterEditChild();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        DeferCenterEditChild();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        DeferCenterEditChild();
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0)
        {
            base.OnDrawItem(e);
            return;
        }

        // ComboBoxEdit 标志表示这是闭合状态下显示选中项，绝不用 Selected 蓝色高亮；
        // 仅在下拉列表里、且系统标了 Selected 时才用蓝色。
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

        // 闭合显示时直接在 e.Graphics 上画下拉箭头，避免依赖 WndProc post-paint
        // 因为 Win32 在 BeginPaint/EndPaint 内已用 themed 控件画了系统箭头，
        // 我们要在同一个绘制周期里盖住它，绘出深色背景 + 白色箭头。
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

    private const int WM_CTLCOLOREDIT = 0x0133;

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        // EDIT 子窗口请求绘制时也校正一次位置，最稳妥的兜底时机
        if (m.Msg == WM_CTLCOLOREDIT)
        {
            CenterEditChild();
        }

        if (m.Msg == WM_PAINT)
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                // 整片覆盖客户区，挡掉系统在 hot-track / focus 时画的白色高亮
                using (var bg = new SolidBrush(BackColor))
                {
                    g.FillRectangle(bg, 0, 0, Width, Height);
                }

                // 与 Parent 同色画边框线，消除系统残留的 1px 白边
                using (var p = new Pen(Parent?.BackColor ?? BackColor, 1))
                {
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }

                if (!Enabled)
                {
                    using var p = new Pen(Parent?.BackColor ?? BackColor, 5);
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }

                // 闭合显示时，重绘当前选中项的文字（背景已经被上面填过）
                if (SelectedIndex >= 0)
                {
                    var text = Items[SelectedIndex]?.ToString() ?? string.Empty;
                    var fg = Enabled ? Color.White : Color.FromArgb(95, 95, 95);
                    var textSize = TextRenderer.MeasureText(text, Font);
                    var y = Math.Max(0, (Height - textSize.Height) / 2);
                    TextRenderer.DrawText(g, text, Font, new Point(2, y), fg);
                }
                else if (!string.IsNullOrEmpty(Text))
                {
                    // DropDown 模式下可能有非列表项的文字
                    var fg = Enabled ? Color.White : Color.FromArgb(95, 95, 95);
                    var textSize = TextRenderer.MeasureText(Text, Font);
                    var y = Math.Max(0, (Height - textSize.Height) / 2);
                    TextRenderer.DrawText(g, Text, Font, new Point(2, y), fg);
                }

                // 绘制自定义下拉箭头
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
}
