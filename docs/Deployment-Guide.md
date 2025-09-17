# Deployment Guide

## Overview

This guide provides comprehensive instructions for deploying the ASP.NET Core 8 Web API Lambda template to AWS using multiple deployment strategies.

## Prerequisites

### Required Tools

- **.NET 8 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **AWS CLI**: [Installation Guide](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- **Terraform**: [Download](https://www.terraform.io/downloads.html)
- **Docker**: [Download](https://www.docker.com/products/docker-desktop/)
- **Serverless Framework**: `npm install -g serverless`
- **AWS SAM CLI**: [Installation Guide](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html)

### AWS Account Setup

1. **Create AWS Account**: Sign up for AWS if you don't have an account
2. **Configure AWS CLI**: Run `aws configure` and enter your credentials
3. **Create IAM User**: Create a user with appropriate permissions for deployment
4. **Set up Terraform Cloud**: Create account and workspace

### Required AWS Services

- Lambda
- API Gateway
- RDS (PostgreSQL)
- DynamoDB
- S3
- SQS
- SNS
- EventBridge
- CloudWatch
- X-Ray
- Secrets Manager

## Environment Configuration

### Environment Variables

Create environment-specific configuration files:

#### Development (appsettings.Development.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=dotnet_api_dev;Username=postgres;Password=password",
    "DynamoDB": "http://localhost:8000"
  },
  "AWS": {
    "Region": "us-east-1",
    "S3Bucket": "dotnet-api-dev-files",
    "SQSQueue": "dotnet-api-dev-orders",
    "SNSTopic": "dotnet-api-dev-notifications"
  },
  "Okta": {
    "Authority": "https://dev-123456.okta.com/oauth2/default",
    "Audience": "api://dotnet-api-dev"
  }
}
```

#### Production (appsettings.Production.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "PostgreSQL": "${DB_CONNECTION_STRING}",
    "DynamoDB": "${DYNAMODB_ENDPOINT}"
  },
  "AWS": {
    "Region": "${AWS_REGION}",
    "S3Bucket": "${S3_BUCKET_NAME}",
    "SQSQueue": "${SQS_QUEUE_URL}",
    "SNSTopic": "${SNS_TOPIC_ARN}"
  },
  "Okta": {
    "Authority": "${OKTA_AUTHORITY}",
    "Audience": "${OKTA_AUDIENCE}"
  }
}
```

## Deployment Strategies

### Strategy 1: Terraform Cloud + GitHub Actions

#### Step 1: Set up Terraform Cloud

1. **Create Terraform Cloud Account**: Sign up at [app.terraform.io](https://app.terraform.io)
2. **Create Workspace**: Create a new workspace for each environment
3. **Configure Variables**: Set required variables in Terraform Cloud

#### Step 2: Terraform Configuration

Create `infrastructure/terraform/main.tf`:

```hcl
terraform {
  required_version = ">= 1.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
  cloud {
    organization = "your-organization"
    workspaces {
      name = "dotnet-api-lambda-${var.environment}"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

# VPC Configuration
resource "aws_vpc" "main" {
  cidr_block           = var.vpc_cidr
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name        = "dotnet-api-lambda-${var.environment}"
    Environment = var.environment
  }
}

# Lambda Function
resource "aws_lambda_function" "api" {
  filename         = "dotnet-api-lambda.zip"
  function_name    = "dotnet-api-lambda-${var.environment}"
  role            = aws_iam_role.lambda_role.arn
  handler         = "DotNetApiLambdaTemplate::DotNetApiLambdaTemplate.LambdaEntryPoint::FunctionHandlerAsync"
  runtime         = "dotnet8"
  timeout         = 30
  memory_size     = 512

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = var.environment
      DB_CONNECTION_STRING   = var.db_connection_string
      AWS_REGION            = var.aws_region
    }
  }

  depends_on = [aws_cloudwatch_log_group.lambda_logs]
}

# API Gateway
resource "aws_api_gateway_rest_api" "api" {
  name        = "dotnet-api-lambda-${var.environment}"
  description = "API Gateway for .NET Lambda API"

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

# RDS PostgreSQL
resource "aws_db_instance" "postgresql" {
  identifier = "dotnet-api-postgresql-${var.environment}"
  engine     = "postgres"
  engine_version = "15.4"
  instance_class = "db.t3.micro"
  allocated_storage = 20
  storage_type = "gp2"

  db_name  = "dotnet_api_${var.environment}"
  username = var.db_username
  password = var.db_password

  vpc_security_group_ids = [aws_security_group.rds.id]
  db_subnet_group_name   = aws_db_subnet_group.main.name

  backup_retention_period = 7
  backup_window          = "03:00-04:00"
  maintenance_window     = "sun:04:00-sun:05:00"

  skip_final_snapshot = var.environment != "prod"

  tags = {
    Name        = "dotnet-api-postgresql-${var.environment}"
    Environment = var.environment
  }
}

# DynamoDB
resource "aws_dynamodb_table" "user_sessions" {
  name           = "user-sessions-${var.environment}"
  billing_mode   = "PAY_PER_REQUEST"
  hash_key       = "UserId"
  range_key      = "SessionId"

  attribute {
    name = "UserId"
    type = "S"
  }

  attribute {
    name = "SessionId"
    type = "S"
  }

  tags = {
    Name        = "user-sessions-${var.environment}"
    Environment = var.environment
  }
}

# S3 Bucket
resource "aws_s3_bucket" "files" {
  bucket = "dotnet-api-files-${var.environment}-${random_id.bucket_suffix.hex}"

  tags = {
    Name        = "dotnet-api-files-${var.environment}"
    Environment = var.environment
  }
}

# SQS Queue
resource "aws_sqs_queue" "order_processing" {
  name = "order-processing-${var.environment}"

  tags = {
    Name        = "order-processing-${var.environment}"
    Environment = var.environment
  }
}

# SNS Topic
resource "aws_sns_topic" "notifications" {
  name = "notifications-${var.environment}"

  tags = {
    Name        = "notifications-${var.environment}"
    Environment = var.environment
  }
}
```

#### Step 3: GitHub Actions Workflow

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to AWS

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  AWS_REGION: us-east-1
  TERRAFORM_VERSION: 1.6.0

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
      - name: Security Scan
        run: |
          dotnet tool install --global security-scan
          dotnet security-scan

  deploy-dev:
    needs: test
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    environment: development
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Build and Package
        run: |
          dotnet publish -c Release -o ./publish
          cd ./publish
          zip -r ../dotnet-api-lambda.zip .
      
      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v4
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}
      
      - name: Deploy to AWS
        run: |
          aws lambda update-function-code \
            --function-name dotnet-api-lambda-dev \
            --zip-file fileb://dotnet-api-lambda.zip

  deploy-prod:
    needs: test
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment: production
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Build and Package
        run: |
          dotnet publish -c Release -o ./publish
          cd ./publish
          zip -r ../dotnet-api-lambda.zip .
      
      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v4
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}
      
      - name: Deploy to AWS
        run: |
          aws lambda update-function-code \
            --function-name dotnet-api-lambda-prod \
            --zip-file fileb://dotnet-api-lambda.zip
