param(
    [switch]$Build,
    [string]$Configuration = "Release",
    [string]$Output = "publish"
)

if (-not $Build) {
    Write-Host "用法: .\run_win.ps1 -Build [-Configuration <Release|Debug>] [-Output <目录>]"
    Write-Host ""
    Write-Host "  -Build           执行构建和发布"
    Write-Host "  -Configuration   构建配置（默认: Release）"
    Write-Host "  -Output          输出目录（默认: publish）"
    exit 0
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "未找到 .NET SDK，请安装：https://dotnet.microsoft.com/download/dotnet/10.0"
    exit 1
}

$proj = "src\MacroDeck\MacroDeck.csproj"

Write-Host "正在还原依赖..."
dotnet restore $proj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "正在发布 ($Configuration)..."
dotnet publish $proj -c $Configuration -r win-x64 --self-contained true -o $Output
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$batLines = @(
    '@echo off'
    'cd /d "%~dp0"'
    'start "" "Macro Deck 2.exe" --portable'
)
[System.IO.File]::WriteAllText("$Output\启动.bat", ($batLines -join "`r`n") + "`r`n", [System.Text.Encoding]::Default)

Write-Host "完成 -> $Output"
