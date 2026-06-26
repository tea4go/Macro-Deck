## 1. 核心构建体系
Macro Deck 采用 **.NET 10 (net10.0-windows)** 作为基础运行时，结合 **Windows Forms** 和 **WPF** 进行桌面端开发。项目使用 **MSBuild** 驱动的标准 .NET 构建流程，并通过 **Inno Setup** 完成 Windows 平台的安装包打包。

### 关键配置
- **全局属性 (`Directory.Build.props`)**：统一指定目标框架为 `net10.0-windows10.0.22000.0`，并启用 Nullable 和 Implicit Usings。
- **集中式依赖管理 (`Directory.Packages.props`)**：开启 `ManagePackageVersionsCentrally`，在根目录统一管理所有 NuGet 包的版本，确保主程序与测试项目依赖一致。
- **主程序配置 (`src/MacroDeck/MacroDeck.csproj`)**：
  - 设置为 `WinExe` 输出类型，支持自包含（SelfContained）发布。
  - 集成 `Microsoft.AspNetCore.App` 以支持内置的 WebSocket 服务器和 Web Client 静态资源托管。
  - 版本号硬编码在 `.csproj` 中（如 `2.15.0-b10`），发布时由 Inno Setup 自动读取文件版本信息。

## 2. CI/CD 流水线策略
由于本项目是官方仓库的 Fork 版本，CI 流程进行了精简，去除了依赖官方私有资源和 API 的自动化发布环节。

### 现有工作流 (`.github/workflows/`)
| 工作流 | 触发条件 | 功能描述 |
| :--- | :--- | :--- |
| **CI** | Push / PR | 编排入口，自动调用测试工作流。 |
| **Run Tests** | 被 CI 调用 | 在 `windows-latest` 环境下执行 `dotnet restore` 和 `dotnet test`。 |
| **Build Local** | 手动触发 | 执行 `dotnet publish` 生成自包含的 `win-x64` 产物，并上传为 GitHub Artifact。 |

### 本地化发布流程
- **构建命令**：`dotnet publish "src\MacroDeck\MacroDeck.csproj" -c Release -r win-x64 --self-contained true -o publish`
- **配套脚本**：提供 `update-macrodeck-local.ps1` 脚本，可触发 `build-local.yml` 工作流，下载最新的 Artifact 并自动替换本地安装文件，实现“云端编译、本地热更”的开发调试闭环。

## 3. 安装包打包 (Inno Setup)
项目使用 **Inno Setup** (`setup/Macro Deck.iss`) 生成最终的 `.exe` 安装程序。
- **动态版本获取**：脚本通过 `GetStringFileInfo` 读取编译后主程序的 `ProductVersion` 作为安装包版本。
- **环境依赖处理**：内置逻辑检测并自动安装 **VC++ Redistributable (v14.14+)**，确保运行环境完整。
- **系统集成**：安装过程中自动配置 Windows Defender 防火墙规则，允许程序进行入站和出站通信。
- **多语言支持**：支持包括中文、英文、德文等在内的 20 余种安装界面语言。

## 4. 开发者规范
- **依赖添加**：新增 NuGet 包时，必须在 `Directory.Packages.props` 中声明版本，并在具体项目中引用，禁止在 `.csproj` 中硬编码版本号。
- **测试要求**：提交 PR 前需确保 `tests/MacroDeck.Tests` 中的单元测试能够通过，CI 会自动拦截失败的构建。
- **版本管理**：若需修改软件版本，应同步更新 `src/MacroDeck/MacroDeck.csproj` 中的 `<Version>` 标签。
