Macro Deck 采用基于 JSON 文件的集中式配置管理方案，结合命令行参数（CLI）和 Windows 注册表实现多层级的运行时配置。系统核心围绕 `MainConfiguration` 类展开，通过 `ApplicationPaths` 统一管理文件路径，并利用 `StartParameters` 处理启动时的动态覆盖。

### 1. 核心配置模型
- **主配置文件 (`config.json`)**：由 `MainConfiguration` 类定义，使用 `Newtonsoft.Json` 进行序列化/反序列化。涵盖自动启动、更新策略、网络监听（Host/Port）、SSL 证书、ADB 连接、界面语言及字体等全局设置。
- **设备配置 (`devices.json`)**：存储已配对设备的连接信息与个性化设置（如亮度、唤醒锁定）。
- **插件配置**：通过 `ISerializableConfiguration` 接口提供统一的 JSON 序列化能力，插件可将配置持久化在用户数据目录的 `configs` 子目录下。

### 2. 路径管理与便携模式
- **双模式支持**：`ApplicationPaths` 根据启动参数 `--portable` 决定数据存储位置。
  - **标准模式**：数据存储在 `%APPDATA%\Macro Deck`。
  - **便携模式**：数据存储在程序根目录下的 `Data` 文件夹。
- **自动初始化**：系统在启动时自动检查并创建必要的目录结构（如 `plugins`, `logs`, `backups`, `profiles`）。

### 3. 配置加载与分层逻辑
1. **命令行解析**：`Program.cs` 入口首先通过 `CommandLine` 库解析 `StartParameters`（如端口、日志级别、调试模式）。
2. **路径初始化**：根据 `--portable` 标志初始化 `ApplicationPaths`。
3. **日志引导**：在加载主配置前，先初始化基础日志系统（`LoggingConfig`），确保启动过程可追踪。
4. **主配置加载**：若 `config.json` 存在，则加载并应用语言、字体等设置；若不存在，则触发 `InitialSetup` 向导。
5. **动态覆盖**：部分设置（如监听端口）允许通过 CLI 参数临时覆盖配置文件中的值。

### 4. 特殊配置处理
- **开机自启**：`MainConfiguration.AutoStart` 属性的 Setter 直接操作 Windows 注册表 `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`，实现配置变更即生效。
- **字体管理**：`FontManager` 在窗口创建前通过 `Application.SetDefaultFont` 设置兜底字体，并在运行时递归刷新控件树的字体族与字号，支持实时预览。
- **敏感信息**：SSL 私钥等敏感字段在配置中以加密字符串形式存储（`SslCertificateKeyPemEncrypted`）。

### 5. 开发者规范
- **新增配置项**：应在 `MainConfiguration` 中添加属性并使用 `[JsonProperty]` 标记，同时提供合理的默认值以确保向后兼容。
- **插件配置**：实现 `ISerializableConfiguration` 接口，利用 `Serialize()` 和 `Deserialize<T>()` 方法处理持久化。
- **路径引用**：严禁硬编码路径，必须通过 `ApplicationPaths` 静态属性获取文件或目录位置。