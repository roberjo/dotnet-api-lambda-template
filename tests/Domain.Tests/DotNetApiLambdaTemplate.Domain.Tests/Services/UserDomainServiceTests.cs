using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.Services;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Moq;
using Xunit;

namespace DotNetApiLambdaTemplate.Domain.Tests.Services;

public class UserDomainServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserDomainService _userDomainService;

    public UserDomainServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userDomainService = new UserDomainService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WhenEmailIsAvailable_ReturnsTrue()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userDomainService.IsEmailAvailableAsync(email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WhenEmailIsNotAvailable_ReturnsFalse()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var existingUser = CreateTestUser();
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userDomainService.IsEmailAvailableAsync(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanAssignRoleAsync_WhenAssignerIsAdmin_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var assignerId = Guid.NewGuid();
        var user = CreateTestUser(id: userId, role: UserRole.User);
        var assigner = CreateTestUser(id: assignerId, role: UserRole.Admin);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.GetByIdAsync(assignerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assigner);

        // Act
        var result = await _userDomainService.CanAssignRoleAsync(userId, UserRole.Manager, assignerId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanAssignRoleAsync_WhenAssignerIsNotAdmin_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var assignerId = Guid.NewGuid();
        var user = CreateTestUser(id: userId, role: UserRole.User);
        var assigner = CreateTestUser(id: assignerId, role: UserRole.User);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.GetByIdAsync(assignerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assigner);

        // Act
        var result = await _userDomainService.CanAssignRoleAsync(userId, UserRole.Manager, assignerId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDeactivateUserAsync_WhenDeactivatingSelf_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _userDomainService.CanDeactivateUserAsync(userId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GenerateUniqueExternalIdAsync_ReturnsUniqueId()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByExternalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userDomainService.GenerateUniqueExternalIdAsync();

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("EXT_", result);
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WhenUserIsAdmin_ReturnsAdminPermissions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUser(id: userId, role: UserRole.Admin);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var permissions = await _userDomainService.GetUserPermissionsAsync(userId);

        // Assert
        Assert.Contains("users.create", permissions);
        Assert.Contains("users.read", permissions);
        Assert.Contains("users.update", permissions);
        Assert.Contains("users.delete", permissions);
        Assert.Contains("system.admin", permissions);
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WhenUserIsRegularUser_ReturnsUserPermissions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUser(id: userId, role: UserRole.User);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var permissions = await _userDomainService.GetUserPermissionsAsync(userId);

        // Assert
        Assert.Contains("users.read", permissions);
        Assert.Contains("users.update", permissions);
        Assert.Contains("products.read", permissions);
        Assert.Contains("orders.create", permissions);
        Assert.DoesNotContain("users.create", permissions);
        Assert.DoesNotContain("users.delete", permissions);
        Assert.DoesNotContain("system.admin", permissions);
    }

    private static User CreateTestUser(
        Guid? id = null,
        string email = "test@example.com",
        string firstName = "Test",
        string lastName = "User",
        UserRole role = UserRole.User)
    {
        var userId = id ?? Guid.NewGuid();
        var userEmail = Email.Create(email);
        var userName = FullName.Create(firstName, lastName);

        return new User(userId, userName, userEmail, role, "System");
    }
}
