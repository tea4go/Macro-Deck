namespace SuchByte.MacroDeck.GUI.CustomControls;

internal class BorderlessComboBox : System.Windows.Forms.ComboBox
{
    private const int WM_PAINT = 0xF;
    private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;

    public BorderlessComboBox()
    {
        // 切换到 OwnerDrawFixed 后 ItemHeight 才能真正控制控件与下拉项的高度，
        // 否则 Win32 COMBOBOX 会按字体把高度强制压回到字号决定的值。
        DrawMode = DrawMode.OwnerDrawFixed;
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

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == WM_PAINT)
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                // Remove white border
                using (var p = new Pen(Parent.BackColor, 1))
                {
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }

                if (!Enabled)
                {
                    using var p = new Pen(Parent.BackColor, 5);
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }


                // Remove white drop down button
                using (var brush = new SolidBrush(Parent.BackColor))
                {
                    g.FillRectangle(brush, Width - buttonWidth - 5, 0, buttonWidth + 5, Height);
                }

                // Draw custom drop down button
                using (var brush = new SolidBrush(Enabled ? Color.White : Color.FromArgb(95, 95, 95)))
                {
                    g.FillPolygon(brush,
                        new Point[]
                        {
                            new(Width - 5, Height / 2 - 2), new(Width - 15, Height / 2 - 2),
                            new(Width - 10, Height / 2 + 3)
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
