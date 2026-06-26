Macro-Deck 采用基于 .NET SDK 的集中式构建体系，结合 GitHub Actions 实现自动化 CI/CD、版本管理与多平台分发。核心特征如下：

### 1. 技术栈与工具链
- **编译框架**：.NET 10.0 (Windows Desktop)，同时启用 Windows Forms 和 WPF。
- **依赖管理**：采用 `Directory.Packages.props` 实现 NuGet 包的集中式版本控制（Central Package Management）。
- **打包工具**：使用 Inno Setup (`setup/Macro Deck.iss`) 生成 Windows 安装程序；使用 `nuget pack` 生成插件开发 API 包。
- **前端集成**：通过 Docker 容器化构建 Angular/Ionic Web Client，并自动同步至主仓库 `wwwroot/client` 目录。

### 2. 核心构建流程
- **本地/CI 编译**：通过 `dotnet build` 和 `dotnet publish` 进行自包含（Self-contained）发布，目标运行时为 `win-x64`。
- **安装程序生成**：在 CI 中调用 Inno Setup Compiler (`ISCC.exe`)，将发布的二进制文件、ADB 组件及 VC++ 运行时分发逻辑打包为 `.exe` 安装包。
- **Web 客户端同步**：利用 `pull-web-client.yml` 工作流，从私有仓库拉取最新前端代码，在 Docker 中编译后提取静态资源并提交 PR。

### 3. 自动化发布策略
- **版本管理**：通过 `create-release.yml` 手动触发版本升级（支持 major/minor/patch 及 beta 分支），自动更新 `.csproj` 中的 `<Version>` 标签。
- **多渠道分发**：
  - **GitHub Releases**：上传 Windows 安装程序并计算 SHA256 校验码。
  - **NuGet.org**：发布 `MacroDeck.nuspec` 定义的 API 库，供第三方插件开发者引用。
  - **更新服务**：构建完成后自动调用 Update API 推送新版本元数据，触发客户端更新提示。

### 4. 开发者规范
- **版本号维护**：版本号统一在 `src/MacroDeck/MacroDeck.csproj` 中定义，格式为 `Major.Minor.Patch-betaN`。
- **敏感配置注入**：Sentry DSN 等密钥通过 GitHub Secrets 在构建时动态注入 `SentryConfiguration.cs`，严禁硬编码。
- **测试门禁**：所有 PR 必须通过 `tests.yml` 触发的 NUnit 单元测试方可合并。