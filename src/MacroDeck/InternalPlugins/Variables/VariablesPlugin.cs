using System.Globalization;
using System.IO;
using Serilog;
using SuchByte.MacroDeck.CottleIntegration;
using SuchByte.MacroDeck.Events;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.InternalPlugins.Variables.Enums;
using SuchByte.MacroDeck.InternalPlugins.Variables.Models;
using SuchByte.MacroDeck.InternalPlugins.Variables.Views;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Utils;
using Timer = System.Timers.Timer;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.Variables.Plugin;
// Don't change because of backwards compatibility!

/// <summary>
/// 变量内部插件，提供变量管理相关动作和事件。
/// 包含修改变量值、保存变量到文件和从文件读取变量等动作，
/// 以及变量变化事件。同时定期更新时间、日期和星期变量。
/// </summary>
public class VariablesPlugin : MacroDeckPlugin
{
    /// <summary>插件名称</summary>
    internal override string Name => LanguageManager.Strings.PluginMacroDeckVariables;

    /// <summary>插件作者</summary>
    internal override string Author => "Macro Deck";

    /// <summary>插件图标</summary>
    internal override Image PluginIcon => Resources.Variable_Normal;

    /// <summary>变量变化事件实例</summary>
    private VariableChangedEvent variableChangedEvent = new();

    /// <summary>时间日期更新定时器，每秒触发一次</summary>
    private Timer timeDateTimer;

    /// <summary>
    /// 启用插件，注册动作、事件监听器，并启动时间日期更新定时器。
    /// </summary>
    public override void Enable()
    {
        Actions = new List<PluginAction>
        {
            new ChangeVariableValueAction(),
            new SaveVariableToFileAction(),
            new ReadVariableFromFileAction()
        };
        EventManager.RegisterEvent(variableChangedEvent);
        VariableManager.OnVariableChanged += VariableChanged;

        // 启动每秒触发的时间日期更新定时器
        timeDateTimer = new Timer(1000)
        {
            Enabled = true
        };
        timeDateTimer.Elapsed += OnTimerTick;
        timeDateTimer.Start();

        Application.ApplicationExit += OnApplicationExit;
    }

    /// <summary>
    /// 应用程序退出时清理资源：取消事件订阅并释放定时器
    /// </summary>
    private void OnApplicationExit(object sender, EventArgs e)
    {
        Application.ApplicationExit -= OnApplicationExit;
        VariableManager.OnVariableChanged -= VariableChanged;

        if (timeDateTimer != null)
        {
            timeDateTimer.Stop();
            timeDateTimer.Elapsed -= OnTimerTick;
            timeDateTimer.Dispose();
            timeDateTimer = null;
        }
    }

    /// <summary>
    /// 定时器触发回调，每秒更新时间、日期和星期变量。
    /// 使用当前语言对应的文化信息格式化日期和星期。
    /// </summary>
    private void OnTimerTick(object sender, EventArgs e)
    {
        Task.Run(() =>
        {
            // 根据选择的语言设置文化信息
            var culture = new CultureInfo(LanguageManager.GetLanguageCode());

            VariableManager.SetValue("time", DateTime.Now.ToString("t"), VariableType.String, "Macro Deck");
            VariableManager.SetValue("date", DateTime.Now.ToString("d"), VariableType.String, "Macro Deck");
            VariableManager.SetValue("day_of_week",
                culture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek),
                VariableType.String,
                "Macro Deck");
        });
    }

    /// <summary>
    /// 变量变化事件处理，触发变量变化事件以通知监听的动作按钮
    /// </summary>
    private void VariableChanged(object sender, EventArgs e)
    {
        variableChangedEvent.Trigger(sender);
    }
}

/// <summary>
/// 变量变化事件，当任意变量值发生变化时触发。
/// 遍历所有配置文件中监听此事件的动作按钮并触发对应事件。
/// </summary>
public class VariableChangedEvent : IMacroDeckEvent
{
    /// <summary>事件名称</summary>
    public string Name => "Variable changed";

    /// <summary>事件触发回调</summary>
    public EventHandler<MacroDeckEventArgs> OnEvent { get; set; }

    /// <summary>
    /// 参数建议列表，返回所有变量名供用户选择
    /// </summary>
    public List<string> ParameterSuggestions
    {
        get
        {
            var variables = new List<string>();
            foreach (var variable in VariableManager.ListVariables)
            {
                variables.Add(variable.Name);
            }

            return variables;
        }
        set { }
    }