```

### Strategy 2: Serverless Framework

#### Step 1: Serverless Configuration

Create `serverless.yml`:

```yaml
service: dotnet-api-lambda-template

frameworkVersion: '3'

provider:
  name: aws
  runtime: dotnet8
  region: ${opt:region, 'us-east-1'}
  stage: ${opt:stage, 'dev'}
  memorySize: 512
  timeout: 30
  environment:
    ASPNETCORE_ENVIRONMENT: ${self:provider.stage}
    DB_CONNECTION_STRING: ${env:DB_CONNECTION_STRING}
    AWS_REGION: ${self:provider.region}
  iam:
    role:
      statements:
        - Effect: Allow
          Action:
            - logs:CreateLogGroup
            - logs:CreateLogStream
            - logs:PutLogEvents
          Resource: "arn:aws:logs:*:*:*"
        - Effect: Allow
          Action:
            - rds:*
          Resource: "*"
        - Effect: Allow
          Action:
            - dynamodb:*
          Resource: "*"
        - Effect: Allow
          Action:
            - s3:*
          Resource: "*"
        - Effect: Allow
          Action:
            - sqs:*
          Resource: "*"
        - Effect: Allow
          Action:
            - sns:*
          Resource: "*"

functions:
  api:
    handler: DotNetApiLambdaTemplate::DotNetApiLambdaTemplate.LambdaEntryPoint::FunctionHandlerAsync
    events:
      - http:
          path: /{proxy+}
          method: ANY
          cors: true
    environment:
      DB_CONNECTION_STRING: ${env:DB_CONNECTION_STRING}

