using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SuchByte.MacroDeck.GUI.CustomControls;

[DefaultEvent(nameof(SelectedIndexChanged))]
[DefaultProperty(nameof(Items))]
[DefaultBindingProperty(nameof(Text))]
public partial class RoundedComboBox : UserControl
{
    private int borderRadius = 0;
    private Image icon;


    public event EventHandler SelectedIndexChanged;

    public Image Icon
    {
        get => icon;
        set
        {
            icon = value;
            Padding = icon == null ? new Padding(0, 0, 0, 0) : new Padding(borderlessComboBox1.Height + 3, 0, 0, 0);
            Invalidate();
        }
    }

    public int FindStringExact(string str)
    {
        return borderlessComboBox1.FindStringExact(str);
    }

    public new bool Enabled
    {
        get => borderlessComboBox1.Enabled;
        set => borderlessComboBox1.Enabled = value;
    }

    public void SetAutoCompleteCustomSource(AutoCompleteStringCollection autoCompleteStringCollection)
    {
        borderlessComboBox1.AutoCompleteCustomSource = autoCompleteStringCollection;
    }

    public void SetAutoCompleteMode(AutoCompleteMode autoCompleteMode)
    {
        borderlessComboBox1.AutoCompleteMode = autoCompleteMode;
    }

    public void SetAutoCompleteSource(AutoCompleteSource autoCompleteSource)
    {
        borderlessComboBox1.AutoCompleteSource = autoCompleteSource;
    }


    public ComboBoxStyle DropDownStyle
    {
        get => borderlessComboBox1.DropDownStyle;
        set => borderlessComboBox1.DropDownStyle = value;
    }

    [Browsable(true)] public System.Windows.Forms.ComboBox.ObjectCollection Items => borderlessComboBox1.Items;

    [Browsable(false)]
    public int SelectedIndex
    {
        get => borderlessComboBox1.SelectedIndex;
        set => borderlessComboBox1.SelectedIndex = value;
    }

    [Bindable(true)]
    [Browsable(false)]
    public object SelectedItem
    {
        get => borderlessComboBox1.SelectedItem;
        set => borderlessComboBox1.SelectedItem = value;
    }

    [Bindable(true)]
    public override string Text
    {
        get => borderlessComboBox1.Text;
        set => borderlessComboBox1.Text = value;
    }

    public override Font Font
    {
        get => base.Font;
        set
        {
            base.Font = value;
            borderlessComboBox1.Font = value;
            UpdateControlHeight();
        }
    }

    /// <summary>
    /// 内嵌 ComboBox 的项高（同时决定 DropDownList 模式下的主控件可见高度）。
    /// 修改后外层会自动跟随重新计算高度。
    /// </summary>
    [Category("Appearance")]
    [Description("内嵌 ComboBox 的项高。DropDownList 模式下同时决定主控件高度。")]
    [DefaultValue(22)]
    public int ItemHeight
    {
        get => borderlessComboBox1.ItemHeight;
        set
        {
            borderlessComboBox1.ItemHeight = value;
            UpdateControlHeight();
        }
    }

    public RoundedComboBox()
    {
        InitializeComponent();
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint,
            true);
        Cursor = Cursors.Hand;

        DoubleBuffered = true;
    }


    private GraphicsPath GetFigurePath(Rectangle rect, int radius)
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

    private void UpdateControlHeight()
    {
        Height = borderlessComboBox1.Height + Padding.Top + Padding.Bottom;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (DesignMode)
        {
            UpdateControlHeight();
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateControlHeight();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var graph = e.Graphics;

        if (borderRadius > 1)
        {
            var rectBorderSmooth = ClientRectangle;
            var rectIcon = new Rectangle(ClientRectangle.X + 3,
                ClientRectangle.Y + ClientRectangle.Height / 2 - borderlessComboBox1.Height / 2,
                borderlessComboBox1.Height,
                borderlessComboBox1.Height);

            var smoothSize = 2;


            using var pathBorderSmooth = GetFigurePath(rectBorderSmooth, borderRadius);
            using var penBorderSmooth = new Pen(Parent.BackColor, smoothSize);
            Region = new Region(pathBorderSmooth);
            if (icon != null)
            {
                graph.DrawImage(icon, rectIcon);
            }

            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.DrawPath(penBorderSmooth, pathBorderSmooth);
        }
        else
        {
            Region = new Region(ClientRectangle);
        }
    }


    private void BorderlessComboBox1_MouseLeave(object sender, EventArgs e)
    {
        OnMouseLeave(e);
    }


    private void BorderlessComboBox1_LostFocus(object sender, EventArgs e)
    {
        OnLostFocus(e);
        if (!borderlessComboBox1.Items.Contains(borderlessComboBox1.Text))
        {
            borderlessComboBox1.Items.Add(borderlessComboBox1.Text);
        }
    }

    private void BorderlessComboBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Return)
        {
            if (!borderlessComboBox1.Items.Contains(borderlessComboBox1.Text))
            {
                borderlessComboBox1.Items.Add(borderlessComboBox1.Text);
            }

            e.Handled = true;
        }

        OnKeyPress(e);
    }

    private void BorderlessComboBox1_GotFocus(object sender, EventArgs e)
    {
        OnGotFocus(e);
    }

    private void BorderlessComboBox1_Enter(object sender, EventArgs e)
    {
        OnEnter(e);
    }

    private void BorderlessComboBox1_Click(object sender, EventArgs e)
    {
        OnClick(e);
    }

    private void BorderlessComboBox1_TextChanged(object sender, EventArgs e)
    {
        OnTextChanged(e);
    }

    private void BorderlessComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
    }
}