    /// <summary>
    /// 触发变量变化事件。
    /// 遍历所有配置文件、文件夹和动作按钮，
    /// 查找监听此事件的按钮并触发事件回调。
    /// </summary>
    /// <param name="sender">触发事件的变量</param>
    public void Trigger(object sender)
    {
        if (OnEvent != null)
        {
            try
            {
                foreach (var macroDeckProfile in ProfileManager.Profiles)
                {
                    foreach (var folder in macroDeckProfile.Folders)
                    {
                        if (folder.ActionButtons == null)
                        {
                            continue;
                        }

                        // 查找所有监听变量变化事件的动作按钮
                        foreach (var actionButton in folder.ActionButtons.FindAll(actionButton =>
                            actionButton.EventListeners != null &&
                            actionButton.EventListeners.Find(x =>
                                x.EventToListen != null && x.EventToListen.Equals(Name)) !=
                            null))
                        {
                            var macroDeckEventArgs = new MacroDeckEventArgs
                            {
                                ActionButton = actionButton,
                                Parameter = ((Variable)sender).Name
                            };
                            OnEvent(this, macroDeckEventArgs);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}

/// <summary>
/// 修改变量值动作，支持递增、递减、设置和切换四种方式。
/// </summary>
public class ChangeVariableValueAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionChangeVariableValue;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionChangeVariableValue;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>修改变量值配置视图</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new ChangeVariableValueActionConfigView(this);
    }

    /// <summary>
    /// 触发修改变量值动作。
    /// 根据配置的方法类型执行不同操作：
    /// - countUp：变量值递增 1
    /// - countDown：变量值递减 1
    /// - set：使用模板引擎渲染后设置变量值
    /// - toggle：切换布尔变量值（true/false）
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        if (string.IsNullOrWhiteSpace(Configuration))
        {
            return;
        }

        var changeVariableActionConfigModel = ChangeVariableValueActionConfigModel.Deserialize(Configuration);
        var variable
            = VariableManager.ListVariables.FirstOrDefault(v => v.Name == changeVariableActionConfigModel.Variable);
        if (variable == null)
        {
            return;
        }

        switch (changeVariableActionConfigModel.Method)
        {
            case ChangeVariableMethod.countUp:
                // 递增：当前值加 1
                VariableManager.SetValue(variable.Name,
                    float.Parse(variable.Value) + 1,
                    (VariableType)Enum.Parse(typeof(VariableType), variable.Type),
                    variable.Creator);
                break;
            case ChangeVariableMethod.countDown:
                // 递减：当前值减 1
                VariableManager.SetValue(variable.Name,
                    float.Parse(variable.Value) - 1,
                    (VariableType)Enum.Parse(typeof(VariableType), variable.Type),
                    variable.Creator);
                break;
            case ChangeVariableMethod.set:
                // 设置：使用模板引擎渲染值后设置
                var value = TemplateManager.RenderTemplate(changeVariableActionConfigModel.Value);
                VariableManager.SetValue(variable.Name,
                    value,
                    (VariableType)Enum.Parse(typeof(VariableType), variable.Type),
                    variable.Creator);
                break;
            case ChangeVariableMethod.toggle:
                // 切换：反转布尔值（支持 "on" 作为 true 的别名）
                VariableManager.SetValue(variable.Name,
                    !bool.Parse(variable.Value.Replace("on", "true")),
                    (VariableType)Enum.Parse(typeof(VariableType), variable.Type),
                    variable.Creator);
                break;
        }
    }
}

/// <summary>
/// 保存变量到文件动作，将指定变量的值写入到文件中。
/// 使用重试机制处理文件写入失败的情况。
/// </summary>
public class SaveVariableToFileAction : PluginAction
{
    private static readonly ILogger Logger = Log.ForContext(typeof(SaveVariableToFileAction));

    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionSaveVariableToFile;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionSaveVariableToFileDescription;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>
    /// 触发保存变量到文件动作。
    /// 读取配置中的文件路径和变量名，将变量值写入文件。
    /// 如果变量不存在，写入 "Variable not found"。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        var configurationModel = ReadVariableFromFileActionConfigModel.Deserialize(Configuration);
        if (configurationModel == null)
        {
            return;
        }

        var filePath = configurationModel.FilePath;
        var variable = VariableManager.ListVariables.FirstOrDefault(x => x.Name == configurationModel.Variable);
        string variableValue;
        if (variable == null)
        {
            variableValue = "Variable not found";
        }
        else
        {
            variableValue = variable.Value;
        }

        try
        {
            // 使用重试机制写入文件，处理可能的 IO 冲突
            Retry.Do(() => { File.WriteAllText(filePath, variableValue); });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to save variable value to file");
        }
    }

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>保存变量到文件配置视图</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new SaveVariableToFileActionConfigView(this);
    }
}

/// <summary>
/// 从文件读取变量动作，将文件内容读取并设置到指定变量。
/// 根据变量类型自动转换文件内容（布尔、浮点、整数、字符串）。
/// </summary>
public class ReadVariableFromFileAction : PluginAction
{
    private static readonly ILogger Logger = Log.ForContext(typeof(ReadVariableFromFileAction));

    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionReadVariableFromFile;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionReadVariableFromFileDescription;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>
    /// 触发从文件读取变量动作。
    /// 读取文件内容并根据变量类型进行转换后设置变量值。
    /// 支持布尔、浮点、整数和字符串四种变量类型。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        var configurationModel = SaveVariableToFileActionConfigModel.Deserialize(Configuration);
        if (configurationModel == null)
        {
            return;
        }

        var filePath = configurationModel.FilePath;
        var variable = VariableManager.ListVariables.FirstOrDefault(x => x.Name == configurationModel.Variable);
        try
        {
            // 使用重试机制读取文件并按变量类型转换
            Retry.Do(() =>
            {
                var value = File.ReadAllText(filePath).Trim();
                switch (variable.Type)
                {
                    case nameof(VariableType.Bool):
                        // 布尔类型：尝试解析为 bool
                        if (bool.TryParse(value, out var valueBool))
                        {
                            VariableManager.SetValue(variable.Name, valueBool, VariableType.Bool);
                        }

                        break;
                    case nameof(VariableType.Float):
                        // 浮点类型：尝试解析为 float
                        if (float.TryParse(value, out var valueFloat))
                        {
                            VariableManager.SetValue(variable.Name, valueFloat, VariableType.Float);
                        }

                        break;
                    case nameof(VariableType.Integer):
                        // 整数类型：尝试解析为 int
                        if (int.TryParse(value, out var valueInt))
                        {
                            VariableManager.SetValue(variable.Name, valueInt, VariableType.Integer);
                        }

                        break;
                    case nameof(VariableType.String):
                        // 字符串类型：直接设置
                        VariableManager.SetValue(variable.Name, value, VariableType.String);
                        break;
                }
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to read variable value from file");
        }
    }

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>从文件读取变量配置视图</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new ReadVariableFromFileActionConfigView(this);
    }
}