resources:
  Resources:
    # RDS PostgreSQL
    PostgresqlDB:
      Type: AWS::RDS::DBInstance
      Properties:
        DBInstanceIdentifier: dotnet-api-postgresql-${self:provider.stage}
        DBName: dotnet_api_${self:provider.stage}
        Engine: postgres
        EngineVersion: '15.4'
        DBInstanceClass: db.t3.micro
        AllocatedStorage: 20
        MasterUsername: ${env:DB_USERNAME}
        MasterUserPassword: ${env:DB_PASSWORD}
        VPCSecurityGroups:
          - Ref: PostgresqlSecurityGroup
        DBSubnetGroupName:
          Ref: PostgresqlSubnetGroup
        BackupRetentionPeriod: 7
        MultiAZ: false
        StorageEncrypted: true

    # DynamoDB
    UserSessionsTable:
      Type: AWS::DynamoDB::Table
      Properties:
        TableName: user-sessions-${self:provider.stage}
        BillingMode: PAY_PER_REQUEST
        AttributeDefinitions:
          - AttributeName: UserId
            AttributeType: S
          - AttributeName: SessionId
            AttributeType: S
        KeySchema:
          - AttributeName: UserId
            KeyType: HASH
          - AttributeName: SessionId
            KeyType: RANGE

    # S3 Bucket
    FilesBucket:
      Type: AWS::S3::Bucket
      Properties:
        BucketName: dotnet-api-files-${self:provider.stage}-${env:BUCKET_SUFFIX}
        VersioningConfiguration:
          Status: Enabled
        PublicAccessBlockConfiguration:
          BlockPublicAcls: true
          BlockPublicPolicy: true
          IgnorePublicAcls: true
          RestrictPublicBuckets: true

    # SQS Queue
    OrderProcessingQueue:
      Type: AWS::SQS::Queue
      Properties:
        QueueName: order-processing-${self:provider.stage}
        VisibilityTimeoutSeconds: 300
        MessageRetentionPeriod: 1209600

    # SNS Topic
    NotificationsTopic:
      Type: AWS::SNS::Topic
      Properties:
        TopicName: notifications-${self:provider.stage}

  Outputs:
    ApiGatewayRestApiId:
      Value:
        Ref: ApiGatewayRestApi
      Export:
        Name: ${self:service}-${self:provider.stage}-ApiGatewayRestApiId

    ApiGatewayRestApiRootResourceId:
      Value:
        Fn::GetAtt:
          - ApiGatewayRestApi
          - RootResourceId
      Export:
        Name: ${self:service}-${self:provider.stage}-ApiGatewayRestApiRootResourceId

plugins:
  - serverless-dotnet
  - serverless-offline
```

#### Step 2: Deploy with Serverless

```powershell
# Install dependencies
npm install

# Deploy to development
serverless deploy --stage dev

# Deploy to production
serverless deploy --stage prod

# Deploy specific function
serverless deploy function --function api --stage dev
```

### Strategy 3: AWS SAM

#### Step 1: SAM Template

Create `template.yaml`:

```yaml
AWSTemplateFormatVersion: '2010-09-09'
Transform: AWS::Serverless-2016-10-31
Description: ASP.NET Core 8 Web API Lambda Template

Globals:
  Function:
    Timeout: 30
    MemorySize: 512
    Runtime: dotnet8
    Environment:
      Variables:
        ASPNETCORE_ENVIRONMENT: !Ref Environment

