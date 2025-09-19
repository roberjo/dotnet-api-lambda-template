using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.Models;
using DotNetApiLambdaTemplate.Application.Users.Commands.CreateUser;
using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.Services;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IUserRepository = DotNetApiLambdaTemplate.Application.Common.Interfaces.IUserRepository;

namespace DotNetApiLambdaTemplate.Application.Tests.Users.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserDomainService> _mockUserDomainService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _mockLogger;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUserDomainService = new Mock<IUserDomainService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        _handler = new CreateUserCommandHandler(
            _mockUserDomainService.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.User,
            CreatedBy = "System"
        };

        var email = Email.Create(command.Email);
        var fullName = FullName.Create(command.FirstName, command.LastName);
        var expectedUser = new User(
            Guid.NewGuid(),
            fullName,
            email,
            command.Role,
            command.CreatedBy);

        _mockUserDomainService
            .Setup(x => x.IsEmailAvailableAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedUser.Id, result.Value.Id);
        Assert.Equal(command.Email, result.Value.Email.Value);
        Assert.Equal(command.FirstName, result.Value.Name.FirstName);
        Assert.Equal(command.LastName, result.Value.Name.LastName);
    }

    [Fact]
    public async Task Handle_WithUnavailableEmail_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.User,
            CreatedBy = "System"
        };

        _mockUserDomainService
            .Setup(x => x.IsEmailAvailableAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Email address is already taken", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.User,
            CreatedBy = "System"
        };

        _mockUserDomainService
            .Setup(x => x.IsEmailAvailableAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Failed to create user", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "invalid-email",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.User,
            CreatedBy = "System"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Failed to create user", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WithInvalidName_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FirstName = "",
            LastName = "Doe",
            Role = UserRole.User,
            CreatedBy = "System"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Failed to create user", result.Error);
        Assert.Null(result.Value);
    }

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Admin)]
    public async Task Handle_WithDifferentRoles_CreatesUserWithCorrectRole(UserRole role)
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = role,
            CreatedBy = "System"
        };

        var email = Email.Create(command.Email);
        var fullName = FullName.Create(command.FirstName, command.LastName);
        var expectedUser = new User(
            Guid.NewGuid(),
            fullName,
            email,
            role,
            command.CreatedBy);

        _mockUserDomainService
            .Setup(x => x.IsEmailAvailableAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(role, result.Value!.Role);
    }
}
