Macro Deck 采用基于 **JSON 文件**的持久化配置方案，结合 **命令行参数**（CLI）进行启动时行为控制，并通过 **Windows 注册表**管理自启动项。系统没有使用复杂的配置框架（如 Microsoft.Extensions.Configuration），而是通过自定义的静态管理类直接处理序列化和路径解析。

### 1. 核心配置组件
*   **主配置 (`MainConfiguration`)**：位于 `src/MacroDeck/Configuration/MainConfiguration.cs`。使用 `Newtonsoft.Json` 进行序列化。包含网络设置（Host/Port, SSL, ADB）、更新策略、语言选择及隐私设置（错误报告）。
    *   **副作用管理**：`AutoStart` 属性的 Setter 直接操作 Windows 注册表 (`HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`) 来实现开机自启的开关。
*   **路径管理 (`ApplicationPaths`)**：位于 `src/MacroDeck/StartupConfig/ApplicationPaths.cs`。负责初始化所有关键目录和文件路径。
    *   **便携模式支持**：通过 `--portable` 启动参数切换。若启用，数据存储在程序目录下的 `Data` 文件夹；否则存储在 `%APPDATA%\Macro Deck`。
    *   **自动创建**：在 `Initialize` 阶段自动检查并创建所有必要的子目录（plugins, logs, backups, profiles 等）。
*   **启动参数 (`StartParameters`)**：位于 `src/MacroDeck/StartupConfig/StartParameters.cs`。使用 `CommandLineParser` 库解析 CLI 参数。支持 `--port`, `--force-update`, `--debug-console`, `--log-level` 等调试和运维选项。

### 2. 配置加载流程
1.  **入口点 (`Program.cs`)**：解析 CLI 参数 -> 初始化 `ApplicationPaths` -> 创建 Serilog 日志器。
2.  **核心启动 (`MacroDeck.cs`)**：
    *   检查 `config.json` 是否存在。若不存在，进入 `InitialSetup`（初始设置向导）。
    *   若存在，调用 `MainConfiguration.LoadFromFile` 反序列化配置。
    *   根据配置应用语言、初始化变量管理器、插件管理器、图标管理器等。
    *   启动 WebSocket 服务器和广播服务。

### 3. 日志与诊断配置
*   **Serilog 集成**：在 `LoggingConfig.cs` 中配置。支持控制台、滚动文件（每日轮转，上限 50MB）以及可选的 Sentry 错误上报。
*   **动态日志级别**：通过 `MacroDeckLogger.LevelSwitch` 允许在运行时或通过 CLI (`--log-level`) 调整日志详细程度。

### 4. 开发者约定
*   **配置持久化**：所有需要跨会话保存的状态应通过 `MainConfiguration` 或类似的 JSON 模型处理。修改配置后需显式调用 `.Save(path)`。
*   **路径引用**：严禁硬编码文件路径。必须使用 `ApplicationPaths` 中定义的静态属性（如 `ApplicationPaths.PluginsDirectoryPath`）。
*   **敏感信息**：SSL 密钥等敏感字段在 `MainConfiguration` 中以加密字符串形式存储（如 `SslCertificateKeyPemEncrypted`），避免明文落盘。
*   **插件配置**：插件拥有独立的配置目录 (`ApplicationPaths.PluginConfigPath`)，通常由插件自行管理其 JSON 配置文件。