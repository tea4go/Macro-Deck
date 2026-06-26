using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.GUI;

/// <summary>
/// 动作配置对话框，用于浏览和选择插件提供的动作。
/// 左侧显示插件列表（支持搜索），右侧显示选中动作的配置面板。
/// </summary>
public partial class ActionConfigurator : DialogForm
{
    /// <summary>
    /// 用户选择或编辑的动作实例。
    /// </summary>
    public PluginAction? Action { get; private set; }

    /// <summary>
    /// 对话框加载时的原始客户端区域大小，用于后续字体变化时的自适应缩放。
    /// </summary>
    private Size _originalClientSize;

    /// <summary>
    /// 初始化 ActionConfigurator 对话框的新实例。
    /// </summary>
    /// <param name="action">要编辑的动作，为 null 表示创建新动作。</param>
    public ActionConfigurator(PluginAction? action = null)
    {
        Action = action;
        InitializeComponent();
        // 初始化界面文本（多语言支持）
        lblSelectToBegin.Text = LanguageManager.Strings.SelectAPluginAndActionToBegin;
        btnApply.Text = LanguageManager.Strings.Ok;
        label2.Text = LanguageManager.Strings.Action;
        pluginSearch.PlaceHolderText = LanguageManager.Strings.Search;
        Shown += OnShown;
    }

