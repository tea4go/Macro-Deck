using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuchByte.MacroDeck.CottleIntegration;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;

namespace SuchByte.MacroDeck.ActionButton;

/// <summary>
/// 条件动作类，继承自 PluginAction，实现基于条件的分支逻辑。
/// 支持三种条件类型：变量比较、按钮状态判断、模板求值，
/// 根据条件结果执行不同的动作列表（满足条件或否则分支）。
/// </summary>
public class ConditionAction : PluginAction
{
    /// <summary>
    /// 动作名称
    /// </summary>
    public override string Name => "Condition";

    private List<PluginAction?> _actions;
    private List<PluginAction?> _actionsElse;
    private string _conditionValue1Source = "";
    private ConditionType _conditionType = ConditionType.Variable;
    private ConditionMethod _conditionMethod = ConditionMethod.Equals;
    private string _conditionValue2 = "";

    /// <summary>
    /// 条件为真时执行的动作列表
    /// </summary>
    public List<PluginAction?> Actions
    {
        get => _actions;
        set
        {
            _actions = value;
            UpdateConfiguration();
        }
    }

    /// <summary>
    /// 条件为假时执行的动作列表（否则分支）
    /// </summary>
    public List<PluginAction?> ActionsElse
    {
        get => _actionsElse;
        set
        {
            _actionsElse = value;
            UpdateConfiguration();
        }
    }

    /// <summary>
    /// 条件值1的来源（变量名或模板表达式）
    /// </summary>
    public string ConditionValue1Source
    {
        get => _conditionValue1Source;
        set
        {
            _conditionValue1Source = value;
            UpdateConfiguration();
        }
    }

    /// <summary>
    /// 条件类型：变量比较、按钮状态、模板求值
    /// </summary>
    public ConditionType ConditionType
    {
        get => _conditionType;
        set
        {
            _conditionType = value;
            UpdateConfiguration();
        }
    }

    /// <summary>
    /// 条件比较方法：等于、大于、小于、不等于
    /// </summary>
    public ConditionMethod ConditionMethod
    {
        get => _conditionMethod;
        set
        {
            _conditionMethod = value;
            UpdateConfiguration();
        }
    }

    /// <summary>
    /// 条件值2（用于与值1进行比较的目标值）
    /// </summary>
    public string ConditionValue2
    {
        get => _conditionValue2;
        set
        {
            _conditionValue2 = value;
            UpdateConfiguration();
        }
    }

    /// <summary>
    /// 构造函数，从配置字符串中反序列化条件动作的各项参数
    /// </summary>
    public ConditionAction()
    {
        if (!string.IsNullOrEmpty(Configuration))
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Error = (sender, args) => { args.ErrorContext.Handled = true; }
            };

