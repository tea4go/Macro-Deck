using System.Text.Json;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 变量视图创建者过滤模型，用于管理变量视图中隐藏的创建者列表。
/// 实现 ISerializableConfiguration 接口以支持 JSON 序列化/反序列化。
/// </summary>
public class VariableViewCreatorFilterModel : ISerializableConfiguration
{
    /// <summary>隐藏的创建者名称列表，这些创建者的变量不会在变量视图中显示</summary>
    public List<string> HiddenCreators { get; set; } = new();

    /// <summary>
    /// 将当前配置序列化为 JSON 字符串
    /// </summary>
    /// <returns>JSON 格式的配置字符串</returns>
    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// 从 JSON 字符串反序列化为 VariableViewCreatorFilterModel 实例
    /// </summary>
    /// <param name="config">JSON 格式的配置字符串</param>
    /// <returns>反序列化后的配置模型实例</returns>
    public static VariableViewCreatorFilterModel Deserialize(string config)
    {
        return ISerializableConfiguration.Deserialize<VariableViewCreatorFilterModel>(config);
    }
}
