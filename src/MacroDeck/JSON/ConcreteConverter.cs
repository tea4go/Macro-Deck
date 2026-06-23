using Newtonsoft.Json;

namespace SuchByte.MacroDeck.JSON;

/// <summary>
/// 具体类型 JSON 转换器，用于在序列化/反序列化时将接口类型强制转换为具体类型。
/// 当属性类型为接口但需要序列化为具体实现类时使用此转换器。
/// </summary>
/// <typeparam name="T">目标具体类型</typeparam>
public class ConcreteConverter<T> : JsonConverter
{
    /// <summary>
    /// 判断是否可以转换，始终返回 true
    /// </summary>
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    /// <summary>
    /// 读取 JSON 并反序列化为具体类型 T
    /// </summary>
    public override object ReadJson(JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        return serializer.Deserialize<T>(reader);
    }

    /// <summary>
    /// 将对象序列化为 JSON
    /// </summary>
    public override void WriteJson(JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
