# Run Tests Script
# This script runs all tests for the .NET API Lambda Template

param(
    [string]$Filter = "",
    [string]$Logger = "console",
    [switch]$Coverage,
    [switch]$Verbose,
    [switch]$Watch,
    [string]$Configuration = "Release"
)

Write-Host "🧪 Running tests for .NET API Lambda Template..." -ForegroundColor Green

# Build the solution first
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed. Cannot run tests." -ForegroundColor Red
    exit 1
}

# Prepare test command
$testCommand = "dotnet test --configuration $Configuration --no-build --logger `"$Logger`""

if ($Coverage) {
    $testCommand += " --collect:`"XPlat Code Coverage`""
}

if ($Verbose) {
    $testCommand += " --verbosity detailed"
}

if ($Watch) {
    $testCommand += " --watch"
}

if ($Filter) {
    $testCommand += " --filter `"$Filter`""
}

Write-Host "🚀 Executing: $testCommand" -ForegroundColor Cyan

# Run tests
Invoke-Expression $testCommand
$testResult = $LASTEXITCODE

# Display results
if ($testResult -eq 0) {
    Write-Host "`n✅ All tests passed!" -ForegroundColor Green
} else {
    Write-Host "`n❌ Some tests failed!" -ForegroundColor Red
}

# Display coverage information if requested
if ($Coverage) {
    Write-Host "`n📊 Coverage reports generated in TestResults folder" -ForegroundColor Cyan
    Write-Host "To view coverage report, open TestResults/*/coverage.cobertura.xml in your IDE" -ForegroundColor Yellow
}

# Display test summary
Write-Host "`n📋 Test Summary:" -ForegroundColor Cyan
Write-Host "- Configuration: $Configuration" -ForegroundColor White
Write-Host "- Logger: $Logger" -ForegroundColor White
if ($Filter) {
    Write-Host "- Filter: $Filter" -ForegroundColor White
}
if ($Coverage) {
    Write-Host "- Coverage: Enabled" -ForegroundColor White
}
if ($Watch) {
    Write-Host "- Watch Mode: Enabled" -ForegroundColor White
}

Write-Host "`n🔧 Available test commands:" -ForegroundColor Cyan
Write-Host "- Run all tests: .\scripts\run-tests.ps1" -ForegroundColor White
Write-Host "- Run with coverage: .\scripts\run-tests.ps1 -Coverage" -ForegroundColor White
Write-Host "- Run specific tests: .\scripts\run-tests.ps1 -Filter 'Category=Unit'" -ForegroundColor White
Write-Host "- Run in watch mode: .\scripts\run-tests.ps1 -Watch" -ForegroundColor White
Write-Host "- Run verbose: .\scripts\run-tests.ps1 -Verbose" -ForegroundColor White

exit $testResult
