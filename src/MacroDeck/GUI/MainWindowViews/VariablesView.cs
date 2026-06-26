using SuchByte.MacroDeck.GUI.CustomControls.Variables;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Models;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Utils;
using SuchByte.MacroDeck.Variables;

namespace SuchByte.MacroDeck.GUI.MainWindowViews;

/// <summary>
/// 变量管理视图，用于查看、创建和筛选系统中的变量。
/// </summary>
public partial class VariablesView : UserControl
{
    /// <summary>
    /// 初始化变量视图，加载界面文本翻译。
    /// </summary>
    public VariablesView()
    {
        InitializeComponent();
        Dock = DockStyle.Fill;
        UpdateTranslation();
    }

    /// <summary>
    /// 更新界面文本翻译，将所有 UI 文本设置为当前语言。
    /// </summary>
    public void UpdateTranslation()
    {
        SuspendLayout();
        Name = LanguageManager.Strings.VariablesTitle;
        lblName.Text = LanguageManager.Strings.Name;
        lblType.Text = LanguageManager.Strings.Type;
        lblValue.Text = LanguageManager.Strings.Value;
        lblCreator.Text = LanguageManager.Strings.Creator;
        btnCreateVariable.Text = LanguageManager.Strings.CreateVariable;
        ResumeLayout();
    }

    /// <summary>
    /// 加载变量创建者筛选器列表，为每个唯一的创建者生成一个复选框。
    /// </summary>
    private void LoadCreators()
    {
        var variableCreators = new List<string>();
        foreach (var variable in VariableManager.ListVariables)
        {
            if (!variableCreators.Contains(variable.Creator))
            {
                variableCreators.Add(variable.Creator);
            }
        }

        var filterModel = VariableViewCreatorFilterModel.Deserialize(Settings.Default.VariableViewSelectedFilter);

        foreach (var creator in variableCreators)
        {
            // 跳过已存在的复选框控件
            if (creatorFilter.Controls.OfType<CheckBox>().Where(x => x.Name.Equals(creator)).Count() > 0)
            {
                continue;
            }

            var creatorCheckBox = new CheckBox
            {

                Checked = !filterModel.HiddenCreators.Contains(creator),
                Text = creator,
                Name = creator,
                AutoSize = false,
                Size = new Size(creatorFilter.Width - 30, 40)
            };
            creatorFilter.Controls.Add(creatorCheckBox);
            creatorCheckBox.CheckedChanged += CreatorCheckBox_CheckedChanged;
        }
    }

    /// <summary>
    /// 创建者筛选复选框状态变更时，更新对应变量的可见性并保存筛选配置。
    /// </summary>
    private void CreatorCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        var checkBox = sender as CheckBox;
        Parallel.ForEach(variablesPanel.Controls.OfType<VariableItem>().Where(x => x.Variable.Creator == checkBox.Name)
                .ToArray(),
            variableItem =>
            {
                if (IsDisposed)
                {
                    return;
                }

                Invoke(new Action(() => variableItem.Visible = checkBox.Checked));
            });

        // 构建并保存筛选模型
        var filterModel = new VariableViewCreatorFilterModel
        {
            HiddenCreators = (from creator in creatorFilter.Controls.OfType<CheckBox>()
                where !creator.Checked
                select creator.Name).ToList()
        };
        Settings.Default.VariableViewSelectedFilter = filterModel.Serialize();
        Settings.Default.Save();
    }

    /// <summary>
    /// 变量页面加载时初始化，加载创建者筛选器和变量列表。
    /// </summary>
    private void VariablesPage_Load(object sender, EventArgs e)
    {
        LoadCreators();
        LoadVariables();
        // 动态创建的 VariableItem 在 LoadVariables 中添加，需补调 FontManager.Apply
        FontManager.Apply(variablesPanel);
        VariableManager.OnVariableChanged += VariableChanged;
        VariableManager.OnVariableRemoved += VariableRemoved;
    }

    /// <summary>
    /// 变量被移除时，从面板中移除对应的 UI 控件。
    /// </summary>
    private void VariableRemoved(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => VariableRemoved(sender, e));
            return;
        }

        var variableName = sender as string;
        var variableItemView = variablesPanel.Controls.OfType<VariableItem>()
            .Where(x => x.Variable.Name.Equals(variableName)).FirstOrDefault();
        if (variableItemView != null)
        {
            variablesPanel.Controls.Remove(variableItemView);
        }
    }

    /// <summary>
    /// 变量值发生变更时，更新或创建对应的 UI 控件。
    /// </summary>
    private void VariableChanged(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => VariableChanged(sender, e));
            return;
        }

        var variable = sender as Variable;
        if (IsDisposed)
        {
            return;
        }

        var variableItemView = variablesPanel.Controls.OfType<VariableItem>()
            .Where(x => x.Variable.Name.Equals(variable.Name)).FirstOrDefault();
        if (variableItemView == null)
        {
            // 新变量，创建控件
            var newVariableItem = new VariableItem(variable);
            variablesPanel.Controls.Add(newVariableItem);
            // 动态创建的 VariableItem 需补调 FontManager.Apply
            FontManager.Apply(newVariableItem);
            LoadCreators();
        }
        else
        {
            // 已有变量，更新显示
            variableItemView.Variable = variable;
            variableItemView.Update();
        }
    }

    /// <summary>
    /// 加载所有变量到面板，并根据当前筛选器设置可见性。
    /// </summary>
    private void LoadVariables()
    {
        variablesPanel.Controls.Clear();
        foreach (var variable in VariableManager.ListVariables)
        {
            if (IsDisposed)
            {
                return;
            }

            var variableItem = new VariableItem(variable)
            {
                Visible = creatorFilter.Controls.OfType<CheckBox>().Where(x => variable.Creator == x.Name)
                    .FirstOrDefault().Checked
            };
            variablesPanel.Controls.Add(variableItem);
        }
    }

    /// <summary>
    /// 点击"创建变量"按钮，打开变量创建对话框。
    /// </summary>
    private void BtnCreateVariable_Click(object sender, EventArgs e)
    {
        using var variableDialog = new VariableDialog();
        if (variableDialog.ShowDialog() == DialogResult.OK)
        {
            VariableManager.InsertVariable(variableDialog.Variable);
        }
    }
}
