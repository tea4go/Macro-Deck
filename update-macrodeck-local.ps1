<#
.SYNOPSIS
    触发 build-local CI 工作流，等待编译完成，再用最新编译产物更新本地 Macro Deck。
.DESCRIPTION
    1. 在 tea4go/Macro-Deck 仓库触发 .github/workflows/build-local.yml。
    2. 轮询等待该次运行完成。
    3. 下载 macro-deck-build 产物。
    4. 关闭 Macro Deck，复制程序文件到安装目录，再重启。
    只复制产物中的文件（不复制子目录），因此用户数据目录
    （plugins、wwwroot、Android Debug Bridge）不会被改动。
.PARAMETER Repo
    GitHub 仓库，格式为 owner/repo。
.PARAMETER InstallDir
    Macro Deck 安装目录。
.PARAMETER SkipBuild
    跳过触发新编译，直接使用最近一次成功运行的产物。
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1
    powershell -ExecutionPolicy Bypass -File .\update-macrodeck-local.ps1 -SkipBuild
#>

param(
    [string]$Repo = "tea4go/Macro-Deck",
    [string]$InstallDir = "C:\Program Files\Macro Deck",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
# 控制台按 UTF-8 输出，确保中文不乱码
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }
$MacroDeckProcessName = "Macro Deck 2"

function Write-Step($m) { Write-Host "[信息] $m" -ForegroundColor Cyan }
function Write-Ok($m)   { Write-Host "[成功] $m" -ForegroundColor Green }
function Write-Fail($m) { Write-Host "[失败] $m" -ForegroundColor Red }

Write-Host "=== Macro Deck 本地更新工具 ===" -ForegroundColor Cyan

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Fail "未找到 gh CLI，请从 https://cli.github.com 安装"
    exit 1
}

if (-not $SkipBuild) {
    Write-Step "正在 $Repo 触发 build-local 工作流 ..."
    gh workflow run build-local.yml --repo $Repo
    if ($LASTEXITCODE -ne 0) { Write-Fail "触发工作流失败"; exit 1 }
    Write-Ok "工作流已触发。"
    Start-Sleep -Seconds 8  # 给 GitHub 一点时间登记本次运行
}

# 查找最新的 build-local 运行并等待其完成
Write-Step "正在等待编译完成（大约需要 5 分钟）..."
$RunId = $null
$deadline = (Get-Date).AddMinutes(20)
while ((Get-Date) -lt $deadline) {
    $runs = gh api "repos/$Repo/actions/workflows/build-local.yml/runs?per_page=5" |
        ConvertFrom-Json |
        Select-Object -ExpandProperty workflow_runs
    $latest = $runs | Select-Object -First 1
    if ($latest) {
        $RunId = $latest.id
        $status = $latest.status
        $conclusion = $latest.conclusion
        Write-Host "  运行 $RunId ：状态=$status 结果=$conclusion"
        if ($status -eq "completed") {
            if ($conclusion -eq "success") { Write-Ok "编译成功。"; break }
            else { Write-Fail "编译结果为 $conclusion，请查看 https://github.com/$Repo/actions/runs/$RunId"; exit 1 }
        }
    }
    Start-Sleep -Seconds 30
}
if (-not $RunId) { Write-Fail "未找到编译运行记录"; exit 1 }

# 下载产物
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

# 关闭 Macro Deck
Write-Step "正在关闭 Macro Deck ..."
$proc = Get-Process -Name $MacroDeckProcessName -ErrorAction SilentlyContinue
if ($proc) {
    $proc | Stop-Process -Force
    Start-Sleep -Seconds 2
    Write-Ok "Macro Deck 已关闭。"
} else {
    Write-Host "Macro Deck 当前未运行。"
}

# 只复制文件（不含子目录），以保留 plugins/wwwroot/Android Debug Bridge 等用户目录
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

# 启动 Macro Deck
Write-Step "正在启动 Macro Deck ..."
Start-Process -FilePath (Join-Path $InstallDir "Macro Deck 2.exe") -WorkingDirectory $InstallDir
Write-Ok "Macro Deck 已启动。"
Write-Ok "完成。"
