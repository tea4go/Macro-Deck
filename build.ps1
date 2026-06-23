param(
    [switch]$Build,
    [string]$Configuration = "Release",
    [string]$Output = "publish"
)

if (-not $Build) {
    Write-Host "Usage: .\build.ps1 -Build [-Configuration <Release|Debug>] [-Output <dir>]"
    Write-Host ""
    Write-Host "  -Build           Execute the build and publish"
    Write-Host "  -Configuration   Build configuration (default: Release)"
    Write-Host "  -Output          Output directory (default: publish)"
    exit 0
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET SDK not found. Install from: https://dotnet.microsoft.com/download/dotnet/10.0"
    exit 1
}

$proj = "src\MacroDeck\MacroDeck.csproj"

Write-Host "Restoring..."
dotnet restore $proj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Publishing ($Configuration)..."
dotnet publish $proj -c $Configuration -r win-x64 --self-contained true -o $Output
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Done -> $Output"
