using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.Utils;
using SuchByte.MacroDeck.Variables;

namespace SuchByte.MacroDeck.GUI.CustomControls.Variables;

public partial class VariableItem : RoundedUserControl
{
    /// <summary>
    /// 设计时的原始宽度，用于布局重算的基准。
    /// </summary>
    private static readonly int DesignWidth = 840;
    /// <summary>
    /// 设计时 lblName 的原始宽度。
    /// </summary>
    private static readonly int DesignLblNameWidth = 223;

    public Variable Variable;

    public VariableItem(Variable variable)
    {
        Variable = variable;
        InitializeComponent();
    }

    public new void Update()
    {
        Invoke(() =>
        {
            lblName.Text = Variable.Name;
            lblType.Text = Variable.Type;
            lblValue.Text = Variable.Value;
            lblCreator.Text = Variable.Creator;
        });
    }

    /// <summary>
    /// 根据当前字体和父容器宽度自适应调整控件布局。
    /// 将自身宽度扩展到填满父容器，额外空间分配给名称列，
    /// 并按字体高度调整行高，确保文字不被截断。
    /// </summary>
    private void AdjustLayout()
    {
        // 获取父容器可用宽度
        var parentWidth = Parent?.ClientSize.Width ?? Width;
        var availableWidth = parentWidth - Margin.Horizontal;

        // 扩展自身宽度以填充容器
        if (Width < availableWidth && availableWidth > 0)
        {
            Width = availableWidth;
        }

        // 将额外宽度分配给 lblName（名称列），右移后续控件
        var extraWidth = Width - DesignWidth;
        if (extraWidth > 0)
        {
            lblName.Width = DesignLblNameWidth + extraWidth;
            lblType.Left = 237 + extraWidth;
            lblValue.Left = 357 + extraWidth;
            lblCreator.Left = 621 + extraWidth;
            btnEdit.Left = 797 + extraWidth;
        }

        // 按当前字体高度调整行高（文字高度 + 上下边距）
        var rowHeight = LayoutHelper.GetTextHeight(this) + 10;
        if (Height < rowHeight)
        {
            Height = rowHeight;
        }

        // 垂直居中各子控件
        var centerY = (Height - lblName.Height) / 2;
        lblName.Top = centerY;
        lblType.Top = (Height - lblType.Height) / 2;
        lblValue.Top = (Height - lblValue.Height) / 2;
        lblCreator.Top = (Height - lblCreator.Height) / 2;
        btnEdit.Top = (Height - btnEdit.Height) / 2;
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        using var variableDialog = new VariableDialog(Variable);
        variableDialog.ShowDialog();
    }

    private void VariableItem_Load(object sender, EventArgs e)
    {
        FontManager.Apply(this);
        AdjustLayout();
        Update();
    }
}
