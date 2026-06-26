## 1. 核心架构与框架
Macro Deck 采用 **Serilog** 作为统一的日志记录框架，结合 **Sentry** 进行生产环境下的错误追踪与异常上报。系统通过 `LoggingConfig` 在应用启动早期（`Program.cs`）初始化全局静态日志器 `Log.Logger`，确保从应用程序生命周期的最开始就有日志输出。

### 关键组件
- **Serilog**: 负责结构化日志的记录、格式化与多路分发（Console, File, Debug UI, Sentry）。
- **Sentry (Serilog Sink)**: 负责将核心模块的 Error/Fatal 级别日志及未处理异常上报至云端监控平台。
- **MacroDeckLogger**: 封装的统一日志 API，提供面向核心代码和插件开发的便捷方法，并管理运行时日志级别。

## 2. 日志路由与接收器 (Sinks)
日志通过 `LoggingConfig.CreateLogger()` 配置，主要分发到以下四个目的地：

1. **控制台 (Console)**:
   - 使用 `AnsiConsoleTheme.Code` 主题，便于开发调试。
   - 输出模板：`[{Timestamp:HH:mm:ss} {Level:u3}] [{Source}] {Message:lj}{NewLine}{Exception}`。
2. **文件 (File)**:
   - 路径：`ApplicationPaths.LogsDirectoryPath/log.txt`。
   - 策略：按天滚动 (`RollingInterval.Day`)，单文件最大 50MB。
3. **调试控制台 (DebugConsoleSink)**:
   - 自定义 Sink，将日志实时转发至 GUI 中的 `DebugConsole` 窗口。
   - 根据日志级别（Fatal/Error=红色, Warning=橙色, Info=白色等）着色显示。
   - 若未打开调试窗口则自动静默，无性能损耗。
4. **Sentry (Conditional)**:
   - 仅在 DSN 已配置且 `SentryConfiguration.Enabled` 为真时启用。
   - 通过 `Conditional` 包装，仅发送符合 `ShouldSend` 规则的事件。

## 3. 结构化字段与上下文增强
系统通过自定义 Enricher 和 Context 策略，确保日志具备清晰的来源标识：

- **PluginSourceEnricher**: 为每条日志添加 `Source` 属性。
  - 如果日志包含 `Plugin` 属性，`Source` 设为插件名称。
  - 否则，`Source` 默认为 `"MacroDeck"`。
- **核心与插件隔离**:
  - **核心日志**: 通过 `Log.ForContext(SourceContextPropertyName, "SuchByte.MacroDeck")` 记录，携带标准的 `SourceContext`。
  - **插件日志**: 通过 `Log.ForContext("Plugin", plugin.Name)` 记录，携带 `Plugin` 属性。
  - **Sentry 过滤**: `SentryConfiguration` 严格排除带有 `Plugin` 属性的日志，且仅允许 `SourceContext` 以 `SuchByte.MacroDeck` 开头的事件上报，防止插件错误污染核心监控数据。

## 4. 隐私保护与数据清理
在上报 Sentry 前，`SentryConfiguration` 执行严格的隐私清洗：
- **BeforeSend/Breadcrumb 处理器**: 移除 `ServerName`。
- **Scrub 方法**: 将日志消息、异常堆栈、文件名中的 Windows 用户路径替换为 `%USERPROFILE%`，用户名替换为 `[user]`。
- **PII 禁用**: 设置 `SendDefaultPii = false`。

## 5. 开发者规范

### 5.1 核心代码日志记录
请使用 `SuchByte.MacroDeck.Logging.MacroDeckLogger` 静态类：
```csharp
// 信息级日志
MacroDeckLogger.Information("Application started on port {Port}", port);

// 错误级日志（带异常）
try { ... } 
catch (Exception ex) {
    MacroDeckLogger.Error(ex, "Failed to connect to device {DeviceId}", deviceId);
}
```

### 5.2 插件开发日志记录
插件应通过传入 `MacroDeckPlugin` 实例来记录日志，以便自动标记来源：
```csharp
public class MyPlugin : MacroDeckPlugin {
    public override void OnLoad() {
        MacroDeckLogger.Information(this, "Plugin loaded successfully");
    }
}
```

### 5.3 日志级别建议
- **Verbose/Debug**: 仅在开发或排查具体问题时使用，生产环境默认关闭。
- **Information**: 记录关键业务流程（如设备连接、配置加载）。
- **Warning**: 记录非致命异常或降级行为。
- **Error/Fatal**: 记录导致功能失效的异常，这类日志会被 Sentry 捕获。

### 5.4 注意事项
- **避免敏感信息**: 尽管有自动清洗，仍应避免在日志中明文记录密码、Token 或个人身份信息。
- **结构化参数**: 优先使用消息模板参数（如 `{UserId}`）而非字符串拼接，以便 Serilog 进行结构化索引。
- **运行时调整**: 可通过 `MacroDeckLogger.LogLevel` 属性在运行时动态调整最低日志级别。