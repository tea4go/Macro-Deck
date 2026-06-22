<#
.SYNOPSIS
    End-to-end smoke test for the Macro Deck main application.
.DESCRIPTION
    Optionally restarts Macro Deck, then verifies the running application:
      - the HTTP/WebSocket port accepts connections
      - the /ping REST endpoint responds
      - the /client/ web client is served
      - a paired WebSocket client can complete the CONNECTED + GET_BUTTONS handshake
      - the log shows a clean startup (plugins loaded, startup finished, no server error)
.PARAMETER SkipRestart
    Do not restart Macro Deck; test the already-running instance.
.PARAMETER ClientId
    Paired client id for the WebSocket handshake (must exist in devices.json, Blocked=false).
.PARAMETER ServerHost
    Macro Deck host (default localhost).
.PARAMETER Port
    Macro Deck HTTP/WebSocket port (default 8191).
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\test-macrodeck.ps1
    powershell -ExecutionPolicy Bypass -File .\test-macrodeck.ps1 -SkipRestart
#>

param(
    [switch]$SkipRestart,
    [string]$ClientId = "lvgoejr",
    [string]$ServerHost = "localhost",
    [int]$Port = 8191
)

$ErrorActionPreference = "Stop"

$MacroDeckProcessName = "Macro Deck 2"
$MacroDeckExe = "C:\Program Files\Macro Deck\Macro Deck 2.exe"
$LogDir = Join-Path $env:APPDATA "Macro Deck\logs"

$script:Results = [ordered]@{}
function Set-Result($Name, $Passed, $Detail) {
    $script:Results[$Name] = [pscustomobject]@{ Passed = [bool]$Passed; Detail = $Detail }
}
function Write-Step($m) { Write-Host "[INFO] $m" -ForegroundColor Cyan }
function Write-Ok($m)   { Write-Host "[ OK ] $m" -ForegroundColor Green }
function Write-Warn($m) { Write-Host "[WARN] $m" -ForegroundColor Yellow }
function Write-Fail($m) { Write-Host "[FAIL] $m" -ForegroundColor Red }

Write-Host "=== Macro Deck Main App - E2E Smoke Test ===" -ForegroundColor Cyan
Write-Host "Host=$ServerHost Port=$Port Client=$ClientId"

# ---- log baseline (before restart) ----
$today = Get-Date -Format "yyyyMMdd"
$logPath = Join-Path $LogDir "log$today.txt"
$logBaseline = 0
if (Test-Path $logPath) { $logBaseline = (Get-Item $logPath).Length }
Write-Host "Log file: $logPath (baseline $logBaseline bytes)"

# ---- restart ----
if (-not $SkipRestart) {
    Write-Step "Restarting Macro Deck..."
    Get-Process -Name $MacroDeckProcessName -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Seconds 2
    if (-not (Test-Path $MacroDeckExe)) {
        Write-Fail "Macro Deck executable not found: $MacroDeckExe"
        exit 1
    }
    # Launch with the working directory set to the install folder, otherwise Macro Deck
    # uses the caller's CWD as its content root and cannot find wwwroot (web client).
    Start-Process -FilePath $MacroDeckExe -WorkingDirectory (Split-Path -Parent $MacroDeckExe)
} else {
    Write-Step "Skip restart; testing the running instance."
}

# ---- 1. wait for port ----
Write-Step "Waiting for port $Port..."
$portReady = $false
$deadline = (Get-Date).AddSeconds(40)
while ((Get-Date) -lt $deadline) {
    try {
        $tcp = New-Object System.Net.Sockets.TcpClient
        $iar = $tcp.BeginConnect($ServerHost, $Port, $null, $null)
        if ($iar.AsyncWaitHandle.WaitOne(1000) -and $tcp.Connected) { $tcp.EndConnect($iar); $tcp.Close(); $portReady = $true; break }
        $tcp.Close()
    } catch { }
    Start-Sleep -Milliseconds 800
}
if ($portReady) { Write-Ok "Port $Port ready." } else { Write-Fail "Port $Port not ready in time." }
Set-Result "PortReady" $portReady "tcp ${ServerHost}:$Port"
if (-not $portReady) {
    Write-Host "`n=== RESULT: FAIL (server not reachable) ===" -ForegroundColor Red
    exit 1
}
Start-Sleep -Seconds 2

# ---- 2. /ping ----
Write-Step "GET /ping ..."
$pingOk = $false
try {
    $r = Invoke-WebRequest -Uri "http://${ServerHost}:$Port/ping" -UseBasicParsing -TimeoutSec 10
    $pingOk = ($r.StatusCode -eq 200)
} catch { Write-Warn $_.Exception.Message }
if ($pingOk) { Write-Ok "/ping -> 200" } else { Write-Fail "/ping failed" }
Set-Result "PingEndpoint" $pingOk "GET /ping"

