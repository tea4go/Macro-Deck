using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;
using SuchByte.MacroDeck.Variables;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.GUI.Dialogs;

public partial class VariableDialog : DialogForm
{
    public Variable Variable;

    private readonly bool _protected;
    private readonly bool _edit;

    private Size _originalClientSize;

    public VariableDialog(Variable variable = null)
    {
        InitializeComponent();
        lblType.Text = LanguageManager.Strings.Type;
        lblName.Text = LanguageManager.Strings.Name;
        lblValue.Text = LanguageManager.Strings.Value;
        btnDelete.Text = LanguageManager.Strings.DeleteVariable;
        btnOk.Text = LanguageManager.Strings.Ok;
        if (variable == null)
        {
            Variable = new Variable();
            variableName.Enabled = true;
            _edit = false;
        }
        else
        {
            Variable = variable;
            variableName.Enabled = false;
            _edit = true;
        }

        _protected = Variable.Creator != "User";
        variableType.Enabled = !_protected;
        variableValue.Enabled = !_protected;
    }

    /// <summary>
    /// 重写 OnLoad，在基类字体应用完成后动态重算所有控件的垂直布局。
    /// 基类 DialogForm.OnLoad 已调用 FontManager.Apply(this) 替换字体，
    /// 此时 RoundedTextBox/RoundedComboBox 已自动更新高度，Label 已 AutoSize，
    /// 需要以当前控件实际高度为基准、10px 行间距重新排列各行。
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        AdjustLayoutForFont();
    }

    /// <summary>
    /// 根据当前字体动态重算所有控件的垂直布局。
    /// 每行（Type / Name / Value）使用该行 Label 与输入控件的最大高度作为行高，
    /// 行间固定 10px 间距，Label 与输入控件在行内垂直居中。
    /// 底部按钮行同理垂直居中后，根据控件边界自动调整窗体 ClientSize。
    /// </summary>
    private void AdjustLayoutForFont()
    {
        const int rowSpacing = 10;
        const int startY = 10;

        // 计算每行高度：取 Label 与输入控件的较高者
        int row1Height = Math.Max(lblType.Height, variableType.Height);
        int row2Height = Math.Max(lblName.Height, variableName.Height);
        int row3Height = Math.Max(lblValue.Height, variableValue.Height);

        // 第 1 行：Type
        lblType.Top = startY + (row1Height - lblType.Height) / 2;
        variableType.Top = startY + (row1Height - variableType.Height) / 2;

        // 第 2 行：Name
        int row2Y = startY + row1Height + rowSpacing;
        lblName.Top = row2Y + (row2Height - lblName.Height) / 2;
        variableName.Top = row2Y + (row2Height - variableName.Height) / 2;

        // 第 3 行：Value
        int row3Y = row2Y + row2Height + rowSpacing;
        lblValue.Top = row3Y + (row3Height - lblValue.Height) / 2;
        variableValue.Top = row3Y + (row3Height - variableValue.Height) / 2;

        // 底部按钮行
        int bottomY = row3Y + row3Height + rowSpacing;
        int bottomRowHeight = Math.Max(btnDelete.Height, btnOk.Height);
        btnDelete.Top = bottomY + (bottomRowHeight - btnDelete.Height) / 2;
        btnOk.Top = bottomY + (bottomRowHeight - btnOk.Height) / 2;

        // 根据控件实际边界调整窗体 ClientSize
        ClientSize = new Size(
            Math.Max(_originalClientSize.Width, variableValue.Right + 12),
            Math.Max(_originalClientSize.Height, btnOk.Bottom + 12)
        );
    }


    private void VariableName_TextChanged(object sender, EventArgs e)
    {
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
        if (_protected)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        if (!_edit)
        {
            if (variableName.Text.Length == 0)
            {
                variableName.Text = "new_variable";
            }

            var variableCount = VariableManager.ListVariables.Count(v => v.Name == variableName.Text);
            if (variableCount > 0)
            {
                variableName.Text = string.Format(variableName.Text + " _{0}", variableCount);
            }

            Variable.Name = VariableManager.ConvertNameString(variableName.Text);
        }

        Variable.Type = variableType.Text;
        switch (Variable.Type)
        {
            case "Bool":
                Variable.Value = (variableValue.Text.ToLower().Equals("true") ? true : false).ToString();
                break;
            case "Integer":
                int.TryParse(variableValue.Text, out var intVal);
                Variable.Value = intVal.ToString();
                break;
            case "Float":
                float.TryParse(variableValue.Text, out var floatVal);
                Variable.Value = floatVal.ToString();
                break;
            case "String":
                Variable.Value = variableValue.Text;
                break;
        }

        VariableManager.SetValue(Variable.Name, Variable.Value, Variable.VariableType, Variable.Creator);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void VariableDialog_Load(object sender, EventArgs e)
    {
        foreach (var name in Enum.GetNames(typeof(VariableType)))
        {
            variableType.Items.Add(name);
        }

        variableType.Text = Variable.Type;
        variableName.Text = Variable.Name;
        variableValue.Text = Variable.Value;
        CenterToScreen();
    }

    private void BtnDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        using var msgBox = new MessageBox();
        if (msgBox.ShowDialog(LanguageManager.Strings.AreYouSure,
                string.Format(LanguageManager.Strings.VariableXGetsDeleted, Variable.Name),
                MessageBoxButtons.YesNo) ==
            DialogResult.Yes)
        {
            VariableManager.DeleteVariable(Variable.Name);
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    private void variableType_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (variableType.Text)
        {
            case "Bool":
                variableValue.Text = "false";
                break;
            case "Integer":
                variableValue.Text = "0";
                break;
            case "Float":
                variableValue.Text = "0.0";
                break;
            case "String":
                variableValue.Text = "";
                break;
        }
    }
}