            // 从 JSON 配置中解析各字段
            _actions = JsonConvert.DeserializeObject<List<PluginAction>>(
                JObject.Parse(Configuration)["actions"].ToString(),
                jsonSerializerSettings);
            _actionsElse
                = JsonConvert.DeserializeObject<List<PluginAction>>(
                    JObject.Parse(Configuration)["actionsElse"].ToString(),
                    jsonSerializerSettings);
            _conditionValue1Source
                = JsonConvert.DeserializeObject<string>(JObject.Parse(Configuration)["source"].ToString(),
                    jsonSerializerSettings);
            _conditionType
                = JsonConvert.DeserializeObject<ConditionType>(JObject.Parse(Configuration)["conditionType"].ToString(),
                    jsonSerializerSettings);
            _conditionMethod
                = JsonConvert.DeserializeObject<ConditionMethod>(
                    JObject.Parse(Configuration)["contitionMethod"].ToString(),
                    jsonSerializerSettings);
            _conditionValue2
                = JsonConvert.DeserializeObject<string>(JObject.Parse(Configuration)["conditionValue2"].ToString(),
                    jsonSerializerSettings);
        }

        if (_actions == null)
        {
            _actions = new List<PluginAction?>();
        }

        if (_actionsElse == null)
        {
            _actionsElse = new List<PluginAction?>();
        }
    }

    /// <summary>
    /// 将当前的条件配置序列化为 JSON 字符串并更新 Configuration 属性
    /// </summary>
    private void UpdateConfiguration()
    {
        JObject configurationString;
        try
        {
            configurationString = JObject.Parse(Configuration);
        }
        catch
        {
            configurationString = new JObject();
        }


        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; }
        };

        configurationString["actions"] = JsonConvert.SerializeObject(_actions, jsonSerializerSettings);
        configurationString["actionsElse"] = JsonConvert.SerializeObject(_actionsElse, jsonSerializerSettings);
        configurationString["source"] = JsonConvert.SerializeObject(_conditionValue1Source, jsonSerializerSettings);
        configurationString["conditionType"] = JsonConvert.SerializeObject(_conditionType, jsonSerializerSettings);
        configurationString["contitionMethod"] = JsonConvert.SerializeObject(_conditionMethod, jsonSerializerSettings);
        configurationString["conditionValue2"] = JsonConvert.SerializeObject(_conditionValue2, jsonSerializerSettings);

        Configuration = configurationString.ToString();
    }

    /// <summary>
    /// 动作描述
    /// </summary>
    public override string Description => "";

    /// <summary>
    /// 获取动作配置控件（此动作不支持配置界面）
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>始终返回 null</returns>
    public ActionConfigControl GetActionConfigurator(ActionConfigurator actionConfigurator)
    {
        return null;
    }

    /// <summary>
    /// 触发条件动作，根据条件类型和比较方法评估条件，
    /// 然后执行对应的动作列表（满足条件执行 Actions，否则执行 ActionsElse）。
    /// 
    /// 条件评估逻辑：
    /// - Variable 类型：比较变量值与目标值（支持等于/不等于/大于/小于）
    /// - Button_State 类型：比较按钮当前状态与目标布尔值
    /// - Template 类型：使用模板引擎求值并解析为布尔结果
    /// </summary>
    /// <param name="clientId">触发动作的客户端 ID</param>
    /// <param name="actionButton">关联的动作按钮</param>
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var result = false;
        var conditionValue2 = ConditionValue2;
        var variable = VariableManager.ListVariables.FirstOrDefault(v => v.Name == _conditionValue1Source);

        // 替换条件值2中的变量占位符，如 {variableName} 替换为实际变量值
        foreach (var v in VariableManager.ListVariables)
        {
            if (conditionValue2.ToLower().Contains("{" + v.Name.ToLower() + "}"))
            {
                conditionValue2
                    = conditionValue2.Replace("{" + v.Name + "}", v.Value, StringComparison.OrdinalIgnoreCase);
            }
        }

        switch (_conditionType)
        {
            case ConditionType.Variable:
                // 变量条件：根据比较方法对变量值和目标值进行比较
                switch (_conditionMethod)
                {
                    case ConditionMethod.Equals:
                        result = !(variable == null || !variable.Value.ToLower().Equals(conditionValue2.ToLower()));
                        break;
                    case ConditionMethod.Not:
                        result = variable == null || !variable.Value.ToLower().Equals(conditionValue2.ToLower());
                        break;
                    case ConditionMethod.Bigger:
                        // 大于比较：仅对整数和浮点数类型变量有效
                        if (variable != null &&
                            !((VariableType)Enum.Parse(typeof(VariableType), variable.Type) != VariableType.Integer &&
                                (VariableType)Enum.Parse(typeof(VariableType), variable.Type) != VariableType.Float))
                        {
                            result = float.Parse(variable.Value) > float.Parse(conditionValue2);
                        }

                        break;
                    case ConditionMethod.Smaller:
                        // 小于比较：仅对整数和浮点数类型变量有效
                        if (variable != null &&
                            !((VariableType)Enum.Parse(typeof(VariableType), variable.Type) != VariableType.Integer &&
                                (VariableType)Enum.Parse(typeof(VariableType), variable.Type) != VariableType.Float))
                        {
                            result = float.Parse(variable.Value) < float.Parse(conditionValue2);
                        }

                        break;
                }

                break;
            case ConditionType.Button_State:
                // 按钮状态条件：比较按钮当前状态与目标布尔值
                var value2 = false;
                switch (_conditionMethod)
                {
                    case ConditionMethod.Equals:
                        // 解析目标值为布尔值（支持 "on"/"true"）
                        if (conditionValue2.ToLower().Equals("on") || conditionValue2.ToLower().Equals("true"))
                        {
                            value2 = true;
                        }

                        result = value2.Equals(actionButton.State);
                        break;
                    case ConditionMethod.Not:
                        if (conditionValue2.ToLower().Equals("on") || conditionValue2.ToLower().Equals("true"))
                        {
                            value2 = true;
                        }

                        result = !value2.Equals(actionButton.State);
                        break;
                }

                break;
            case ConditionType.Template:
                // 模板条件：使用 Cottle 模板引擎渲染表达式并解析为布尔结果
                if (bool.TryParse(TemplateManager.RenderTemplate(_conditionValue1Source), out var boolResult))
                {
                    result = boolResult;
                }

                break;
        }

        // 根据条件结果执行对应的动作列表
        if (result)
        {
            foreach (var action in _actions)
            {
                action.Trigger(clientId, actionButton);
            }
        }
        else
        {
            foreach (var action in _actionsElse)
            {
                action.Trigger(clientId, actionButton);
            }
        }
    }
}

/// <summary>
/// 条件类型枚举
/// </summary>
public enum ConditionType
{
    /// <summary>变量比较</summary>
    Variable,
    /// <summary>按钮状态判断</summary>
    Button_State,
    /// <summary>模板求值</summary>
    Template
}

/// <summary>
/// 条件比较方法枚举
/// </summary>
public enum ConditionMethod
{
    /// <summary>等于</summary>
    Equals,
    /// <summary>大于</summary>
    Bigger,
    /// <summary>小于</summary>
    Smaller,
    /// <summary>不等于</summary>
    Not
}
