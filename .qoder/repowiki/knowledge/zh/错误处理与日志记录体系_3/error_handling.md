## 1. 核心系统与工具
该仓库采用 **Serilog** 作为统一的日志记录框架，并集成 **Sentry** 进行生产环境的异常监控与上报。错误处理主要依赖于 .NET 标准的 `try-catch` 机制、全局异常钩子以及事件驱动的异步错误传播。

- **日志框架**: Serilog (结构化日志)
- **异常监控**: Sentry (通过 `Sentry.Serilog` 集成)
- **重试机制**: 自定义 `Retry` 工具类
- **全局捕获**: `Application.ThreadException` 和 `AppDomain.UnhandledException`

## 2. 关键文件与组件

| 文件路径 | 职责描述 |
| :--- | :--- |
| `src/MacroDeck/Logging/MacroDeckLogger.cs` | 核心日志门面。提供静态方法（`Error`, `Warning`, `Information` 等），支持结构化参数和插件上下文隔离。 |
| `src/MacroDeck/Logging/SentryConfiguration.cs` | Sentry 配置中心。定义了 DSN 占位符、隐私清洗逻辑（Scrub）以及仅上报核心模块（`SuchByte.MacroDeck`）错误的过滤规则。 |
| `src/MacroDeck/StartupConfig/LoggingConfig.cs` | 启动配置。初始化 Serilog Logger，配置控制台、文件滚动输出以及条件触发的 Sentry Sink。 |
| `src/MacroDeck/Program.cs` | 入口点。注册全局未处理异常处理器，确保崩溃前记录日志。 |
| `src/MacroDeck/Utils/Retry.cs` | 通用重试工具。对易失败操作提供带间隔的重试逻辑，最终抛出 `AggregateException`。 |
| `src/MacroDeck/DataTypes/WebSocketSession.cs` | 会话管理。在异步循环中捕获异常并通过 `Error` 事件向外传播。 |

## 3. 架构设计与约定

### 3.1 结构化日志与上下文隔离
`MacroDeckLogger` 强制使用结构化日志模板（如 `{LogMessage}`）。它通过 `SourceContext` 或自定义 `Plugin` 属性来区分日志来源：
- **核心模块**: 标记为 `SuchByte.MacroDeck`，会被 Sentry 捕获。
- **插件模块**: 标记为 `Plugin: {Name}`，在 `SentryConfiguration.ShouldSend` 中被显式排除，防止第三方插件错误污染主项目的监控数据。

### 3.2 隐私保护与数据清洗
在将错误发送至 Sentry 之前，`SentryConfiguration` 会执行严格的清洗：
- 移除 `ServerName`。
- 替换用户配置文件路径 (`%USERPROFILE%`) 和用户名 (`[user]`)。
- 过滤堆栈跟踪中的绝对路径信息。

### 3.3 全局异常兜底
在 `Program.cs` 中，系统注册了两个关键的全局处理器：
1. `Application.ThreadException`: 捕获 UI 线程的未处理异常。
2. `CurrentDomainOnUnhandledException`: 捕获非 UI 线程的致命错误。
这些处理器均调用 `MacroDeckLogger.Error` 记录现场，确保应用在意外崩溃前留下诊断线索。

### 3.4 异步错误传播
在 WebSocket 等长连接场景中（`WebSocketSession.cs`），错误不直接抛出导致进程终止，而是通过 `Error` 事件通知订阅者，并在 `finally` 块中执行清理和断开连接操作，体现了“故障隔离”的设计思想。

## 4. 开发者规范

1. **禁止使用 `Console.WriteLine`**: 所有调试和运行信息必须通过 `MacroDeckLogger` 记录。
2. **优先使用结构化参数**: 调用日志方法时，应使用 `MacroDeckLogger.Error(ex, "Failed to connect to {Host}", host)` 而非字符串拼接。
3. **插件错误隔离**: 编写插件时应使用带有 `MacroDeckPlugin` 参数的日志重载，以确保错误不会被误报至 Sentry。
4. **谨慎使用空 Catch**: 仅在确实需要忽略的非关键操作（如日志清理、进程强制终止）中使用空的 `catch {}`，业务逻辑中必须记录异常详情。
5. **重试策略**: 对于网络请求或文件 IO 等不稳定操作，优先使用 `Retry.Do` 工具方法，避免手动编写嵌套的重试循环。