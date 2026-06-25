param(
    [switch]$Build,
    [switch]$Run,
    [switch]$Portable,
    [string]$Configuration = "Release",
    [string]$Output = "C:\Program Files\Macro Deck"
)

if (-not $Build -and -not $Run) {
    Write-Host "用法: .\run_win.ps1 [-Build] [-Run] [-Portable] [-Configuration <Release|Debug>] [-Output <目录>]"
    Write-Host ""
    Write-Host "  -Build           执行构建和发布"
    Write-Host "  -Run             退出旧版本并运行已发布的最新版本"
    Write-Host "  -Portable        以便携模式运行（数据存于输出目录的 Data 子目录）"
    Write-Host "                   默认不加此参数，使用标准模式（数据存于 %APPDATA%\Macro Deck）"
    Write-Host "  -Configuration   构建配置（默认: Release）"
    Write-Host "  -Output          输出目录（默认: C:\Program Files\Macro Deck，与标准安装目录一致）"
    Write-Host "                   注意：写入该目录需以管理员身份运行"
    exit 0
}

$exeName = "Macro Deck 2"

function Set-FirewallRule {
    $rule = "Macro Deck 2"
    if (-not (Get-NetFirewallRule -DisplayName $rule -ErrorAction SilentlyContinue)) {
        try {
            New-NetFirewallRule -DisplayName $rule -Direction Inbound -Protocol TCP -LocalPort 8191 -Action Allow | Out-Null
            Write-Host "防火墙已放行: $rule (TCP 8191)"
        } catch {
            Write-Warning "添加防火墙规则需要管理员权限，请以管理员身份运行"
        }
    } else {
        Write-Host "防火墙规则已存在: $rule"
    }
}

function Update-HostAddress {
    $wifiIP = (Get-NetIPAddress -AddressFamily IPv4 |
        Where-Object { $_.InterfaceAlias -match 'Wi-Fi|WLAN|Wireless' -and $_.IPAddress -notlike '169.254*' } |
        Select-Object -First 1).IPAddress
    if (-not $wifiIP) { Write-Host "未检测到 Wi-Fi，跳过主机地址更新"; return }

    $configPath = "$env:APPDATA\Macro Deck\config.json"
    if (-not (Test-Path $configPath)) { Write-Host "config.json 不存在，跳过"; return }

    $cfg = Get-Content $configPath -Raw | ConvertFrom-Json
    $cfg.'Connection.Host.Address' = $wifiIP
    $cfg | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8
    Write-Host "主机地址已更新: $wifiIP"
}

if ($Build) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Error "未找到 .NET SDK，请安装：https://dotnet.microsoft.com/download/dotnet/10.0"
        exit 1
    }

    # 写入 Program Files 等受保护目录需要管理员权限，提前检查避免构建到一半失败
    $isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    if (($Output -like "$env:ProgramFiles*" -or $Output -like "${env:ProgramFiles(x86)}*") -and -not $isAdmin) {
        Write-Error "输出目录位于受保护位置：$Output`n请以管理员身份运行 PowerShell，或用 -Output 指定其他目录。"
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
    $exeFullPath = Join-Path (Resolve-Path $Output).Path "$exeName.exe"
    Write-Host "可执行文件: $exeFullPath"

    Set-FirewallRule
    Update-HostAddress
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
