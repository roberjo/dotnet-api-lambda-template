using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.Models;
using DotNetApiLambdaTemplate.Application.Users.Queries.GetUsers;
using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IUserRepository = DotNetApiLambdaTemplate.Application.Common.Interfaces.IUserRepository;

namespace DotNetApiLambdaTemplate.Application.Tests.Users.Queries.GetUsers;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<GetUsersQueryHandler>> _mockLogger;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<GetUsersQueryHandler>>();
        _handler = new GetUsersQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var users = new List<User>
        {
            CreateTestUser("user1@example.com", "John", "Doe", UserRole.User),
            CreateTestUser("user2@example.com", "Jane", "Smith", UserRole.Manager)
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Role,
                query.IsActive,
                query.SortBy,
                query.SortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mockUserRepository
            .Setup(x => x.GetCountAsync(
                query.SearchTerm,
                query.Role,
                query.IsActive,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Users.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(1, result.Value.PageNumber);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(1, result.Value.TotalPages);
        Assert.False(result.Value.HasPreviousPage);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_FiltersUsers()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageNumber = 1,
            PageSize = 10,
            SearchTerm = "john"
        };

        var users = new List<User>
        {
            CreateTestUser("john@example.com", "John", "Doe", UserRole.User)
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Role,
                query.IsActive,
                query.SortBy,
                query.SortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mockUserRepository
            .Setup(x => x.GetCountAsync(
                query.SearchTerm,
                query.Role,
                query.IsActive,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Users);
        Assert.Equal("john@example.com", result.Value.Users.First().Email);
    }

    [Fact]
    public async Task Handle_WithRoleFilter_FiltersUsersByRole()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Role = UserRole.Manager
        };

        var users = new List<User>
        {
            CreateTestUser("manager@example.com", "Manager", "User", UserRole.Manager)
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Role,
                query.IsActive,
                query.SortBy,
                query.SortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mockUserRepository
            .Setup(x => x.GetCountAsync(
                query.SearchTerm,
                query.Role,
                query.IsActive,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Users);
        Assert.Equal("Manager", result.Value.Users.First().Role);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPaginationInfo()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageNumber = 2,
            PageSize = 5
        };

        var users = new List<User>
        {
            CreateTestUser("user6@example.com", "User", "Six", UserRole.User),
            CreateTestUser("user7@example.com", "User", "Seven", UserRole.User)
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Role,
                query.IsActive,
                query.SortBy,
                query.SortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mockUserRepository
            .Setup(x => x.GetCountAsync(
                query.SearchTerm,
                query.Role,
                query.IsActive,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(12); // Total of 12 users

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(5, result.Value.PageSize);
        Assert.Equal(12, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages); // 12 users / 5 per page = 3 pages
        Assert.True(result.Value.HasPreviousPage);
        Assert.True(result.Value.HasNextPage);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_ReturnsFailureResult()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<UserRole?>(),
                It.IsAny<bool?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Failed to get users", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ReturnsEmptyResponse()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Role,
                query.IsActive,
                query.SortBy,
                query.SortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        _mockUserRepository
            .Setup(x => x.GetCountAsync(
                query.SearchTerm,
                query.Role,
                query.IsActive,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Users);
        Assert.Equal(0, result.Value.TotalCount);
        Assert.Equal(0, result.Value.TotalPages);
    }

    private static User CreateTestUser(string email, string firstName, string lastName, UserRole role)
    {
        var userEmail = Email.Create(email);
        var fullName = FullName.Create(firstName, lastName);
        return new User(Guid.NewGuid(), fullName, userEmail, role, "System");
    }
}
