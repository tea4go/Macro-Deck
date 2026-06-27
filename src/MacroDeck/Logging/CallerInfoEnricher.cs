using System.Diagnostics;
using System.IO;
using Serilog.Core;
using Serilog.Events;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// 日志事件丰富器，通过遍历调用栈帧捕获调用者的源文件名和行号。
/// 跳过 Serilog 内部帧和 MacroDeck 日志基础设施帧，找到第一个业务代码帧后提取
/// SourceFile（仅文件名，不含路径）和 SourceLine 属性。
/// 当调用栈中没有可用的调试符号信息时，属性值为空字符串。
/// </summary>
public class CallerInfoEnricher : ILogEventEnricher
{
    /// <summary>源码文件名属性键</summary>
    public const string SourceFilePropertyName = "SourceFile";

    /// <summary>源码行号属性键</summary>
    public const string SourceLinePropertyName = "SourceLine";

    /// <summary>
    /// 丰富日志事件，添加 SourceFile 和 SourceLine 属性。
    /// </summary>
    /// <param name="logEvent">日志事件</param>
    /// <param name="propertyFactory">属性工厂</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var (fileName, lineNumber) = ResolveCallerInfo();

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty(SourceFilePropertyName, fileName));
        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty(SourceLinePropertyName, lineNumber));
    }

    /// <summary>
    /// 遍历调用栈，查找第一个不属于 Serilog/MacroDeck 日志基础设施的帧，
    /// 返回其源文件名（不含路径）和行号。
    /// </summary>
    /// <returns>(文件名, 行号) 元组；无法获取时返回空字符串和 0。</returns>
    private static (string fileName, int lineNumber) ResolveCallerInfo()
    {
        // 跳过 Logging 命名空间内部的帧类型名称前缀
        const string loggingNamespace = "SuchByte.MacroDeck.Logging";

        var stackTrace = new StackTrace(fNeedFileInfo: true);
        var frames = stackTrace.GetFrames();
        if (frames is null)
        {
            return (string.Empty, 0);
        }

        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            if (method?.DeclaringType is null)
            {
                continue;
            }

            var typeFullName = method.DeclaringType.FullName ?? string.Empty;

            // 跳过 Serilog 内部帧
            if (typeFullName.StartsWith("Serilog.", System.StringComparison.Ordinal))
            {
                continue;
            }

            // 跳过 MacroDeck 日志基础设施帧（MacroDeckLogger / DebugConsoleSink / 本类）
            if (typeFullName.StartsWith(loggingNamespace, System.StringComparison.Ordinal))
            {
                continue;
            }

            // 跳过编译器生成的闭包/异步状态机
            if (typeFullName.Contains("<>c") || typeFullName.Contains("<"))
            {
                continue;
            }

            var fileName = frame.GetFileName();
            if (string.IsNullOrEmpty(fileName))
            {
                continue;
            }

            return (Path.GetFileName(fileName), frame.GetFileLineNumber());
        }

        return (string.Empty, 0);
    }
}
