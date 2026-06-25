using CommandLine;

namespace SuchByte.MacroDeck.StartupConfig;

/// <summary>
/// 启动参数解析类。使用 CommandLine 库解析命令行启动参数，
/// 支持端口、更新通道、便携模式等配置选项。
/// </summary>
public class StartParameters 
{ 
    /// <summary>指定 WebSocket 服务器监听端口。-1 表示使用默认端口。</summary> 
    [Option("port", Default = -1, Required = false)] 
    public int Port { get; set; } 
 
    /// <summary>是否强制执行更新检查。</summary> 
    [Option("force-update", Required = false)] 
    public bool ForceUpdate { get; set; } 
 
    /// <summary>是否启用测试更新通道。</summary> 
    [Option("test-channel", Required = false)] 
    public bool TestUpdateChannel { get; set; } 
 
    /// <summary>是否导出默认语言字符串。</summary> 
    [Option("export-default-strings", Required = false)] 
    public bool ExportDefaultStrings { get; set; } 
 
    /// <summary>是否以便携模式运行。便携模式下所有数据存储在应用程序目录中。</summary> 
    [Option("portable", Required = false)] public bool PortableMode { get; set; } 
 
    /// <summary>是否在启动时显示主窗口。</summary> 
    [Option("show", Required = false)] public bool ShowMainWindow { get; set; } 
 
    /// <summary>是否禁用文件日志记录。</summary> 
    [Option("disable-file-logging", Required = false)] 
    public bool DisableFileLogging { get; set; } 
 
    // 0 表示未指定；回退到 MacroDeckLogger 中基于调试器的默认值。 
    /// <summary>日志级别。0 表示未指定，将回退到 MacroDeckLogger 中基于调试器的默认值。</summary> 
    [Option("log-level", Default = 0, Required = false)] 
    public int LogLevel { get; set; } 
 
    /// <summary>是否启用调试控制台输出。</summary> 
    [Option("debug-console", Required = false)] 
    public bool DebugConsole { get; set; } 
 
    /// <summary>忽略 PID 检查。用于允许同时运行多个实例。</summary> 
    [Option("ignore-pid-check", Default = 0, Required = false)] 
    public int IgnorePidCheck { get; set; } 
 
    /// <summary>指定启动后显示的视图名称。</summary> 
    [Option("view", Default = "", Required = false)] 
    public string View { get; set; } = ""; 
 
    /// <summary> 
    /// 解析命令行参数并返回 StartParameters 实例。 
    /// 如果没有提供任何参数，返回默认配置的实例。 
    /// </summary> 
    /// <param name="args">命令行参数数组。</param> 
    /// <returns>解析后的 StartParameters 实例。</returns> 
    public static StartParameters ParseParameters(string[] args) 
    { 
        var startParameters = new StartParameters(); 
        if (args.Length == 0) 
        { 
            return startParameters; 
        } 
 
        using var parser = new Parser(options => 
        { 
            options.HelpWriter = Console.Error; 
            options.IgnoreUnknownArguments = true; 
            options.EnableDashDash = true; 
        }); 
 
        parser.ParseArguments<StartParameters>(args) 
            .WithParsed(sp => startParameters = sp); 
 
        return startParameters; 
    } 
 
    /// <summary> 
    /// 将 StartParameters 实例转换为命令行参数数组。 
    /// 通过反射读取所有带有 OptionAttribute 的属性，生成对应的 --option value 格式字符串。 
    /// </summary> 
    /// <param name="startParameters">要转换的 StartParameters 实例。</param> 
    /// <returns>命令行参数数组。</returns> 
    public static string[] ToArray(StartParameters startParameters) 
    { 
        var parameters = new string[startParameters.GetType().GetProperties().Length]; 
        var properties = startParameters.GetType().GetProperties(); 
 
        for (var i = 0; i < properties.Length; i++) 
        { 
            var propertyInfo = properties[i]; 
            var optionAttribute 
                = Attribute.GetCustomAttribute(propertyInfo, typeof(OptionAttribute)) as OptionAttribute; 
            if (optionAttribute == null) 
            { 
                continue; 
            } 
 
            parameters[i] = $"--{optionAttribute.LongName} " + propertyInfo.GetValue(startParameters); 
        } 
 
        return parameters; 
    } 
} 
