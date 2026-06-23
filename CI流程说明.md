# CI 流程说明

本文档说明本仓库（tea4go/Macro-Deck，Macro Deck 主程序的 fork）的 GitHub Actions 工作流。

## 背景

Macro Deck 上游仓库带有一整套面向官方发布的 CI 流程：编译官方安装包、推送到官方更新 API、发布 NuGet 包、从私有仓库拉取 Web Client 等。这些流程依赖只有官方组织才拥有的 secrets、私有仓库访问权限和官方 API，**在 fork 中无法运行**。

因此本 fork 精简为三个真正可用的工作流，其余五个已删除。

## 保留的工作流

| 工作流 | 文件 | 触发方式 | 作用 |
|--------|------|----------|------|
| CI | `ci.yml` | push / 向 main、production 提 PR | 编排测试，调用 `tests.yml` |
| Run Tests | `tests.yml` | 被 `ci.yml` 调用（workflow_call） | 实际执行 `dotnet test`，运行单元测试 |
| Build Local | `build-local.yml` | 手动触发（workflow_dispatch） | 编译主程序并上传产物（artifact），供本地更新脚本下载安装 |

### 调用关系

```
ci.yml  (push / PR 触发)
   └── 调用 tests.yml  (dotnet test)

build-local.yml  (手动触发)
   └── dotnet publish → 上传 artifact: macro-deck-build
```

`build-local.yml` 与本地脚本 `update-macrodeck-local.ps1` 配合：脚本触发该工作流、等待完成、下载 artifact 并替换本地安装。

## 已删除的工作流及原因

以下五个工作流依赖 fork 不具备的资源，运行必然失败，已删除：

| 已删除 | 原作用 | 删除原因 |
|--------|--------|----------|
| `create-release.yml` | 官方发版：查官方版本号 API、创建 GitHub Release | 依赖 `secrets.PAT` 与官方 action `Macro-Deck-App/Actions/fetch-version`、`create-github-release` |
| `release-created.yml` | release 发布后的总编排（校验版本、触发构建/发包/通知） | 依赖官方更新 API `update.api.macro-deck.app` 校验、`secrets.WEBHOOK_KEY`、Discord 通知 action |
| `build-push-windows.yml` | 用 Inno Setup 打官方安装包并推送官方更新 API | 依赖 `secrets.UPDATE_SERVICE_TOKEN`、官方 action `push-to-update-api`、`secrets.SENTRY_DSN` |
| `publish-nuget.yml` | 发布 NuGet 包到 nuget.org | 依赖 `secrets.NUGET_USER`（官方 NuGet 账号 OIDC 登录） |
| `pull-web-client.yml` | 从私有仓库拉取并编译 Web Client | 依赖 `secrets.PRIVATE_REPO_PAT`，访问私有仓库 `Macro-Deck-App/Macro-Deck-Client-App` |

## 如何恢复官方发版流程

这些工作流已从 git 历史中删除，但仍可从上游仓库找回。若将来需要官方风格的发版，需要：

1. 从上游 `Macro-Deck-App/Macro-Deck` 复制对应的 `.yml` 文件回 `.github/workflows/`。
2. 在仓库 Settings → Secrets 中配置所需的 secrets（PAT、UPDATE_SERVICE_TOKEN、NUGET_USER、PRIVATE_REPO_PAT、SENTRY_DSN、WEBHOOK_KEY 等）。
3. 注意官方 API 校验（如 `update.api.macro-deck.app`）对第三方 fork 可能不可用。

对于 fork 的日常使用，`build-local.yml` + `update-macrodeck-local.ps1` 已足够：编译最新代码并更新本地安装。
