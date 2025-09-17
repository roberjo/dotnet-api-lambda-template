# Technical Specification

## Overview

This document provides detailed technical specifications for the ASP.NET Core 8 Web API Lambda template project, including architecture, implementation details, and technical requirements.

## Architecture Overview

### Clean Architecture Layers

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

### Technology Stack

- **Framework**: ASP.NET Core 8
- **Runtime**: .NET 8
- **Database**: PostgreSQL (EF Core) + DynamoDB (AWS SDK)
- **Caching**: In-memory caching
- **Authentication**: OAuth 2.0 Client Credentials + Okta
- **Logging**: Serilog + CloudWatch
- **Testing**: xUnit + Moq + Integration Tests
- **Infrastructure**: Terraform Cloud + AWS Lambda
- **CI/CD**: GitHub Actions

## Detailed Technical Requirements

### 1. Project Structure

```
src/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Services/
│   ├── Events/
│   └── Interfaces/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── Infrastructure/
│   ├── Data/
│   │   ├── PostgreSQL/
│   │   └── DynamoDB/
│   ├── ExternalServices/
│   ├── AWS/
│   └── Repositories/
├── API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Models/
│   └── Program.cs
└── Lambda/
    └── LambdaEntryPoint.cs

tests/
├── Domain.Tests/
├── Application.Tests/
├── Infrastructure.Tests/
├── API.Tests/
└── Integration.Tests/

docs/
├── PRD.md
├── ARDs.md
├── Technical-Specification.md
└── Deployment-Guide.md

infrastructure/
├── terraform/
└── serverless/

scripts/
├── setup-dev.ps1
├── run-tests.ps1
└── deploy.ps1
```

### 2. Database Design

#### PostgreSQL Schema (EF Core)

```sql
-- Users Table
CREATE TABLE Users (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Email VARCHAR(255) UNIQUE NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Role VARCHAR(50) NOT NULL CHECK (Role IN ('Admin', 'Manager', 'User')),
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Products Table
CREATE TABLE Products (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL,
    Category VARCHAR(100),
    StockQuantity INTEGER DEFAULT 0,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Orders Table
CREATE TABLE Orders (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES Users(Id),
    OrderNumber VARCHAR(50) UNIQUE NOT NULL,
    Status VARCHAR(50) NOT NULL CHECK (Status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    TotalAmount DECIMAL(10,2) NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- OrderItems Table
CREATE TABLE OrderItems (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrderId UUID NOT NULL REFERENCES Orders(Id),
    ProductId UUID NOT NULL REFERENCES Products(Id),
    Quantity INTEGER NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

#### DynamoDB Schema

```json
{
  "UserSessions": {
    "PartitionKey": "UserId",
    "SortKey": "SessionId",
    "Attributes": {
      "UserId": "S",
      "SessionId": "S",
      "CreatedAt": "S",
      "ExpiresAt": "S",
      "Data": "M"
    }
  },
  "EventStore": {
    "PartitionKey": "AggregateId",
    "SortKey": "Version",
    "Attributes": {
      "AggregateId": "S",
      "Version": "N",
      "EventType": "S",
      "EventData": "M",
      "Timestamp": "S"
    }
  }
}
```

### 3. API Endpoints Specification

#### User Management API

```http
GET    /api/v1/users                    # List users with pagination
GET    /api/v1/users/{id}               # Get user by ID
POST   /api/v1/users                    # Create user
PUT    /api/v1/users/{id}               # Update user
DELETE /api/v1/users/{id}               # Delete user
GET    /api/v1/users/search             # Search users with filters
```

#### Product Management API

```http
GET    /api/v1/products                 # List products with pagination
GET    /api/v1/products/{id}            # Get product by ID
POST   /api/v1/products                 # Create product
PUT    /api/v1/products/{id}            # Update product
DELETE /api/v1/products/{id}            # Delete product
GET    /api/v1/products/search          # Search products with filters
POST   /api/v1/products/{id}/upload     # Upload product image
```

#### Order Management API

```http
GET    /api/v1/orders                   # List orders with pagination
GET    /api/v1/orders/{id}              # Get order by ID
POST   /api/v1/orders                   # Create order
PUT    /api/v1/orders/{id}              # Update order
DELETE /api/v1/orders/{id}              # Cancel order
POST   /api/v1/orders/{id}/process      # Process order
POST   /api/v1/orders/{id}/ship         # Ship order
```

#### Health Check API

```http
GET    /health                         # Basic health check
GET    /ready                         # Readiness check
GET    /live                          # Liveness check
```

### 4. Authentication & Authorization

#### OAuth 2.0 Client Credentials Flow

```csharp
// Configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-okta-domain.okta.com/oauth2/default";
        options.Audience = "api://your-api-identifier";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });
