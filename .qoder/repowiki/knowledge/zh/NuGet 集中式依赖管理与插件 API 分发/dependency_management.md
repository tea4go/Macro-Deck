## 1. 使用的系统与工具
- **包管理器**：NuGet（.NET SDK / MSBuild）。
- **版本管理策略**：启用 **中央包版本管理（Central Package Management, CPM）**，通过 `Directory.Packages.props` 统一声明所有 NuGet 包的版本，项目级 `.csproj` 中仅引用包名而不写版本号。
- **目标框架**：在 `Directory.Build.props` 中统一设定为 `net10.0-windows10.0.22000.0`，确保所有项目使用一致的运行时与 API 表面。
- **插件 API 分发**：通过 `MacroDeck.nuspec` 将宿主程序集打包为 **编译时引用、运行时由宿主提供** 的 NuGet 包，用于插件开发者依赖。

## 2. 关键文件与包
- **`Directory.Packages.props`**
  - 开启 `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`。
  - 集中定义所有第三方库版本，例如：
    - `AdvancedSharpAdbClient`、`Cottle`、`Magick.NET-*`、`Newtonsoft.Json`、`QRCoder`
    - `Serilog` 系列及 `Sentry.Serilog`
    - `sqlite-net-pcl`
    - `Microsoft.Extensions.*`（Hosting、DI、Logging.Abstractions）
    - 测试相关：`Microsoft.NET.Test.Sdk`、`NUnit`、`NUnit3TestAdapter`、`coverlet.collector` 等。
- **`Directory.Build.props`**
  - 统一设置 `<TargetFramework>net10.0-windows10.0.22000.0</TargetFramework>`、Nullable、ImplicitUsings 等基础编译属性。
- **`src/MacroDeck/MacroDeck.csproj`**
  - 作为主宿主项目，所有 `<PackageReference>` **不带 Version**，版本完全来自中央配置。
  - 引用 `Microsoft.AspNetCore.App` 框架引用（`<FrameworkReference>`），不通过 NuGet 单独管理 ASP.NET Core 组件版本。
  - 配置 `RuntimeIdentifier=win-x64` 与 `<SelfContained>true</SelfContained>`，影响最终发布时的依赖打包方式（将运行时与依赖一起发布）。
- **`tests/MacroDeck.Tests/MacroDeck.Tests.csproj`**
  - 测试项目同样通过中央管理引入 NUnit 及相关工具包，并通过 `<ProjectReference>` 依赖主项目。
- **`src/MacroDeck/MacroDeck.nuspec`**
  - 定义插件 API 包 `SuchByte.MacroDeck`，目标框架与宿主一致。
  - `<dependencies>` 中仅保留空的 dependency group，明确说明：**插件在编译期依赖该包，但运行时由 Macro Deck 宿主提供程序集，不再传递第三方依赖**。

## 3. 架构与约定
- **集中化版本控制**：
  - 所有第三方库版本集中在根目录的 `Directory.Packages.props`，避免不同项目出现版本分歧。
  - 主项目与测试项目都遵循“只声明包名，不声明版本”的约定。
- **宿主 vs 插件依赖边界**：
  - 宿主项目负责引入并承载所有第三方依赖（日志、序列化、图像处理、ADB 客户端等）。
  - 插件 API 包刻意不包含 runtime 依赖，插件开发者只需引用 API 程序集，实际运行时期望宿主已加载所需库。
- **框架依赖处理**：
  - ASP.NET Core 相关能力通过 `Microsoft.AspNetCore.App` 框架引用获得，版本由 SDK/运行时决定，不纳入 NuGet 版本管理。
  - Windows 桌面相关能力通过 `Microsoft.NET.Sdk.WindowsDesktop` 与 WPF/WinForms 集成。

## 4. 开发者应遵循的规则
- **新增或升级第三方库时**：
  - 必须在 `Directory.Packages.props` 中添加或修改 `<PackageVersion>`，而不是在 `.csproj` 中直接写版本。
  - 保持所有项目对同一包的版本一致，避免隐式冲突。
- **开发插件时**：
  - 依赖 `SuchByte.MacroDeck` NuGet 包作为编译期引用，不要假设可以独立携带或替换宿主中的第三方库版本。
  - 理解插件运行在宿主进程中，宿主提供的依赖版本即为运行时版本。
- **测试项目**：
  - 测试相关的包（NUnit、Test.Sdk、coverlet 等）也通过中央管理统一版本，避免测试环境与主项目依赖不一致。
- **自包含发布注意**：
  - 由于主项目配置为 `SelfContained=true` 且限定 `win-x64`，构建发布产物时会打包运行时与依赖；如需调整发布形态，应同步评估对依赖解析和部署大小的影响。