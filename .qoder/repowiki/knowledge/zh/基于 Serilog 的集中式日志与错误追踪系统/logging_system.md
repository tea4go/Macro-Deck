## 1. 核心框架与架构
Macro Deck 采用 **Serilog** 作为统一的日志记录框架，通过 `LoggingConfig` 在应用启动早期进行全局配置。系统采用了**结构化日志（Structured Logging）**模式，支持将日志事件路由到多个 Sink（输出端），并集成了远程错误追踪服务。

### 关键组件
- **`MacroDeckLogger`**: 应用程序的全局日志门面（Facade）。它封装了 Serilog 的 API，提供了静态方法供全项目调用，并管理日志级别的动态切换。
- **`LoggingConfig`**: 负责构建 `ILogger` 实例，定义日志的输出模板、最低级别以及各个 Sink 的配置。
- **`PluginSourceEnricher`**: 自定义的 Serilog Enricher，用于自动识别日志来源是“Macro Deck 核心”还是“第三方插件”，并在日志事件中附加 `Source` 属性。
- **`DebugConsoleSink`**: 自定义 Sink，将日志实时转发至 GUI 界面中的“调试控制台”窗口，方便开发者在不查看文件的情况下监控运行时状态。
- **`SentryConfiguration`**: 集成 Sentry 进行错误追踪和崩溃报告，包含严格的隐私过滤和来源白名单机制。

## 2. 日志输出与路由策略
日志事件会被同时分发到以下三个主要目的地：

1. **控制台 (Console)**: 
   - 使用 `AnsiConsoleTheme.Code` 主题，提供带颜色的高亮输出。
   - 适用于开发环境和容器化部署时的即时观察。
2. **滚动文件 (Rolling File)**:
   - 路径: `ApplicationPaths.LogsDirectoryPath/log.txt`。
   - 策略: 按天滚动 (`RollingInterval.Day`)，单文件大小限制为 50MB。
   - 用途: 持久化存储，用于事后排查和历史审计。
3. **GUI 调试控制台 (Debug Console)**:
   - 通过 `DebugConsoleSink` 实现。
   - 仅当 `DebugConsole` 窗口打开时生效，根据日志级别（Error, Warning, Info 等）显示不同颜色。
4. **Sentry (条件触发)**:
   - 仅在 CI/CD 注入了有效的 DSN 且用户未禁用错误报告时启用。
   - 仅上报 `Error` 及以上级别的事件。

## 3. 结构化字段与 enrichers
系统通过 `PluginSourceEnricher` 确保每条日志都包含明确的来源标识：
- **`Source`**: 如果日志来自插件，则该字段为插件名称；否则为 `"MacroDeck"`。
- **`Plugin`**: 内部属性，用于标记该事件是否由插件产生（Sentry 利用此属性过滤掉插件产生的错误）。
- **`Exception`**: 异常堆栈信息会被自动捕获并格式化。

输出模板统一为：
`[{Timestamp:HH:mm:ss} {Level:u3}] [{Source}] {Message:lj}{NewLine}{Exception}`

## 4. 日志级别管理
`MacroDeckLogger` 维护了一个全局可变的 `LogLevel` 枚举（Trace, Info, Warning, Error, Nothing）：
- **默认行为**: 如果附加了调试器（Debugger.IsAttached），默认级别为 `Trace`；否则为 `Info`。
- **动态调整**: 修改 `MacroDeckLogger.LogLevel` 会实时更新 Serilog 的 `LoggingLevelSwitch`，无需重启应用。
- **框架噪音抑制**: 在 `LoggingConfig` 中显式将 `Microsoft`、`System` 等命名空间的日志级别提升至 `Warning` 或 `Information`，以减少底层框架产生的冗余日志。

## 5. 隐私保护与 Sentry 集成
为了符合隐私规范，Sentry 集成实施了严格的数据清洗策略：
- **来源白名单**: `SentryConfiguration.ShouldSend` 仅允许 `SourceContext` 以 `SuchByte.MacroDeck` 开头的事件上报。**插件产生的错误不会被发送到 Sentry**，以保护第三方开发者的隐私并避免噪音。
- **数据脱敏 (Scrubbing)**: 在 `BeforeSend` 和 `BeforeBreadcrumb` 钩子中，系统会自动替换以下敏感信息：
  - Windows 用户配置文件路径 (`%USERPROFILE%`)
  - 当前用户名 (`[user]`)
  - 服务器名称被置空。
- **环境标识**: 自动区分 `debug` 和 `release` 环境，并附加程序集版本号作为 Release 标签。

## 6. 开发者规范
1. **统一入口**: 所有代码必须通过 `SuchByte.MacroDeck.Logging.MacroDeckLogger` 进行日志记录，禁止直接实例化 Serilog Logger。
2. **结构化模板**: 使用消息模板而非字符串拼接。例如：
   ```csharp
   // 推荐
   MacroDeckLogger.Information("Connected to {Host}", hostName);
   // 不推荐
   MacroDeckLogger.Information($"Connected to {hostName}");
   ```
3. **插件日志**: 插件开发者应使用带有 `MacroDeckPlugin` 参数的重载方法，以便系统自动标记来源：
   ```csharp
   MacroDeckLogger.Error(this, ex, "Failed to process action");
   ```
4. **异常处理**: 记录错误时务必传入 `Exception` 对象，以便 Sentry 和文件日志能捕获完整的堆栈跟踪。
5. **清理机制**: 系统会自动删除 30 天前的日志文件（见 `MacroDeckLogger.CleanUpLogsDir`），开发者无需手动管理磁盘空间。