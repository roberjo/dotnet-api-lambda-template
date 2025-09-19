# .NET API Lambda Template

A comprehensive ASP.NET Core 8 Web API template designed for AWS Lambda deployment, incorporating modern best practices, Clean Architecture, and comprehensive AWS service integration.

## 🚀 Features

- **Clean Architecture** with Domain, Application, Infrastructure, and API layers
- **CQRS Pattern** with MediatR for command/query separation
- **Multi-Database Support** (PostgreSQL + DynamoDB)
- **AWS Lambda Optimization** with <100ms cold start target
- **Comprehensive Testing** (Unit, Integration, Contract, Security)
- **OAuth 2.0 Authentication** with Okta integration
- **Observability** with Serilog, CloudWatch, and X-Ray
- **Infrastructure as Code** with Terraform, Serverless Framework, and AWS SAM
- **CI/CD Pipeline** with GitHub Actions
- **Docker Support** for local development

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- [Terraform](https://www.terraform.io/downloads.html) (optional)
- [Serverless Framework](https://www.serverless.com/framework/docs/getting-started) (optional)
- [AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html) (optional)

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│  Controllers, Middleware, Request/Response Models          │
├─────────────────────────────────────────────────────────────┤
│                     Application Layer                       │
│  Use Cases, CQRS Commands/Queries, DTOs, MediatR          │
├─────────────────────────────────────────────────────────────┤
│                       Domain Layer                         │
│  Entities, Value Objects, Domain Services, Events          │
├─────────────────────────────────────────────────────────────┤
│                    Infrastructure Layer                     │
│  Data Access, External Services, AWS Integrations          │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Quick Start

### 1. Clone and Setup

```bash
git clone <repository-url>
cd dotnet-api-lambda-template
```

### 2. Run Development Setup

```powershell
# Windows PowerShell
.\scripts\setup-dev.ps1

# Or skip Docker if not available
.\scripts\setup-dev.ps1 -SkipDocker
```

### 3. Start the API

```bash
dotnet run --project src/API/DotNetApiLambdaTemplate.API
```

### 4. Access the API

- **API**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger
- **Health Check**: https://localhost:5001/health

## 🧪 Testing

```powershell
# Run all tests
.\scripts\run-tests.ps1

# Run with coverage
.\scripts\run-tests.ps1 -Coverage

# Run specific tests
.\scripts\run-tests.ps1 -Filter "Category=Unit"

# Run in watch mode
.\scripts\run-tests.ps1 -Watch
```

## 🐳 Docker Development

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

## 🚀 Deployment

### Serverless Framework

```bash
# Deploy to development
serverless deploy --stage dev

# Deploy to production
serverless deploy --stage prod
```

### AWS SAM

```bash
# Deploy to development
sam deploy --template-file template.yaml --stack-name dotnet-api-lambda-dev --parameter-overrides Environment=dev --capabilities CAPABILITY_IAM

# Deploy to production
sam deploy --template-file template.yaml --stack-name dotnet-api-lambda-prod --parameter-overrides Environment=prod --capabilities CAPABILITY_IAM
```

### Terraform

```bash
# Initialize Terraform
terraform init

# Deploy to development
terraform apply -var environment=dev

# Deploy to production
terraform apply -var environment=prod
```

### PowerShell Deploy Script

```powershell
# Deploy to development
.\scripts\deploy.ps1 -Environment dev

# Deploy to production
.\scripts\deploy.ps1 -Environment prod

# Dry run
.\scripts\deploy.ps1 -Environment dev -DryRun
```

## 📚 Documentation

- [Product Requirements Document](docs/PRD.md)
- [Architectural Decision Records](docs/ARDs.md)
- [Technical Specification](docs/Technical-Specification.md)
- [Development Plan](docs/Development-Plan.md)
- [Deployment Guide](docs/Deployment-Guide.md)

## 🏗️ Project Structure

```
src/
├── Domain/                    # Core business logic
│   ├── Entities/             # Domain entities
│   ├── ValueObjects/         # Value objects
│   ├── Services/             # Domain services
│   ├── Events/               # Domain events
│   └── Interfaces/           # Domain interfaces
├── Application/              # Use cases and application logic
│   ├── Commands/             # CQRS commands
│   ├── Queries/              # CQRS queries
│   ├── Handlers/             # Command/Query handlers
│   ├── DTOs/                 # Data transfer objects
│   ├── Interfaces/           # Application interfaces
│   └── Services/             # Application services
├── Infrastructure/           # External concerns
│   ├── Data/                 # Data access
│   │   ├── PostgreSQL/       # PostgreSQL implementation
│   │   └── DynamoDB/         # DynamoDB implementation
│   ├── ExternalServices/     # External API integrations
│   ├── AWS/                  # AWS service integrations
│   └── Repositories/         # Repository implementations
├── API/                      # Web API layer
│   ├── Controllers/          # API controllers
│   ├── Middleware/           # Custom middleware
│   ├── Models/               # Request/Response models
│   └── Program.cs            # Application entry point
└── Lambda/                   # AWS Lambda entry point
    └── LambdaEntryPoint.cs   # Lambda function handler

tests/
├── Domain.Tests/             # Domain layer tests
├── Application.Tests/        # Application layer tests
├── Infrastructure.Tests/     # Infrastructure layer tests
├── API.Tests/                # API layer tests
└── Integration.Tests/        # Integration tests

infrastructure/
├── terraform/                # Terraform configurations
└── serverless/               # Serverless configurations

scripts/
├── setup-dev.ps1            # Development setup script
├── run-tests.ps1            # Test execution script
└── deploy.ps1               # Deployment script
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Development` |
| `DB_CONNECTION_STRING` | PostgreSQL connection string | `Host=localhost;Database=dotnet_api_dev;Username=postgres;Password=password` |
| `AWS_REGION` | AWS region | `us-east-1` |
| `OKTA_AUTHORITY` | Okta authority URL | `https://dev-123456.okta.com/oauth2/default` |
| `OKTA_AUDIENCE` | Okta audience | `api://dotnet-api-dev` |

### AWS Services

- **Lambda**: Serverless compute
- **API Gateway**: API management
- **RDS PostgreSQL**: Relational database
- **DynamoDB**: NoSQL database
- **S3**: File storage
- **SQS**: Message queuing
- **SNS**: Notifications
- **EventBridge**: Event routing
- **CloudWatch**: Logging and monitoring
- **X-Ray**: Request tracing

## 🧪 Testing Strategy

- **Unit Tests**: 80% coverage target with xUnit and Moq
- **Integration Tests**: End-to-end API testing
- **Contract Tests**: API contract validation with Pact
- **Security Tests**: OWASP, SAST, and DAST scanning
- **Load Tests**: Performance and scalability testing

## 📊 Monitoring & Observability

- **Structured Logging**: Serilog with CloudWatch integration
- **Health Checks**: `/health`, `/ready`, `/live` endpoints
- **Metrics**: Custom CloudWatch metrics
- **Tracing**: AWS X-Ray request tracing
- **Alerts**: CloudWatch alarms for errors and performance

## 🔒 Security

- **Authentication**: OAuth 2.0 Client Credentials with Okta
- **Authorization**: Role-based access control (RBAC)
- **HTTPS Only**: Enforced in production
- **Rate Limiting**: 100 requests per minute per client
- **Input Validation**: FluentValidation for all inputs
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, etc.
- **Secrets Management**: AWS Secrets Manager

## 🚀 Performance

- **Cold Start**: <100ms target
- **Response Time**: <200ms for 95th percentile
- **Throughput**: 1000 requests per second
- **Memory Usage**: <512MB per Lambda instance
- **Caching**: In-memory caching with TTL

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: Check the `docs/` folder
- **Issues**: Create an issue in the repository
- **Discussions**: Use GitHub Discussions for questions

## 🎯 Development Status

### ✅ Completed Phases

**Phase 1: Foundation & Project Setup** - ✅ **COMPLETED**
- Clean Architecture solution structure
- .NET 8 projects with proper references
- Docker Compose development environment
- Development scripts (PowerShell)
- VS Code configuration (debugging, tasks, settings)
- Serilog logging configuration
- Application settings (Development, Production)

**Phase 2: Domain Layer Implementation** - 🚧 **IN PROGRESS (75% Complete)**
- ✅ BaseEntity<TId> with common properties and methods
- ✅ Email and FullName value objects with validation
- ✅ User entity with comprehensive business logic
- ✅ UserRole enum (User, Manager, Admin)
- ✅ Domain events infrastructure (IDomainEvent, BaseDomainEvent)
- ✅ User-specific domain events
- ✅ ProductCategory enum with 11 categories
- ✅ Money value object with currency operations
- ✅ Product entity with inventory management
- ✅ OrderStatus enum with comprehensive order states
- ✅ Order entity with order management and calculations
- ✅ OrderItem value object with product snapshots
- 🚧 Domain services (in progress)
- ⏳ Additional domain events

### 📋 Upcoming Phases

- [ ] Phase 3: Application Layer Implementation (CQRS, MediatR, Commands/Queries)
- [ ] Phase 4: Infrastructure Layer Implementation (PostgreSQL, DynamoDB, AWS services)
- [ ] Phase 5: API Layer Implementation (Controllers, Middleware, Authentication)
- [ ] Phase 6: Lambda Integration (Entry point, optimization)
- [ ] Phase 7: Caching and Performance (Redis, in-memory caching)
- [ ] Phase 8: Testing Implementation (Unit, Integration, Contract tests)
- [ ] Phase 9: Infrastructure as Code (Terraform, Serverless, SAM)
- [ ] Phase 10: CI/CD Pipeline (GitHub Actions)
- [ ] Phase 11: Documentation and Examples (API docs, guides)
- [ ] Phase 12: Performance Testing and Optimization

### 📊 Current Metrics

- **Overall Progress**: 31% (Phase 1 complete, Phase 2 75% complete)
- **Build Status**: ✅ All projects build successfully
- **Test Coverage**: Not yet measured (Phase 8)
- **Documentation**: ✅ Comprehensive documentation created
- **Domain Entities**: 4/4 completed (User ✅, Product ✅, Order ✅, OrderItem ✅)
- **Domain Services**: 0/3 completed (User 🚧, Order ⏳, Product ⏳)

---

**Built with ❤️ using ASP.NET Core 8 and AWS Lambda**