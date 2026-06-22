<#
.SYNOPSIS
    Trigger the build-local CI workflow, wait for it to complete, then update
    the local Macro Deck installation with the freshly built output.
.DESCRIPTION
    1. Triggers .github/workflows/build-local.yml on the tea4go/Macro-Deck repo.
    2. Polls until the run completes.
    3. Downloads the macro-deck-build artifact.
    4. Stops Macro Deck, copies program files to the install directory, restarts.
    Only non-directory items from the artifact are copied so that user data
    directories (plugins, wwwroot, Android Debug Bridge) are left untouched.
.PARAMETER Repo
    GitHub repository in owner/repo format.
.PARAMETER InstallDir
    Macro Deck installation directory.
.PARAMETER SkipBuild
    Skip triggering a new build; use the most recent successful run's artifact.
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
$MacroDeckProcessName = "Macro Deck 2"

function Write-Step($m) { Write-Host "[INFO] $m" -ForegroundColor Cyan }
function Write-Ok($m)   { Write-Host "[ OK ] $m" -ForegroundColor Green }
function Write-Fail($m) { Write-Host "[FAIL] $m" -ForegroundColor Red }

Write-Host "=== Macro Deck Local Updater ===" -ForegroundColor Cyan

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Fail "gh CLI not found. Install from https://cli.github.com"
    exit 1
}

if (-not $SkipBuild) {
    Write-Step "Triggering build-local workflow on $Repo ..."
    gh workflow run build-local.yml --repo $Repo
    if ($LASTEXITCODE -ne 0) { Write-Fail "Failed to trigger workflow"; exit 1 }
    Write-Ok "Workflow triggered."
    Start-Sleep -Seconds 8  # give GitHub a moment to register the run
}

# Find the latest build-local run and wait for it
Write-Step "Waiting for build to complete (this takes ~5 minutes)..."
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
        Write-Host "  run $RunId : status=$status conclusion=$conclusion"
        if ($status -eq "completed") {
            if ($conclusion -eq "success") { Write-Ok "Build succeeded."; break }
            else { Write-Fail "Build $conclusion. Check https://github.com/$Repo/actions/runs/$RunId"; exit 1 }
        }
    }
    Start-Sleep -Seconds 30
}
if (-not $RunId) { Write-Fail "Could not find a build run"; exit 1 }

# Download artifact
$TempDir = Join-Path $env:TEMP "macro-deck-build-$(Get-Date -Format yyyyMMdd_HHmmss)"
New-Item -ItemType Directory -Force -Path $TempDir | Out-Null
Write-Step "Downloading artifact to $TempDir ..."
gh run download $RunId --repo $Repo --name macro-deck-build --dir $TempDir
if ($LASTEXITCODE -ne 0) { Write-Fail "Failed to download artifact"; exit 1 }
Write-Ok "Download complete."

# The artifact may be in a subdirectory; find the root with the exe
$PublishRoot = Get-ChildItem -Path $TempDir -Filter "Macro Deck 2.exe" -Recurse | Select-Object -First 1
if (-not $PublishRoot) { Write-Fail "Macro Deck 2.exe not found in downloaded artifact"; exit 1 }
$PublishRoot = $PublishRoot.DirectoryName
Write-Host "Publish root: $PublishRoot"

# Stop Macro Deck
Write-Step "Stopping Macro Deck..."
$proc = Get-Process -Name $MacroDeckProcessName -ErrorAction SilentlyContinue
if ($proc) {
    $proc | Stop-Process -Force
    Start-Sleep -Seconds 2
    Write-Ok "Macro Deck stopped."
} else {
    Write-Host "Macro Deck was not running."
}

# Copy only files (not subdirectories) to preserve plugins/wwwroot/Android Debug Bridge
Write-Step "Copying updated program files to $InstallDir ..."
$count = 0
Get-ChildItem -Path $PublishRoot -File | ForEach-Object {
    $dest = Join-Path $InstallDir $_.Name
    Copy-Item -Path $_.FullName -Destination $dest -Force
    $count++
}
Write-Ok "Copied $count files."

# Verify the updated DLL is newer
$dll = Get-Item (Join-Path $InstallDir "Macro Deck 2.dll") -ErrorAction SilentlyContinue
if ($dll) { Write-Host "Installed DLL last modified: $($dll.LastWriteTime)" }

# Clean up temp
Remove-Item -Path $TempDir -Recurse -Force -ErrorAction SilentlyContinue

# Start Macro Deck
Write-Step "Starting Macro Deck..."
Start-Process -FilePath (Join-Path $InstallDir "Macro Deck 2.exe") -WorkingDirectory $InstallDir
Write-Ok "Macro Deck started."
Write-Ok "Done."
