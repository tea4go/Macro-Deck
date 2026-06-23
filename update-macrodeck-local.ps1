<#
.SYNOPSIS
    更新本地 Macro Deck：可下载最近一次成功的 CI 编译产物并安装，
    也可触发新构建、查询版本号或查询最近的 CI 状态。
    不带任何参数或使用 -Help 时显示帮助。
.DESCRIPTION
    使用 -Install 时：
        下载最近一次成功的 build-local 编译产物（artifact），关闭 Macro Deck，
        复制程序文件到安装目录，再重启。不会触发新的 CI 编译。
    只复制产物中的文件（不含子目录），因此用户数据目录
    （plugins、wwwroot、Android Debug Bridge）不会被改动。
.PARAMETER Install
    下载最近一次成功的 build-local 编译产物并安装到本地（不触发新构建）。
.PARAMETER Build
    触发一次新的 build-local CI 编译，等待完成后再下载安装。
.PARAMETER Version
    仅查询 GitHub 上最新的版本号（main 分支 csproj 的 Version 及最新 Release），然后退出。
.PARAMETER Status
    仅查询最近 5 次 CI 构建状态，然后退出。
.PARAMETER Help
    显示帮助信息。
.PARAMETER Repo
    GitHub 仓库，格式为 owner/repo。
.PARAMETER InstallDir
    Macro Deck 安装目录。
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1 -Install
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1 -Build
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1 -Version
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1 -Status
#>

param(
    [switch]$Install,
    [switch]$Build,
    [switch]$Version,
    [switch]$Status,
    [switch]$Help,
    [string]$Repo = "tea4go/Macro-Deck",
    [string]$InstallDir = "C:\Program Files\Macro Deck"
)

$ErrorActionPreference = "Stop"
# 控制台按 UTF-8 输出，确保中文不乱码
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }
$MacroDeckProcessName = "Macro Deck 2"
$WorkflowFile = "build-local.yml"

function Write-Step($m) { Write-Host "[信息] $m" -ForegroundColor Cyan }
function Write-Ok($m)   { Write-Host "[成功] $m" -ForegroundColor Green }
function Write-Fail($m) { Write-Host "[失败] $m" -ForegroundColor Red }

function Show-Help {
    Write-Host "=== Macro Deck 本地更新工具 ===" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "用法：" -ForegroundColor Yellow
    Write-Host "  powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1 [参数]"
    Write-Host ""
    Write-Host "参数：" -ForegroundColor Yellow
    Write-Host "  -Install     下载最近一次成功的 CI 编译产物并安装到本地（不触发新构建）"
    Write-Host "  -Build       触发一次新的 CI 编译，等待完成后再下载安装"
    Write-Host "  -Version     仅查询最新版本号（main 分支 csproj 的 Version 及最新 Release）"
    Write-Host "  -Status      仅查询最近 5 次 CI 构建状态"
    Write-Host "  -Help        显示本帮助"
    Write-Host "  -Repo        GitHub 仓库（owner/repo），默认 tea4go/Macro-Deck"
    Write-Host "  -InstallDir  Macro Deck 安装目录，默认 C:\Program Files\Macro Deck"
    Write-Host ""
    Write-Host "示例：" -ForegroundColor Yellow
    Write-Host "  .\update-macrodeck-local.ps1 -Install    # 安装最新编译产物"
    Write-Host "  .\update-macrodeck-local.ps1 -Build      # 重新编译后安装"
    Write-Host "  .\update-macrodeck-local.ps1 -Version    # 查看版本号"
    Write-Host "  .\update-macrodeck-local.ps1 -Status     # 查看最近构建状态"
    Write-Host ""
    Write-Host "不带任何参数或使用 -Help 时显示本帮助。" -ForegroundColor DarkGray
}

# 不带任何参数 或 -Help 时显示帮助并退出
if ($Help -or -not ($Install -or $Build -or $Version -or $Status)) {
    Show-Help
    exit 0
}

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Fail "未找到 gh CLI，请从 https://cli.github.com 安装"
    exit 1
}

# ---- 查询最新版本号 ----
function Get-LatestVersion {
    Write-Step "正在查询最新版本号 ..."
    # 从 main 分支 csproj 读取 Version
    try {
        $b64 = gh api "repos/$Repo/contents/src/MacroDeck/MacroDeck.csproj?ref=main" -q '.content'
        $csproj = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(($b64 -replace '\s','')))
        $m = [regex]::Match($csproj, '<Version>([^<]+)</Version>')
        if ($m.Success) { Write-Ok "main 分支 csproj 版本：$($m.Groups[1].Value)" }
        else { Write-Fail "未能从 csproj 解析版本号" }
    } catch { Write-Fail "读取 csproj 失败：$($_.Exception.Message)" }

    # 最新 Release（可能没有）
    try {
        $tag = gh api "repos/$Repo/releases/latest" -q '.tag_name' 2>$null
        if ($tag) { Write-Ok "最新 Release 标签：$tag" }
        else { Write-Host "  （仓库暂无正式 Release）" }
    } catch { Write-Host "  （仓库暂无正式 Release）" }
}

