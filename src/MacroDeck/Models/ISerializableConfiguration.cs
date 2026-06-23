using System.Text.Json;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 可序列化配置接口，为插件配置模型提供统一的序列化/反序列化能力。
/// 实现此接口的类可以方便地将配置序列化为 JSON 字符串或从 JSON 反序列化。
/// </summary>
public interface ISerializableConfiguration
{
    /// <summary>
    /// 将当前配置序列化为字符串
    /// </summary>
    /// <returns>序列化后的配置字符串</returns>
    public string Serialize();

    /// <summary>
    /// 从配置字符串反序列化为指定类型的配置实例。
    /// 如果配置字符串为空，则返回新的默认实例。
    /// </summary>
    /// <typeparam name="T">目标配置类型</typeparam>
    /// <param name="configuration">JSON 格式的配置字符串</param>
    /// <returns>反序列化后的配置实例</returns>
    protected static T Deserialize<T>(string configuration)
        where T : ISerializableConfiguration, new()
    {
        return !string.IsNullOrWhiteSpace(configuration) ? JsonSerializer.Deserialize<T>(configuration) : new T();
    }
}