    /// <summary>
    /// 窗体加载完成后自适应布局，确保所有控件尺寸与当前字体匹配。
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustAllLabelHeights(this);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
    }

    /// <summary>
    /// 对话框首次显示后执行插件列表加载、字体应用及布局适配。
    /// 同时处理已有动作的预选中逻辑。
    /// </summary>
    private void OnShown(object? sender, EventArgs e)
    {
        Application.DoEvents();
        // 添加所有插件到列表
        AddPlugins();
        // 应用全局字体设置
        FontManager.Apply(this);

        foreach (Control control in pluginsList.Controls)
        {
            // 宽度自适应：填满容器宽度（扣除控件自身的水平边距）
            control.Width = pluginsList.ClientSize.Width - control.Margin.Horizontal - 24;

            if (control is ActionConfiguratorPluginItem pluginItem)
                pluginItem.AdjustLayout();
            else if (control is ActionConfiguratorActionItem actionItem)
                actionItem.AdjustLayout();
        }

        // 触发 FlowLayoutPanel 重新计算滚动布局
        pluginsList.PerformLayout();

        // 如果是编辑已有动作（非新建），自动定位并选中对应动作
        if (Action is null) { return; }

        foreach (var plugin in from plugin in PluginManager.Plugins.Values
            from macroDeckAction in plugin.Actions.Where(macroDeckAction =>
                macroDeckAction.GetType() == Action.GetType())
            select plugin)
        {
            SetExpand(plugin, true);
            foreach (Control item in pluginsList.Controls)
            {
                if (item is not ActionConfiguratorActionItem actionItem) continue;
                if (actionItem.PluginAction.GetType() == Action.GetType())
                {
                    ActionConfiguratorActionItem_MouseClick(actionItem,
                        new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                }
            }
        }
    }

    /// <summary>
    /// 根据搜索文本筛选插件列表和动作列表。
    /// 匹配规则的插件和动作将保持可见，不匹配的隐藏。
    /// </summary>
    /// <param name="filter">搜索过滤关键词。</param>
    private void Filter(string filter)
    {
        // 搜索文本长度大于 1 时启用过滤
        if (pluginSearch.Text.Length > 1)
        {
            // 根据插件名称过滤
            foreach (Control item in pluginsList.Controls)
            {
                if (!(item is ActionConfiguratorPluginItem)) continue;
                item.Visible = StringSearch.StringContains((item as ActionConfiguratorPluginItem).Plugin.Name, filter);
            }

            // 根据动作名称过滤，匹配到的动作所在插件自动展开
            foreach (Control item in pluginsList.Controls)
            {
                if (!(item is ActionConfiguratorActionItem)) continue;
                item.Visible = StringSearch.StringContains((item as ActionConfiguratorActionItem).PluginAction.Name, filter);
                if (item.Visible) SetExpand((item as ActionConfiguratorActionItem).Plugin, true);
            }
        }
        else
        {
            // 搜索文本过短时，恢复全部显示并折叠
            foreach (Control item in pluginsList.Controls)
            {
                if (!(item is ActionConfiguratorPluginItem)) continue;
                item.Visible = true;
                SetExpand(item as ActionConfiguratorPluginItem, false);
            }
        }
    }

    /// <summary>
    /// 遍历所有已注册的插件，为每个包含动作的插件创建对应的条目控件。
    /// 插件条目下方会追加该插件的所有动作子条目。
    /// </summary>
    private void AddPlugins()
    {
        // 清理已有条目的事件绑定
        foreach (Control item in pluginsList.Controls)
        {
            if (item is ActionConfiguratorActionItem)
                item.MouseClick -= ActionConfiguratorActionItem_MouseClick;
            else if (item is ActionConfiguratorPluginItem)
                item.MouseClick -= ActionConfiguratorPluginItem_MouseClick;
        }
        pluginsList.Controls.Clear();

        // 遍历所有插件并添加对应的条目
        foreach (var plugin in PluginManager.Plugins.Values)
        {
            if (plugin.Actions.Count > 0)
            {
                // 创建插件条目（折叠头部）
                var actionConfiguratorPluginItem = new ActionConfiguratorPluginItem(plugin);
                actionConfiguratorPluginItem.MouseClick += ActionConfiguratorPluginItem_MouseClick;
                pluginsList.Controls.Add(actionConfiguratorPluginItem);
                // 为每个动作创建子条目（默认隐藏）
                foreach (var action in plugin.Actions)
                {
                    var actionConfiguratorActionItem
                        = new ActionConfiguratorActionItem(plugin, PluginManager.GetNewActionInstance(action))
                        { Visible = false };
                    actionConfiguratorActionItem.MouseClick += ActionConfiguratorActionItem_MouseClick;
                    pluginsList.Controls.Add(actionConfiguratorActionItem);
                }
            }
        }
    }

    /// <summary>
    /// 处理动作条目的点击事件：选中该动作并在右侧面板显示配置界面。
    /// </summary>
    private void ActionConfiguratorActionItem_MouseClick(object sender, MouseEventArgs e)
    {
        var actionConfiguratorActionItem = sender as ActionConfiguratorActionItem;
        if (actionConfiguratorActionItem.PluginAction == null) return;

        // 仅当选中不同类型动作时才重新赋值
        if (Action == null || Action.GetType() != actionConfiguratorActionItem.PluginAction.GetType())
            Action = actionConfiguratorActionItem.PluginAction;

        // 更新右侧面板的图标和描述信息
        selectedPluginIcon.BackgroundImage = actionConfiguratorActionItem.Plugin.PluginIcon ?? Resources.Icon;
        lblSelectedActionName.Text = Action.Name;
        labelDescription.Text = Action.Description;

        // 清理旧的配置控件
        foreach (Control control in configurationPanel.Controls) control.Dispose();
        configurationPanel.Controls.Clear();

        // 根据动作是否支持配置，显示配置控件或提示信息
        if (Action.CanConfigure)
            configurationPanel.Controls.Add(Action.GetActionConfigControl(this));
        else
        {
            var noConfigure = new Label
            {
                Text = LanguageManager.Strings.ActionNeedsNoConfiguration,
                Size = configurationPanel.Size,
                TextAlign = ContentAlignment.MiddleCenter
            };
            configurationPanel.Controls.Add(noConfigure);
        }
    }

    /// <summary>
    /// 处理插件条目的点击事件：切换该插件的展开/折叠状态。
    /// </summary>
    private void ActionConfiguratorPluginItem_MouseClick(object sender, MouseEventArgs e)
    {
        var actionConfiguratorPluginItem = sender as ActionConfiguratorPluginItem;
        SetExpand(actionConfiguratorPluginItem, !actionConfiguratorPluginItem.Selected);
    }

    /// <summary>
    /// 展开或折叠指定插件条目，并控制其下所有动作子条目的可见性。
    /// </summary>
    /// <param name="actionConfiguratorPluginItem">目标插件条目。</param>
    /// <param name="expand">True 展开，False 折叠。</param>
    private void SetExpand(ActionConfiguratorPluginItem actionConfiguratorPluginItem, bool expand)
    {
        actionConfiguratorPluginItem.Selected = expand;
        actionConfiguratorPluginItem.Visible = true;
        // 将目标条目滚动到可视区域
        pluginsList.ScrollControlIntoView(actionConfiguratorPluginItem);
        // 控制该插件下所有动作子条目的可见性
        foreach (var actionConfiguratorActionItem in pluginsList.Controls)
        {
            if (!(actionConfiguratorActionItem is ActionConfiguratorActionItem) ||
                !(actionConfiguratorActionItem as ActionConfiguratorActionItem).Plugin.Equals(
                    actionConfiguratorPluginItem.Plugin)) continue;

            (actionConfiguratorActionItem as ActionConfiguratorActionItem).Visible
                = actionConfiguratorPluginItem.Selected;
        }
    }

    /// <summary>
    /// 根据插件实例查找对应的条目控件，并执行展开/折叠操作。
    /// </summary>
    /// <param name="plugin">目标插件。</param>
    /// <param name="expand">True 展开，False 折叠。</param>
    private void SetExpand(MacroDeckPlugin plugin, bool expand)
    {
        ActionConfiguratorPluginItem actionConfiguratorPluginItem = null;
        // 在控件列表中查找与指定插件匹配的条目
        foreach (Control control in pluginsList.Controls)
        {
            if (!(control is ActionConfiguratorPluginItem)) continue;
            if ((control as ActionConfiguratorPluginItem).Plugin.Equals(plugin))
                actionConfiguratorPluginItem = (ActionConfiguratorPluginItem)control;
        }
        if (actionConfiguratorPluginItem != null) SetExpand(actionConfiguratorPluginItem, expand);
    }

    /// <summary>
    /// 确认按钮点击事件：保存动作配置并关闭对话框。
    /// </summary>
    private void BtnApply_Click(object sender, EventArgs e)
    {
        if (Action != null)
        {
            var actionConfigControl = configurationPanel.Controls[0] as ActionConfigControl;
            // 如果动作可配置，先尝试保存配置
            if (Action.CanConfigure && actionConfigControl != null)
            {
                if (!actionConfigControl.OnActionSave()) return;
            }
            // 验证配置是否已保存（非空）
            if (Action.CanConfigure && Action.Configuration == null && string.IsNullOrWhiteSpace(Action.Configuration)) return;
            DialogResult = DialogResult.OK;
        }
        Close();
    }

    /// <summary>
    /// 搜索框文本变化时触发过滤逻辑。
    /// </summary>
    private void PluginSearch_TextChanged(object sender, EventArgs e)
    {
        Filter(pluginSearch.Text);
    }
}