# ---- 查询最近 5 次 CI 状态 ----
function Get-RecentStatus {
    Write-Step "正在查询最近 5 次 CI 构建状态 ..."
    $runs = gh api "repos/$Repo/actions/runs?per_page=5" | ConvertFrom-Json | Select-Object -ExpandProperty workflow_runs
    if (-not $runs) { Write-Host "  （暂无运行记录）"; return }
    foreach ($r in $runs) {
        $concl = if ($r.conclusion) { $r.conclusion } else { "进行中" }
        Write-Host ("  [{0}] {1} | 分支={2} | 状态={3} | 结果={4} | {5}" -f `
            $r.id, $r.name, $r.head_branch, $r.status, $concl, $r.created_at)
    }
}

if ($Version) { Get-LatestVersion; exit 0 }
if ($Status)  { Get-RecentStatus; exit 0 }

Write-Host "=== Macro Deck 本地更新工具 ===" -ForegroundColor Cyan

# ---- 触发新构建（仅 -Build） ----
if ($Build) {
    Write-Step "正在 $Repo 触发 $WorkflowFile 工作流 ..."
    gh workflow run $WorkflowFile --repo $Repo
    if ($LASTEXITCODE -ne 0) { Write-Fail "触发工作流失败"; exit 1 }
    Write-Ok "工作流已触发。"
    Start-Sleep -Seconds 8  # 给 GitHub 一点时间登记本次运行

    Write-Step "正在等待编译完成（大约需要 5 分钟）..."
    $RunId = $null
    $deadline = (Get-Date).AddMinutes(20)
    while ((Get-Date) -lt $deadline) {
        $runs = gh api "repos/$Repo/actions/workflows/$WorkflowFile/runs?per_page=5" |
            ConvertFrom-Json | Select-Object -ExpandProperty workflow_runs
        $latest = $runs | Select-Object -First 1
        if ($latest) {
            $RunId = $latest.id
            Write-Host "  运行 $RunId ：状态=$($latest.status) 结果=$($latest.conclusion)"
            if ($latest.status -eq "completed") {
                if ($latest.conclusion -eq "success") { Write-Ok "编译成功。"; break }
                else { Write-Fail "编译结果为 $($latest.conclusion)，请查看 https://github.com/$Repo/actions/runs/$RunId"; exit 1 }
            }
        }
        Start-Sleep -Seconds 30
    }
    if (-not $RunId) { Write-Fail "未找到编译运行记录"; exit 1 }
}
else {
    # 默认：使用最近一次成功的 build-local 产物，不触发新构建
    Write-Step "正在查找最近一次成功的编译产物 ..."
    $runs = gh api "repos/$Repo/actions/workflows/$WorkflowFile/runs?per_page=20" |
        ConvertFrom-Json | Select-Object -ExpandProperty workflow_runs
    $RunId = ($runs | Where-Object { $_.status -eq "completed" -and $_.conclusion -eq "success" } |
        Select-Object -First 1).id
    if (-not $RunId) {
        Write-Fail "未找到成功的编译产物。请先用 -Build 触发一次构建。"
        exit 1
    }
    Write-Ok "使用编译运行 $RunId 的产物。"
}

# ---- 下载产物 ----
$TempDir = Join-Path $env:TEMP "macro-deck-build-$(Get-Date -Format yyyyMMdd_HHmmss)"
New-Item -ItemType Directory -Force -Path $TempDir | Out-Null
Write-Step "正在下载产物到 $TempDir ..."
gh run download $RunId --repo $Repo --name macro-deck-build --dir $TempDir
if ($LASTEXITCODE -ne 0) { Write-Fail "下载产物失败"; exit 1 }
Write-Ok "下载完成。"

# 产物可能位于子目录，查找包含 exe 的根目录
$PublishRoot = Get-ChildItem -Path $TempDir -Filter "Macro Deck 2.exe" -Recurse | Select-Object -First 1
if (-not $PublishRoot) { Write-Fail "下载的产物中未找到 Macro Deck 2.exe"; exit 1 }
$PublishRoot = $PublishRoot.DirectoryName
Write-Host "产物根目录：$PublishRoot"

# ---- 关闭 Macro Deck ----
Write-Step "正在关闭 Macro Deck ..."
$proc = Get-Process -Name $MacroDeckProcessName -ErrorAction SilentlyContinue
if ($proc) {
    $proc | Stop-Process -Force
    Start-Sleep -Seconds 2
    Write-Ok "Macro Deck 已关闭。"
} else {
    Write-Host "Macro Deck 当前未运行。"
}

# ---- 复制程序文件（只复制文件，保留 plugins/wwwroot/Android Debug Bridge 等目录）----
Write-Step "正在复制程序文件到 $InstallDir ..."
$count = 0
Get-ChildItem -Path $PublishRoot -File | ForEach-Object {
    $dest = Join-Path $InstallDir $_.Name
    Copy-Item -Path $_.FullName -Destination $dest -Force
    $count++
}
Write-Ok "已复制 $count 个文件。"

# 校验更新后的 DLL 时间
$dll = Get-Item (Join-Path $InstallDir "Macro Deck 2.dll") -ErrorAction SilentlyContinue
if ($dll) { Write-Host "已安装 DLL 的修改时间：$($dll.LastWriteTime)" }

# 清理临时目录
Remove-Item -Path $TempDir -Recurse -Force -ErrorAction SilentlyContinue

# ---- 启动 Macro Deck ----
Write-Step "正在启动 Macro Deck ..."
Start-Process -FilePath (Join-Path $InstallDir "Macro Deck 2.exe") -WorkingDirectory $InstallDir
Write-Ok "Macro Deck 已启动。"
Write-Ok "完成。"
