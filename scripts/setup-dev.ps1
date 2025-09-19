# Setup Development Environment Script
# This script sets up the development environment for the .NET API Lambda Template

param(
    [switch]$SkipDocker,
    [switch]$SkipDatabase,
    [switch]$SkipTests
)

Write-Host "🚀 Setting up development environment for .NET API Lambda Template..." -ForegroundColor Green

# Check if .NET 8 SDK is installed
Write-Host "📋 Checking prerequisites..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -notmatch "^8\.") {
        Write-Host "❌ .NET 8 SDK is required. Current version: $dotnetVersion" -ForegroundColor Red
        Write-Host "Please install .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "✅ .NET 8 SDK found: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK not found. Please install .NET 8 SDK." -ForegroundColor Red
    exit 1
}

# Check if Docker is installed
if (-not $SkipDocker) {
    try {
        $dockerVersion = docker --version
        Write-Host "✅ Docker found: $dockerVersion" -ForegroundColor Green
    } catch {
        Write-Host "❌ Docker not found. Please install Docker Desktop." -ForegroundColor Red
        Write-Host "Skipping Docker setup..." -ForegroundColor Yellow
        $SkipDocker = $true
    }
}

# Restore NuGet packages
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Packages restored successfully" -ForegroundColor Green

# Build the solution
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Solution built successfully" -ForegroundColor Green

# Start Docker services
if (-not $SkipDocker) {
    Write-Host "🐳 Starting Docker services..." -ForegroundColor Yellow
    docker-compose up -d postgresql redis dynamodb-local
    
    # Wait for services to be ready
    Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
    $maxAttempts = 30
    $attempt = 0
    
    do {
        Start-Sleep -Seconds 2
        $attempt++
        
        $postgresReady = docker exec dotnet-api-postgresql pg_isready -U postgres -d dotnet_api_dev 2>$null
        $redisReady = docker exec dotnet-api-redis redis-cli ping 2>$null
        $dynamoReady = curl -s http://localhost:8000/ 2>$null
        
        if ($postgresReady -and $redisReady -and $dynamoReady) {
            Write-Host "✅ All services are ready" -ForegroundColor Green
            break
        }
        
        if ($attempt -ge $maxAttempts) {
            Write-Host "❌ Services failed to start within expected time" -ForegroundColor Red
            Write-Host "Please check Docker logs: docker-compose logs" -ForegroundColor Yellow
            exit 1
        }
        
        Write-Host "⏳ Waiting for services... (attempt $attempt/$maxAttempts)" -ForegroundColor Yellow
    } while ($true)
}

# Run database migrations (if not skipped)
if (-not $SkipDatabase -and -not $SkipDocker) {
    Write-Host "🗄️ Running database migrations..." -ForegroundColor Yellow
    # This will be implemented when we add Entity Framework
    Write-Host "⚠️ Database migrations will be implemented in Phase 4" -ForegroundColor Yellow
}

# Run tests (if not skipped)
if (-not $SkipTests) {
    Write-Host "🧪 Running tests..." -ForegroundColor Yellow
    dotnet test --configuration Release --verbosity normal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Some tests failed" -ForegroundColor Red
        Write-Host "Continuing with setup..." -ForegroundColor Yellow
    } else {
        Write-Host "✅ All tests passed" -ForegroundColor Green
    }
}

# Create development certificates
Write-Host "🔐 Setting up development certificates..." -ForegroundColor Yellow
dotnet dev-certs https --trust
Write-Host "✅ Development certificates configured" -ForegroundColor Green

# Display next steps
Write-Host "`n🎉 Development environment setup complete!" -ForegroundColor Green
Write-Host "`n📋 Next steps:" -ForegroundColor Cyan
Write-Host "1. Start the API: dotnet run --project src/API/DotNetApiLambdaTemplate.API" -ForegroundColor White
Write-Host "2. View API documentation: https://localhost:5001/swagger" -ForegroundColor White
Write-Host "3. Run tests: dotnet test" -ForegroundColor White
Write-Host "4. Start Docker services: docker-compose up -d" -ForegroundColor White

if (-not $SkipDocker) {
    Write-Host "`n🐳 Docker services status:" -ForegroundColor Cyan
    docker-compose ps
}

Write-Host "`n📚 Documentation:" -ForegroundColor Cyan
Write-Host "- PRD: docs/PRD.md" -ForegroundColor White
Write-Host "- Technical Spec: docs/Technical-Specification.md" -ForegroundColor White
Write-Host "- Development Plan: docs/Development-Plan.md" -ForegroundColor White
Write-Host "- Deployment Guide: docs/Deployment-Guide.md" -ForegroundColor White

Write-Host "`n✨ Happy coding!" -ForegroundColor Green
