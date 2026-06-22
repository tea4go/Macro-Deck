## 1. 系统概述
Macro Deck 采用 **Serilog** 作为核心日志框架，构建了包含控制台、文件滚动、GUI 调试窗口以及 Sentry 错误上报的多通道日志系统。该系统通过 `MacroDeckLogger` 静态类提供统一的 API，支持结构化日志记录，并实现了主机代码与插件代码的日志隔离。

## 2. 核心组件与架构

### 2.1 日志初始化与配置 (`LoggingConfig.cs`)
- **启动时机**：在 `Program.cs` 的 `Main` 方法中最早期调用 `LoggingConfig.CreateLogger()` 并赋值给 `Log.Logger`，确保从应用启动第一行代码起即可记录日志。
- **日志级别控制**：使用 `LoggingLevelSwitch` 实现运行时动态调整日志级别（默认调试模式下为 `Trace`，发布模式为 `Info`）。
- **输出模板**：统一格式为 `[HH:mm:ss Level] [Source] Message{Exception}`。

### 2.2 日志接收器 (Sinks)
1. **Console Sink**：输出到标准控制台，使用 `AnsiConsoleTheme.Code` 主题进行着色。
2. **File Sink**：滚动写入 `%AppData%\Macro Deck\logs\log.txt`。
   - 策略：按天滚动 (`RollingInterval.Day`)。
   - 限制：单文件最大 50MB。
   - 清理：`MacroDeckLogger.CleanUpLogsDir()` 自动删除 30 天前的日志。
3. **DebugConsoleSink**：自定义 Sink，将日志实时推送到 GUI 层的 `DebugConsole` 窗口，方便用户在不查看文件的情况下排查问题。
4. **Sentry Sink**：条件启用。仅当 CI/CD 注入有效 DSN 时启用，且通过 `Conditional` 包装器过滤，仅上报主机代码错误。

### 2.3 结构化与 enrichers
- **PluginSourceEnricher**：自动为每条日志添加 `Source` 属性。如果日志来自插件，`Source` 为插件名；否则为 `MacroDeck`。
- **结构化参数**：所有日志 API 均支持 Serilog 消息模板（如 `{Host}`），避免字符串拼接，便于后续查询分析。

## 3. 日志 API 规范 (`MacroDeckLogger.cs`)
开发者应使用 `SuchByte.MacroDeck.Logging.MacroDeckLogger` 静态类进行日志记录，而非直接调用 `Serilog.Log`。

### 3.1 推荐用法
```csharp
// 主机代码日志
MacroDeckLogger.Information("Server started on port {Port}", port);
MacroDeckLogger.Error(ex, "Failed to connect to {DeviceId}", deviceId);

// 插件代码日志 (自动标记 Source)
MacroDeckLogger.Information(plugin, "Action executed for {User}", userName);
```

### 3.2 日志级别定义
- `Trace`: 详细调试信息（仅调试模式或手动开启）。
- `Debug`: 开发调试信息。
- `Information`: 常规运行信息。
- `Warning`: 潜在问题，但不影响运行。
- `Error`: 错误事件，会触发 Sentry 上报（仅限主机代码）。
- `Fatal`: 致命错误。

## 4. 错误追踪与隐私保护 (`SentryConfiguration.cs`)
- **隔离策略**：通过检查 `SourceContext` 是否以 `SuchByte.MacroDeck` 开头，严格禁止插件错误上报到 Sentry，保护第三方开发者隐私并减少噪音。
- **数据脱敏 (Scrubbing)**：
  - 自动替换用户配置文件路径为 `%USERPROFILE%`。
  - 自动替换当前用户名为 `[user]`。
  - 清除堆栈跟踪中的绝对路径信息。
- **环境标识**：自动区分 `debug` 和 `release` 环境。

## 5. 开发者准则
1. **禁止直接使用 `Console.WriteLine`**：所有输出必须经过 `MacroDeckLogger`。
2. **使用结构化模板**：始终使用 `{PropertyName}` 占位符，不要手动拼接字符串到消息中。
3. **异常记录**：记录异常时，务必将 `Exception` 对象作为第一个参数传入，以便 Sentry 捕获堆栈。
4. **插件开发者**：调用带 `MacroDeckPlugin` 参数的重载方法，以便日志系统正确标记来源。
5. **敏感信息**：尽管有自动脱敏，仍应避免在日志中明文记录密码、Token 或个人身份信息。