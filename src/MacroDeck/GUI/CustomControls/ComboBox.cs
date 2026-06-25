using System.Drawing.Drawing2D;

namespace SuchByte.MacroDeck.GUI.CustomControls;

[Obsolete("Use RoundedComboBox instead.")]
public class ComboBox : System.Windows.Forms.ComboBox
{
    private int _borderRadius = 0;
    private bool _regionSet;
    private bool _hover;

    public int BorderRadius
    {
        get => _borderRadius;
        set
        {
            _borderRadius = value;
            Invalidate();
        }
    }

    /// <summary>
    /// 覆盖 Font 属性，在字体变更时自动重算 ComboBox 的最小高度，
    /// 确保下拉框文字在任何字号下都不会被裁剪。
    /// </summary>
    public override Font Font
    {
        get => base.Font;
        set
        {
            base.Font = value;
            UpdateComboHeight();
        }
    }

    /// <summary>
    /// 根据当前字体文字高度 + 上下 padding 重算 ComboBox 的最小高度。
    /// </summary>
    private void UpdateComboHeight()
    {
        var textHeight = TextRenderer.MeasureText("Ay", Font).Height + 1;
        var minHeight = textHeight + 8;
        if (Height < minHeight)
        {
            Height = minHeight;
        }
    }


    public ComboBox()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint,
            true);

        DoubleBuffered = true;
        BackColor = Color.FromArgb(65, 65, 65);
        ForeColor = Color.White;
        Cursor = Cursors.Hand;
        MouseEnter += MouseEnterEvent;
        MouseLeave += MouseLeaveEvent;
        SelectedIndexChanged += SelectedIndexEvent;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateComboHeight();
    }

    private void SelectedIndexEvent(object sender, EventArgs e)
    {
        _hover = false;
        Invalidate();
    }

    private void MouseEnterEvent(object sender, EventArgs e)
    {
        _hover = true;
        Invalidate();
    }

    private void MouseLeaveEvent(object sender, EventArgs e)
    {
        _hover = false;
        Invalidate();
    }

    private GraphicsPath GetFigurePath(Rectangle rect, float radius)
    {
        var path = new GraphicsPath();
        var curveSize = radius * 2F;
        path.StartFigure();
        path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
        path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
        path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
        path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);

        var rectSurface = ClientRectangle;
        var rectText = new Rectangle
        {
            Height = Height,
            Width = Width,
            X = 10
        };
        var smoothSize = 2;
        if (_borderRadius > 2)
        {
            using var pathSurface = GetFigurePath(rectSurface, _borderRadius);
            using var penSurface = new Pen(Parent.BackColor, smoothSize);
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (!_regionSet)
            {
                Region = new Region(pathSurface);
                _regionSet = true; // Preventing future region changes because the event raises on every region change
            }

            pe.Graphics.DrawPath(penSurface, pathSurface);
        }


        var stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };

        using var brush = new SolidBrush(Enabled ? _hover ? Color.White : Color.Silver : Color.FromArgb(95, 95, 95));
        pe.Graphics.DrawString(Text, Font, brush, rectText, stringFormat);
        if (Enabled)
        {
            pe.Graphics.FillPolygon(brush,
                new Point[]
                {
                    new(Width - 10, Height / 2 - 2), new(Width - 20, Height / 2 - 2), new(Width - 15, Height / 2 + 3)
                });
        }
    }
}