# ---- 3. /client/ ----
Write-Step "GET /client/ ..."
$clientOk = $false
try {
    $r = Invoke-WebRequest -Uri "http://${ServerHost}:$Port/client/" -UseBasicParsing -TimeoutSec 10
    $clientOk = ($r.StatusCode -eq 200)
} catch { Write-Warn $_.Exception.Message }
if ($clientOk) { Write-Ok "/client/ -> 200" } else { Write-Fail "/client/ failed" }
Set-Result "WebClient" $clientOk "GET /client/"

# ---- 4. WebSocket handshake ----
Write-Step "WebSocket CONNECTED + GET_BUTTONS handshake..."
$wsOk = $false
$wsErr = $null
try {
    $ws = New-Object System.Net.WebSockets.ClientWebSocket
    $cts = New-Object System.Threading.CancellationTokenSource
    $cts.CancelAfter(12000)
    $ws.ConnectAsync([Uri]"ws://${ServerHost}:$Port", $cts.Token).Wait()

    function Send-Json($o) {
        $b = [System.Text.Encoding]::UTF8.GetBytes(($o | ConvertTo-Json -Compress))
        $s = New-Object System.ArraySegment[byte] (,$b)
        $ws.SendAsync($s, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, $cts.Token).Wait()
    }
    function Receive-One($ms) {
        $buf = New-Object byte[] 65536
        $rc = New-Object System.Threading.CancellationTokenSource
        $rc.CancelAfter($ms)
        $sb = New-Object System.Text.StringBuilder
        do {
            $s = New-Object System.ArraySegment[byte] (,$buf)
            $t = $ws.ReceiveAsync($s, $rc.Token); $t.Wait()
            [void]$sb.Append([System.Text.Encoding]::UTF8.GetString($buf, 0, $t.Result.Count))
        } while (-not $t.Result.EndOfMessage)
        return $sb.ToString()
    }

    Send-Json ([ordered]@{ Method = "CONNECTED"; "Client-Id" = $ClientId; API = "20"; "Device-Type" = "Web" })
    Send-Json ([ordered]@{ Method = "GET_BUTTONS" })
    $resp1 = Receive-One 5000
    $resp2 = Receive-One 7000
    if (($resp1 -match '"Method"') -and ($resp2 -match '"Method"')) { $wsOk = $true }

    try {
        $cc = New-Object System.Threading.CancellationTokenSource; $cc.CancelAfter(2000)
        $ws.CloseAsync([System.Net.WebSockets.WebSocketCloseStatus]::NormalClosure, "done", $cc.Token).Wait()
    } catch { }
    $ws.Dispose()
} catch { $wsErr = $_.Exception.Message }
if ($wsOk) { Write-Ok "WebSocket handshake responded." } else { Write-Fail "WebSocket handshake failed: $wsErr" }
Set-Result "WebSocketHandshake" $wsOk $(if ($wsOk) { "CONNECTED+GET_BUTTONS answered" } else { $wsErr })

# ---- 5. log check ----
Write-Step "Checking startup log..."
$pluginsLoaded = $false
$startupFinished = $false
$serverError = $false
$newLog = ""
$verifyDeadline = (Get-Date).AddSeconds(10)
while ((Get-Date) -lt $verifyDeadline) {
    if (Test-Path $logPath) {
        try {
            $fs = [System.IO.File]::Open($logPath, 'Open', 'Read', 'ReadWrite')
            try {
                if ($fs.Length -gt $logBaseline) {
                    [void]$fs.Seek($logBaseline, 'Begin')
                    $sr = New-Object System.IO.StreamReader($fs)
                    $newLog = $sr.ReadToEnd(); $sr.Dispose()
                }
            } finally { $fs.Dispose() }
        } catch { }
    }
    if ($newLog -match "Loading plugins") { $pluginsLoaded = $true }
    if ($newLog -match "startup finished") { $startupFinished = $true }
    if ($newLog -match "Failed to start server") { $serverError = $true }
    if ($pluginsLoaded -and $startupFinished) { break }
    Start-Sleep -Milliseconds 700
}
$logOk = $pluginsLoaded -and $startupFinished -and (-not $serverError)
if ($logOk) { Write-Ok "Startup log clean (plugins loaded, startup finished)." }
else { Write-Fail "Startup log check failed (plugins=$pluginsLoaded finished=$startupFinished error=$serverError)." }
Set-Result "StartupLog" $logOk "log markers"

# ---- summary ----
Write-Host "`n=== Test Summary ===" -ForegroundColor Cyan
$allPass = $true
foreach ($key in $script:Results.Keys) {
    $r = $script:Results[$key]
    $tag = if ($r.Passed) { "PASS" } else { "FAIL" }
    $color = if ($r.Passed) { "Green" } else { "Red" }
    if (-not $r.Passed) { $allPass = $false }
    Write-Host ("  {0,-20} {1}  {2}" -f $key, $tag, $r.Detail) -ForegroundColor $color
}
Write-Host ""
if ($allPass) { Write-Host "=== RESULT: PASS ===" -ForegroundColor Green; exit 0 }
else { Write-Host "=== RESULT: FAIL ===" -ForegroundColor Red; exit 1 }
