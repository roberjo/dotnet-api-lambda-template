# Deploy Script
# This script deploys the .NET API Lambda Template to AWS

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("dev", "qa", "test", "prod")]
    [string]$Environment,
    
    [string]$Region = "us-east-1",
    [switch]$DryRun,
    [switch]$SkipTests,
    [switch]$SkipBuild,
    [string]$DeploymentMethod = "serverless"
)

Write-Host "🚀 Deploying .NET API Lambda Template to $Environment environment..." -ForegroundColor Green

# Validate prerequisites
Write-Host "📋 Checking prerequisites..." -ForegroundColor Yellow

# Check AWS CLI
try {
    $awsVersion = aws --version
    Write-Host "✅ AWS CLI found: $awsVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ AWS CLI not found. Please install AWS CLI." -ForegroundColor Red
    exit 1
}

# Check AWS credentials
try {
    $awsIdentity = aws sts get-caller-identity
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ AWS credentials not configured. Please run 'aws configure'." -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ AWS credentials configured" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to verify AWS credentials" -ForegroundColor Red
    exit 1
}

# Check deployment tools
switch ($DeploymentMethod) {
    "serverless" {
        try {
            $serverlessVersion = serverless --version
            Write-Host "✅ Serverless Framework found: $serverlessVersion" -ForegroundColor Green
        } catch {
            Write-Host "❌ Serverless Framework not found. Please install: npm install -g serverless" -ForegroundColor Red
            exit 1
        }
    }
    "sam" {
        try {
            $samVersion = sam --version
            Write-Host "✅ AWS SAM CLI found: $samVersion" -ForegroundColor Green
        } catch {
            Write-Host "❌ AWS SAM CLI not found. Please install AWS SAM CLI." -ForegroundColor Red
            exit 1
        }
    }
    "terraform" {
        try {
            $terraformVersion = terraform --version
            Write-Host "✅ Terraform found: $terraformVersion" -ForegroundColor Green
        } catch {
            Write-Host "❌ Terraform not found. Please install Terraform." -ForegroundColor Red
            exit 1
        }
    }
}

# Run tests (if not skipped)
if (-not $SkipTests) {
    Write-Host "🧪 Running tests before deployment..." -ForegroundColor Yellow
    .\scripts\run-tests.ps1 -Configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Tests failed. Deployment aborted." -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ All tests passed" -ForegroundColor Green
}

# Build the solution (if not skipped)
if (-not $SkipBuild) {
    Write-Host "🔨 Building solution..." -ForegroundColor Yellow
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed. Deployment aborted." -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Solution built successfully" -ForegroundColor Green
}

# Deploy based on method
Write-Host "🚀 Deploying using $DeploymentMethod..." -ForegroundColor Yellow

switch ($DeploymentMethod) {
    "serverless" {
        if ($DryRun) {
            Write-Host "🔍 Dry run: serverless deploy --stage $Environment --region $Region --verbose" -ForegroundColor Cyan
        } else {
            serverless deploy --stage $Environment --region $Region --verbose
        }
    }
    "sam" {
        if ($DryRun) {
            Write-Host "🔍 Dry run: sam deploy --template-file template.yaml --stack-name dotnet-api-lambda-$Environment --parameter-overrides Environment=$Environment --capabilities CAPABILITY_IAM" -ForegroundColor Cyan
        } else {
            sam deploy --template-file template.yaml --stack-name dotnet-api-lambda-$Environment --parameter-overrides Environment=$Environment --capabilities CAPABILITY_IAM
        }
    }
    "terraform" {
        if ($DryRun) {
            Write-Host "🔍 Dry run: terraform apply -var environment=$Environment -var aws_region=$Region" -ForegroundColor Cyan
        } else {
            terraform apply -var environment=$Environment -var aws_region=$Region
        }
    }
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Deployment failed!" -ForegroundColor Red
    exit 1
}

if (-not $DryRun) {
    Write-Host "`n✅ Deployment completed successfully!" -ForegroundColor Green
    
    # Display deployment information
    Write-Host "`n📋 Deployment Information:" -ForegroundColor Cyan
    Write-Host "- Environment: $Environment" -ForegroundColor White
    Write-Host "- Region: $Region" -ForegroundColor White
    Write-Host "- Method: $DeploymentMethod" -ForegroundColor White
    Write-Host "- Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
    
    # Display next steps
    Write-Host "`n🔗 Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Check CloudWatch logs for any issues" -ForegroundColor White
    Write-Host "2. Test the API endpoints" -ForegroundColor White
    Write-Host "3. Monitor performance metrics" -ForegroundColor White
    Write-Host "4. Set up monitoring and alerting" -ForegroundColor White
} else {
    Write-Host "`n🔍 Dry run completed. No changes were made." -ForegroundColor Yellow
}

Write-Host "`n📚 Documentation:" -ForegroundColor Cyan
Write-Host "- Deployment Guide: docs/Deployment-Guide.md" -ForegroundColor White
Write-Host "- Technical Spec: docs/Technical-Specification.md" -ForegroundColor White

Write-Host "`n✨ Deployment script completed!" -ForegroundColor Green
