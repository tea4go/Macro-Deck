## 1. 核心系统与工具
该项目采用 **.NET (C#)** 作为主要开发语言，使用 **Inno Setup** 进行 Windows 安装包打包。构建流程完全托管于 **GitHub Actions**，实现了从代码提交、测试、版本管理到最终发布的全自动化 CI/CD 流水线。

- **编译框架**: .NET 10.0 (Windows Desktop)
- **包管理**: Central Package Management (CPM) via `Directory.Packages.props`
- **安装程序**: Inno Setup 6 (`setup/Macro Deck.iss`)
- **CI/CD**: GitHub Actions
- **Web 客户端集成**: Docker (用于构建前端资源)

## 2. 关键文件与目录
- **解决方案配置**: `Macro-Deck.slnx`, `Directory.Build.props`, `Directory.Packages.props`
- **主项目文件**: `src/MacroDeck/MacroDeck.csproj`
- **CI 工作流**:
  - `.github/workflows/ci.yml`: 触发 PR 和 Push 的入口。
  - `.github/workflows/tests.yml`: 执行单元测试。
  - `.github/workflows/build-push-windows.yml`: 核心构建脚本，负责编译、发布并调用 Inno Setup 生成 `.exe` 安装包。
  - `.github/workflows/create-release.yml`: 自动化版本号提升（Bump Version）并创建 GitHub Release。
  - `.github/workflows/pull-web-client.yml`: 从外部仓库拉取并构建 Web 客户端资源。
- **安装脚本**: `setup/Macro Deck.iss`

## 3. 架构与约定
### 3.1 依赖管理
项目启用了 **Central Package Management (CPM)**。所有 NuGet 包的版本统一在根目录的 `Directory.Packages.props` 中定义，子项目通过 `<PackageReference Include="PackageName" />` 引用，确保了全仓库依赖版本的一致性。

### 3.2 构建目标
- **目标框架**: `net10.0-windows10.0.22000.0`
- **运行时标识符 (RID)**: `win-x64`
- **自包含发布**: 启用 `SelfContained=true`，确保发布的二进制文件包含 .NET 运行时，用户无需单独安装 .NET SDK/Runtime。

### 3.3 发布流程
1. **版本提升**: 通过 `create-release.yml` 手动触发，根据选择的类型（major/minor/patch/beta）自动修改 `MacroDeck.csproj` 中的 `<Version>` 标签并提交。
2. **触发构建**: 创建 GitHub Release 后，`release-created.yml` 被触发。
3. **并行任务**:
   - **Windows 构建**: 编译代码 -> 注入 Sentry DSN -> 运行 Inno Setup -> 上传安装包至 Release -> 推送更新信息至 Update API。
   - **NuGet 发布**: 将核心库发布至 NuGet.org。
4. **Web 客户端同步**: 通过 `pull-web-client.yml` 手动触发，利用 Docker 容器构建前端静态资源，并自动创建 PR 合并至主分支。

## 4. 开发者规范
- **版本号管理**: 严禁手动修改 `MacroDeck.csproj` 中的版本号，应通过 GitHub Actions 的 `Create Release` 工作流进行自动化管理。
- **依赖添加**: 新增 NuGet 包时，必须在 `Directory.Packages.props` 中声明版本，禁止在 `.csproj` 中硬编码 `Version` 属性。
- **本地构建**: 
  ```bash
  dotnet restore
  dotnet build src/MacroDeck/MacroDeck.csproj -c Release -r win-x64
  ```
- **测试**: 提交前务必运行 `dotnet test tests/MacroDeck.Tests/MacroDeck.Tests.csproj` 确保单元测试通过。