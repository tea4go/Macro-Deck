param(
    [switch]$Build,
    [switch]$Run,
    [switch]$Portable,
    [string]$Configuration = "Release",
    [string]$Output = "publish"
)

if (-not $Build -and -not $Run) {
    Write-Host "用法: .\run_win.ps1 [-Build] [-Run] [-Portable] [-Configuration <Release|Debug>] [-Output <目录>]"
    Write-Host ""
    Write-Host "  -Build           执行构建和发布"
    Write-Host "  -Run             退出旧版本并运行已发布的最新版本"
    Write-Host "  -Portable        以便携模式运行（数据存于输出目录的 Data 子目录）"
    Write-Host "                   默认不加此参数，使用标准模式（数据存于 %APPDATA%\Macro Deck）"
    Write-Host "  -Configuration   构建配置（默认: Release）"
    Write-Host "  -Output          输出目录（默认: publish）"
    exit 0
}

$exeName = "Macro Deck 2"

if ($Build) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Error "未找到 .NET SDK，请安装：https://dotnet.microsoft.com/download/dotnet/10.0"
        exit 1
    }

    # 构建前先退出正在运行的旧版本，避免文件被锁
    Get-Process -Name $exeName -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Seconds 1

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
    $batPath = Join-Path (Resolve-Path $Output).Path "启动.bat"
    [System.IO.File]::WriteAllText($batPath, ($batLines -join "`r`n") + "`r`n", [System.Text.Encoding]::Default)

    Write-Host "完成 -> $Output"
}

if ($Run) {
    if (-not (Test-Path $Output)) {
        Write-Error "输出目录不存在：$Output，请先执行 -Build"
        exit 1
    }

    $exePath = Join-Path (Resolve-Path $Output).Path "$exeName.exe"
    if (-not (Test-Path $exePath)) {
        Write-Error "未找到可执行文件：$exePath，请先执行 -Build"
        exit 1
    }

    # 退出旧版本
    $running = Get-Process -Name $exeName -ErrorAction SilentlyContinue
    if ($running) {
        Write-Host "正在退出旧版本..."
        $running | Stop-Process -Force
        Start-Sleep -Seconds 2
    }

    Write-Host "正在启动 $exeName..."
    if ($Portable) {
        Start-Process -FilePath $exePath -ArgumentList "--portable"
    } else {
        Start-Process -FilePath $exePath
    }
}