Parameters:
  Environment:
    Type: String
    Default: dev
    AllowedValues:
      - dev
      - qa
      - test
      - prod
    Description: Environment name

  DBUsername:
    Type: String
    Default: postgres
    Description: Database username

  DBPassword:
    Type: String
    NoEcho: true
    Description: Database password

Resources:
  # Lambda Function
  DotNetApiFunction:
    Type: AWS::Serverless::Function
    Properties:
      CodeUri: ./publish/
      Handler: DotNetApiLambdaTemplate::DotNetApiLambdaTemplate.LambdaEntryPoint::FunctionHandlerAsync
      Events:
        ApiGateway:
          Type: Api
          Properties:
            Path: /{proxy+}
            Method: ANY
            RestApiId: !Ref DotNetApiGateway
      Environment:
        Variables:
          DB_CONNECTION_STRING: !Sub "Host=${PostgresqlDB.Endpoint.Address};Database=dotnet_api_${Environment};Username=${DBUsername};Password=${DBPassword}"
          AWS_REGION: !Ref AWS::Region

  # API Gateway
  DotNetApiGateway:
    Type: AWS::Serverless::Api
    Properties:
      StageName: !Ref Environment
      Cors:
        AllowMethods: "'GET,POST,PUT,DELETE,OPTIONS'"
        AllowHeaders: "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'"
        AllowOrigin: "'*'"

  # RDS PostgreSQL
  PostgresqlDB:
    Type: AWS::RDS::DBInstance
    Properties:
      DBInstanceIdentifier: !Sub "dotnet-api-postgresql-${Environment}"
      DBName: !Sub "dotnet_api_${Environment}"
      Engine: postgres
      EngineVersion: '15.4'
      DBInstanceClass: db.t3.micro
      AllocatedStorage: 20
      MasterUsername: !Ref DBUsername
      MasterUserPassword: !Ref DBPassword
      VPCSecurityGroups:
        - !Ref PostgresqlSecurityGroup
      DBSubnetGroupName: !Ref PostgresqlSubnetGroup
      BackupRetentionPeriod: 7
      MultiAZ: false
      StorageEncrypted: true
      DeletionProtection: !If [IsProduction, true, false]

  # DynamoDB
  UserSessionsTable:
    Type: AWS::DynamoDB::Table
    Properties:
      TableName: !Sub "user-sessions-${Environment}"
      BillingMode: PAY_PER_REQUEST
      AttributeDefinitions:
        - AttributeName: UserId
          AttributeType: S
        - AttributeName: SessionId
          AttributeType: S
      KeySchema:
        - AttributeName: UserId
          KeyType: HASH
        - AttributeName: SessionId
          KeyType: RANGE

  # S3 Bucket
  FilesBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub "dotnet-api-files-${Environment}-${AWS::AccountId}"
      VersioningConfiguration:
        Status: Enabled
      PublicAccessBlockConfiguration:
        BlockPublicAcls: true
        BlockPublicPolicy: true
        IgnorePublicAcls: true
        RestrictPublicBuckets: true

  # SQS Queue
  OrderProcessingQueue:
    Type: AWS::SQS::Queue
    Properties:
      QueueName: !Sub "order-processing-${Environment}"
      VisibilityTimeoutSeconds: 300
      MessageRetentionPeriod: 1209600

  # SNS Topic
  NotificationsTopic:
    Type: AWS::SNS::Topic
    Properties:
      TopicName: !Sub "notifications-${Environment}"

Conditions:
  IsProduction: !Equals [!Ref Environment, prod]

Outputs:
  ApiGatewayUrl:
    Description: API Gateway endpoint URL
    Value: !Sub "https://${DotNetApiGateway}.execute-api.${AWS::Region}.amazonaws.com/${Environment}"
    Export:
      Name: !Sub "${AWS::StackName}-ApiGatewayUrl"

  LambdaFunctionArn:
    Description: Lambda Function ARN
    Value: !GetAtt DotNetApiFunction.Arn
    Export:
      Name: !Sub "${AWS::StackName}-LambdaFunctionArn"