```

#### Authorization Policies

```csharp
// Role-based policies
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => 
        policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("UserOrAbove", policy => 
        policy.RequireRole("User", "Manager", "Admin"));
});
```

### 5. CQRS Implementation

#### Command Example

```csharp
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

#### Query Example

```csharp
public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### 6. Caching Implementation

```csharp
public class CachedUserService : IUserService
{
    private readonly IUserService _userService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedUserService> _logger;

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var cacheKey = $"user_{userId}";
        
        if (_cache.TryGetValue(cacheKey, out UserDto cachedUser))
        {
            return cachedUser;
        }

        var user = await _userService.GetUserByIdAsync(userId);
        
        _cache.Set(cacheKey, user, TimeSpan.FromMinutes(15));
        
        return user;
    }
}
```

### 7. Logging Configuration

```csharp
// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.AmazonCloudWatch(new CloudWatchSinkOptions
    {
        LogGroup = "/aws/lambda/dotnet-api-lambda-template",
        Region = "us-east-1"
    })
    .Enrich.WithProperty("Application", "DotNetApiLambdaTemplate")
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    .Enrich.WithCorrelationId()
    .CreateLogger();
```

### 8. Error Handling

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse
        {
            Error = "An error occurred while processing your request",
            CorrelationId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => 400,
            NotFoundException => 404,
            UnauthorizedException => 401,
            _ => 500
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

### 9. AWS Service Integration

#### S3 File Upload

```csharp
public class S3FileService : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3FileService> _logger;

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = "your-bucket-name",
            Key = $"uploads/{Guid.NewGuid()}/{fileName}",
            InputStream = fileStream,
            ContentType = contentType
        };

        var response = await _s3Client.PutObjectAsync(request);
        return $"https://your-bucket-name.s3.amazonaws.com/{request.Key}";
    }
}
```

#### SQS Message Processing

```csharp
public class OrderProcessingService : IOrderProcessingService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<OrderProcessingService> _logger;

    public async Task ProcessOrderAsync(OrderDto order)
    {
        var message = new SendMessageRequest
        {
            QueueUrl = "https://sqs.us-east-1.amazonaws.com/account/order-processing-queue",
            MessageBody = JsonSerializer.Serialize(order),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["MessageType"] = new MessageAttributeValue
                {
                    StringValue = "OrderProcessing",
                    DataType = "String"
                }
            }
        };

        await _sqsClient.SendMessageAsync(message);
    }
}
```

### 10. Testing Specifications

#### Unit Test Example

```csharp
[Fact]
public async Task CreateUserCommand_ValidInput_ReturnsSuccess()
{
    // Arrange
    var command = new CreateUserCommand
    {
        Email = "test@example.com",
        FirstName = "John",
        LastName = "Doe",
        Role = "User"
    };

    var mockRepository = new Mock<IUserRepository>();
    var handler = new CreateUserCommandHandler(mockRepository.Object, Mock.Of<ILogger<CreateUserCommandHandler>>());

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    mockRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
}
```

#### Integration Test Example

```csharp
[Fact]
public async Task GetUsers_ReturnsListOfUsers()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/v1/users");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var users = JsonSerializer.Deserialize<List<UserDto>>(content);
    
    Assert.NotNull(users);
    Assert.True(users.Count > 0);
}
```

### 11. Performance Requirements

- **Cold Start Time**: < 100ms
- **Response Time**: < 200ms for 95th percentile
- **Throughput**: 1000 requests per second
- **Memory Usage**: < 512MB per Lambda instance
- **Database Connection Pool**: 10-20 connections per instance

### 12. Security Requirements

- **HTTPS Only**: All API endpoints must use HTTPS
- **Rate Limiting**: 100 requests per minute per client
- **Input Validation**: All inputs must be validated using FluentValidation
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
- **Secrets Management**: AWS Secrets Manager for sensitive configuration
- **CORS**: Configurable CORS policy for cross-origin requests

### 13. Monitoring & Observability

- **Logging**: Structured JSON logs to CloudWatch
- **Metrics**: Custom CloudWatch metrics for business operations
- **Tracing**: AWS X-Ray for request tracing
- **Health Checks**: /health, /ready, /live endpoints
- **Alerts**: CloudWatch alarms for errors and performance issues

### 14. Deployment Requirements

- **Infrastructure**: Terraform Cloud for IaC
- **Environments**: dev, qa, test, prod
- **Deployment**: Blue-green deployment strategy
- **CI/CD**: GitHub Actions with automated testing and deployment
- **Rollback**: Automated rollback capability
- **Monitoring**: Real-time monitoring and alerting
