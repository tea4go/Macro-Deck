using System.Drawing.Drawing2D;

namespace SuchByte.MacroDeck.GUI.CustomControls;

/// <summary>
/// 圆角用户控件基类，提供圆角边框绘制功能。
/// 通过重写 OnPaint 方法，利用 GraphicsPath 绘制抗锯齿的圆角区域。
/// </summary>
public partial class RoundedUserControl : UserControl
{
    /// <summary>
    /// 圆角半径，默认为 3 像素。值大于 1 时启用圆角绘制。
    /// </summary>
    private int borderRadius = 3;

    /// <summary>
    /// 初始化圆角用户控件，启用双缓冲以减少闪烁。
    /// </summary>
    public RoundedUserControl()
    {
        InitializeComponent();
        // 启用优化双缓冲，减少重绘时的闪烁
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    /// <summary>
    /// 根据指定矩形区域和圆角半径，构建一个圆角矩形路径。
    /// 路径由四个角的圆弧组成，依次为左上、右上、右下、左下。
    /// </summary>
    /// <param name="rect">目标矩形区域</param>
    /// <param name="radius">圆角半径</param>
    /// <returns>圆角矩形的 GraphicsPath 对象</returns>
    private GraphicsPath GetFigurePath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        // 圆弧的直径大小为半径的两倍
        var curveSize = radius * 2F;

        path.StartFigure();
        // 左上角圆弧：从 180° 开始，逆时针绘制 90°
        path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
        // 右上角圆弧：从 270° 开始，逆时针绘制 90°
        path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
        // 右下角圆弧：从 0° 开始，逆时针绘制 90°
        path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
        // 左下角圆弧：从 90° 开始，逆时针绘制 90°
        path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
        // 闭合路径，形成完整的圆角矩形
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// 重写绘制事件，根据 borderRadius 绘制圆角控件区域。
    /// 当 borderRadius > 1 时，使用 GraphicsPath 设置控件的可见区域为圆角矩形，
    /// 并使用父控件背景色绘制边框以消除锯齿；否则恢复为默认矩形区域。
    /// </summary>
    /// <param name="e">绘制事件参数</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var graph = e.Graphics;

        if (borderRadius > 1)
        {
            var rectBorderSmooth = ClientRectangle;
            // 边框平滑宽度，用于抗锯齿处理
            var smoothSize = 2;
            // 构建圆角路径
            using var pathBorderSmooth = GetFigurePath(rectBorderSmooth, borderRadius);
            // 使用父控件背景色绘制边框，使边缘与背景自然融合
            using var penBorderSmooth = new Pen(Parent.BackColor, smoothSize);
            // 将控件区域裁剪为圆角矩形
            Region = new Region(pathBorderSmooth);
            // 启用抗锯齿，使圆角边缘平滑
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            // 沿路径绘制边框
            graph.DrawPath(penBorderSmooth, pathBorderSmooth);
        }
        else
        {
            // 圆角半径过小，恢复为默认矩形区域
            Region = new Region(ClientRectangle);
        }
    }
}
