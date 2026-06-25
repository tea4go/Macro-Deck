namespace SuchByte.MacroDeck.GUI.MainWindowViews;

/// <summary>
/// 加载视图，在程序启动或切换页面时显示加载动画。
/// </summary>
public partial class LoadingView : UserControl
{
    /// <summary>
    /// 初始化加载视图，设置停靠方式为填充。
    /// </summary>
    public LoadingView()
    {
        InitializeComponent();
        Dock = DockStyle.Fill;
    }
}