```

#### Step 2: Deploy with SAM

```powershell
# Build the application
dotnet publish -c Release -o ./publish

# Deploy to development
sam deploy --template-file template.yaml --stack-name dotnet-api-lambda-dev --parameter-overrides Environment=dev --capabilities CAPABILITY_IAM

# Deploy to production
sam deploy --template-file template.yaml --stack-name dotnet-api-lambda-prod --parameter-overrides Environment=prod --capabilities CAPABILITY_IAM
```

## Local Development Setup

### Docker Compose

Create `docker-compose.yml`:

```yaml
version: '3.8'

services:
  postgresql:
    image: postgres:15
    environment:
      POSTGRES_DB: dotnet_api_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  dynamodb-local:
    image: amazon/dynamodb-local:latest
    ports:
      - "8000:8000"
    command: ["-jar", "DynamoDBLocal.jar", "-sharedDb", "-inMemory"]

  api:
    build: .
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__PostgreSQL=Host=postgresql;Database=dotnet_api_dev;Username=postgres;Password=password
      - ConnectionStrings__DynamoDB=http://dynamodb-local:8000
    depends_on:
      - postgresql
      - redis
      - dynamodb-local

volumes:
  postgres_data:
```

### Development Scripts

Create `scripts/setup-dev.ps1`:

```powershell
# Setup development environment
Write-Host "Setting up development environment..." -ForegroundColor Green

# Start Docker services
docker-compose up -d

# Wait for services to be ready
Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Run database migrations
Write-Host "Running database migrations..." -ForegroundColor Yellow
dotnet ef database update --project src/Infrastructure --startup-project src/API

# Seed development data
Write-Host "Seeding development data..." -ForegroundColor Yellow
dotnet run --project src/API --environment Development --seed-data

Write-Host "Development environment setup complete!" -ForegroundColor Green
Write-Host "API available at: https://localhost:5001" -ForegroundColor Cyan
Write-Host "Swagger UI available at: https://localhost:5001/swagger" -ForegroundColor Cyan
```

## Monitoring and Maintenance

### Health Checks

The API includes comprehensive health checks:

- **Basic Health**: `GET /health` - Basic application health
- **Readiness**: `GET /ready` - Application ready to accept requests
- **Liveness**: `GET /live` - Application is alive and running

### Monitoring Setup

1. **CloudWatch Dashboards**: Pre-configured dashboards for monitoring
2. **Alarms**: Automated alerts for errors and performance issues
3. **X-Ray Tracing**: Request tracing and performance analysis
4. **Log Analysis**: Structured logging with correlation IDs

### Maintenance Tasks

1. **Database Maintenance**: Automated backups and maintenance windows
2. **Security Updates**: Automated dependency updates with Dependabot
3. **Performance Monitoring**: Regular performance analysis and optimization
4. **Cost Monitoring**: AWS Cost Explorer and budget alerts

## Troubleshooting

### Common Issues

1. **Cold Start Performance**: Monitor cold start times and optimize if needed
2. **Database Connections**: Check connection pooling configuration
3. **Memory Usage**: Monitor Lambda memory usage and adjust if needed
4. **Timeout Issues**: Check Lambda timeout configuration

### Debugging

1. **CloudWatch Logs**: Check application logs for errors
2. **X-Ray Traces**: Analyze request flow and performance
3. **Health Checks**: Verify all health check endpoints
4. **Local Testing**: Use Docker Compose for local debugging

## Security Considerations

1. **Secrets Management**: Use AWS Secrets Manager for sensitive data
2. **Network Security**: Configure VPC and security groups properly
3. **Access Control**: Implement proper IAM roles and policies
4. **Encryption**: Enable encryption at rest and in transit
5. **Monitoring**: Set up security monitoring and alerting

## Cost Optimization

1. **Lambda Configuration**: Optimize memory and timeout settings
2. **Database Sizing**: Right-size RDS instances
3. **Storage Optimization**: Use appropriate S3 storage classes
4. **Monitoring**: Set up cost alerts and budgets
5. **Reserved Capacity**: Consider reserved instances for production
