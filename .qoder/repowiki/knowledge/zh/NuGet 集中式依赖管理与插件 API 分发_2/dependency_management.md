## 1. 使用的系统与工具
- **包管理器**：NuGet（基于 .NET SDK / MSBuild）。
- **版本管理策略**：启用 **中央包版本管理（Central Package Management, CPM）**。通过根目录的 `Directory.Packages.props` 统一声明所有 NuGet 包的版本号，子项目（`.csproj`）中仅引用包名而不指定版本，确保全仓库依赖版本的一致性。
- **目标框架统一**：在 `Directory.Build.props` 中统一设定 `<TargetFramework>net10.0-windows10.0.22000.0</TargetFramework>`，并开启 Nullable 和 ImplicitUsings，为所有项目提供一致的编译环境。
- **插件 API 分发机制**：利用 `MacroDeck.nuspec` 将宿主核心程序集打包为独立的 NuGet 包（`SuchByte.MacroDeck`），供第三方插件开发者在编译期引用，而运行时则由 Macro Deck 宿主进程直接提供程序集。

## 2. 关键文件与包
- **`Directory.Packages.props`**
  - 核心配置文件，设置 `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`。
  - 集中定义了所有第三方库的版本，包括：
    - **核心功能**：`AdvancedSharpAdbClient` (ADB 通信), `Cottle` (模板引擎), `Magick.NET` (图像处理), `Newtonsoft.Json` (序列化)。
    - **日志与监控**：`Serilog` 系列及 `Sentry.Serilog`。
    - **基础设施**：`Microsoft.Extensions.*` (DI, Hosting, Logging), `sqlite-net-pcl` (本地存储)。
    - **测试工具**：`NUnit`, `Microsoft.NET.Test.Sdk`, `coverlet.collector`。
- **`Directory.Build.props`**
  - 定义全局编译属性，如目标框架、空值安全策略等，减少各项目文件的重复配置。
- **`src/MacroDeck/MacroDeck.csproj`**
  - 主宿主项目。所有 `<PackageReference>` 均不包含 `Version` 属性，完全依赖中央配置。
  - 通过 `<FrameworkReference Include="Microsoft.AspNetCore.App" />` 引用 ASP.NET Core 框架，其版本由 SDK 决定，不纳入 NuGet 版本管理。
  - 配置为 `SelfContained=true` 且 `RuntimeIdentifier=win-x64`，意味着发布时会打包完整的 .NET 运行时和所有依赖库。
- **`src/MacroDeck/MacroDeck.nuspec`**
  - 插件 API 包定义文件。
  - `<dependencies>` 组为空，明确表明该包在运行时没有外部传递依赖。插件开发者引用此包后，无需再处理复杂的第三方库版本冲突，因为宿主环境已预加载了所有必要的库。

## 3. 架构与约定
- **集中化版本控制（CPM）**：
  - 解决了多项目解决方案中常见的“依赖地狱”问题。任何版本的升级只需在 `Directory.Packages.props` 中修改一处，即可同步到所有引用该项目的项目中。
- **宿主与插件的依赖边界隔离**：
  - **宿主（Host）**：负责引入、管理和承载所有第三方依赖。它是依赖的“唯一真相源”。
  - **插件（Plugin）**：作为类库运行在宿主进程中。通过引用空的依赖组 API 包，插件在编译时获得类型定义，在运行时直接使用宿主内存中已加载的程序集。这种设计简化了插件的开发和部署，避免了插件携带冗余或冲突的 DLL。
- **框架依赖处理**：
  - Windows 桌面能力（WPF/WinForms）通过 `Microsoft.NET.Sdk.WindowsDesktop` 集成。
  - Web 服务能力通过 `Microsoft.AspNetCore.App` 框架引用获得，确保了与 .NET 运行时的高度兼容性和性能。

## 4. 开发者应遵循的规则
- **新增或升级依赖**：
  - **严禁**在 `.csproj` 文件中硬编码版本号。
  - 必须在 `Directory.Packages.props` 中添加或更新 `<PackageVersion>` 条目。
  - 确保新引入的库与现有的目标框架 `net10.0-windows10.0.22000.0` 兼容。
- **插件开发规范**：
  - 插件项目应引用 `SuchByte.MacroDeck` NuGet 包。
  - 不要尝试在插件中重新引入或覆盖宿主已管理的第三方库版本（如 Serilog 或 Newtonsoft.Json），否则可能导致运行时类型加载异常（TypeLoadException）或方法缺失。
  - 理解插件的生命周期与宿主绑定，宿主提供的依赖版本即为运行时实际执行的版本。
- **测试项目维护**：
  - 测试项目（如 `MacroDeck.Tests`）同样遵循 CPM 规则，确保测试环境与生产环境的依赖版本严格一致，提高测试结果的可靠性。
- **发布配置注意**：
  - 由于主项目配置为自包含（Self-contained）发布，构建产物体积较大但具备独立运行能力。若需调整发布策略（如改为框架依赖发布），需同步评估对依赖解析和最终用户安装体验的影响。