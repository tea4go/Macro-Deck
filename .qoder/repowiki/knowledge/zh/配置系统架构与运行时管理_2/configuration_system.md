Macro Deck 采用基于 JSON 文件的持久化配置方案，结合命令行参数（CLI）和 Windows 注册表实现多层级的运行时配置管理。系统核心围绕 `MainConfiguration` 单例展开，通过 `ApplicationPaths` 统一管理文件路径，支持“便携模式”与“安装模式”的双轨制存储策略。

### 1. 核心配置模型 (MainConfiguration)
- **存储格式**：使用 `Newtonsoft.Json` 进行序列化/反序列化，配置文件名为 `config.json`。
- **属性映射**：通过 `[JsonProperty]` 特性定义配置键名（如 `Connection.Host.Port`），支持嵌套命名空间风格的键名。
- **副作用逻辑**：部分配置项（如 `AutoStart`）在 Setter 中直接操作 Windows 注册表 (`HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`) 以实现开机自启管理。
- **默认值**：在属性初始化器中定义默认值（如端口 `8191`，主机 `127.0.0.1`）。

### 2. 路径管理与双轨制 (ApplicationPaths)
- **启动初始化**：在 `Program.cs` 入口点解析 `StartParameters` 后，立即调用 `ApplicationPaths.Initialize(portableMode)`。
- **路径策略**：
  - **便携模式** (`--portable`)：所有数据存储在程序目录下的 `Data` 文件夹内。
  - **安装模式**：数据存储在 `%APPDATA%\Macro Deck` 目录下。
- **自动创建**：系统启动时会自动检查并创建必要的子目录（如 `plugins`, `logs`, `backups`, `profiles` 等）。

### 3. 命令行参数 (StartParameters)
- **解析库**：使用 `CommandLineParser` 库解析启动参数。
- **关键参数**：
  - `--portable`：启用便携模式。
  - `--port`：覆盖配置文件中的 WebSocket 端口。
  - `--log-level`：动态调整日志级别。
  - `--force-update` / `--test-channel`：控制更新行为。
- **优先级**：命令行参数通常具有较高优先级，可直接覆盖配置文件中的部分设置（如端口）。

### 4. 配置生命周期
- **加载**：在 `MacroDeck.Start()` 中，若 `config.json` 存在则通过 `MainConfiguration.LoadFromFile` 加载；否则触发 `InitialSetup` 向导生成新配置。
- **保存**：通过 `MainConfiguration.Save(path)` 手动触发保存，通常在 UI 设置变更或初始化完成后调用。
- **插件配置**：插件拥有独立的配置目录 (`configs`)，并通过 `ISerializableConfiguration` 接口规范序列化行为。

### 5. 开发者规范
- **新增配置项**：应在 `MainConfiguration.cs` 中添加属性并标注 `[JsonProperty]`，同时提供合理的默认值。
- **路径引用**：严禁硬编码路径，必须通过 `ApplicationPaths` 类的静态属性获取文件或目录路径。
- **敏感信息**：SSL 密钥等敏感信息在配置文件中以加密字符串形式存储（如 `SslCertificateKeyPemEncrypted`）。
- **线程安全**：配置对象的读写主要在 UI 线程或启动阶段进行，若在多线程环境下修改需注意同步问题。