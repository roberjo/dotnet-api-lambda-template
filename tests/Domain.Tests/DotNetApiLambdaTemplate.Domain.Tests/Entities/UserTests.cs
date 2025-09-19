using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using Xunit;

namespace DotNetApiLambdaTemplate.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ReturnsUserWithCorrectProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = Email.Create("test@example.com");
        var name = FullName.Create("John", "Doe");
        var role = UserRole.User;
        var createdBy = "System";

        // Act
        var user = new User(id, name, email, role, createdBy);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.Equal(role, user.Role);
        Assert.True(user.IsActive);
        Assert.True(user.CreatedAt > DateTimeOffset.MinValue);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = Email.Create("test@example.com");
        var role = UserRole.User;
        var createdBy = "System";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new User(id, null!, email, role, createdBy));
    }

    [Fact]
    public void Constructor_WithNullEmail_ThrowsArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = FullName.Create("John", "Doe");
        var role = UserRole.User;
        var createdBy = "System";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new User(id, name, null!, role, createdBy));
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_UpdatesEmail()
    {
        // Arrange
        var user = CreateTestUser();
        var newEmail = Email.Create("new@example.com");
        var updatedBy = "Admin";

        // Act
        user.UpdateEmail(newEmail, updatedBy);

        // Assert
        Assert.Equal(newEmail, user.Email);
        Assert.True(user.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void UpdateEmail_WithNullEmail_ThrowsArgumentNullException()
    {
        // Arrange
        var user = CreateTestUser();
        var updatedBy = "Admin";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => user.UpdateEmail(null!, updatedBy));
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesName()
    {
        // Arrange
        var user = CreateTestUser();
        var newName = FullName.Create("Jane", "Smith");
        var updatedBy = "Admin";

        // Act
        user.UpdateName(newName, updatedBy);

        // Assert
        Assert.Equal(newName, user.Name);
        Assert.True(user.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void UpdateName_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var user = CreateTestUser();
        var updatedBy = "Admin";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => user.UpdateName(null!, updatedBy));
    }

    [Fact]
    public void UpdateRole_WithValidRole_UpdatesRole()
    {
        // Arrange
        var user = CreateTestUser();
        var newRole = UserRole.Manager;
        var updatedBy = "Admin";

        // Act
        user.UpdateRole(newRole, updatedBy);

        // Assert
        Assert.Equal(newRole, user.Role);
        Assert.True(user.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void UpdateRole_WithEmptyUpdatedBy_ThrowsArgumentException()
    {
        // Arrange
        var user = CreateTestUser();
        var newRole = UserRole.Manager;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.UpdateRole(newRole, ""));
    }

    [Fact]
    public void Activate_WhenInactive_ActivatesUser()
    {
        // Arrange
        var user = CreateTestUser();
        user.Deactivate("System");

        // Act
        user.Activate("Admin");

        // Assert
        Assert.True(user.IsActive);
        Assert.True(user.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesUser()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.Deactivate("Admin");

        // Assert
        Assert.False(user.IsActive);
        Assert.True(user.UpdatedAt > DateTime.MinValue);
    }

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Admin)]
    public void Constructor_WithDifferentRoles_CreatesUserWithCorrectRole(UserRole role)
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = Email.Create("test@example.com");
        var name = FullName.Create("John", "Doe");
        var createdBy = "System";

        // Act
        var user = new User(id, name, email, role, createdBy);

        // Assert
        Assert.Equal(role, user.Role);
    }

    private static User CreateTestUser(
        Guid? id = null,
        string email = "test@example.com",
        string firstName = "John",
        string lastName = "Doe",
        UserRole role = UserRole.User)
    {
        var userId = id ?? Guid.NewGuid();
        var userEmail = Email.Create(email);
        var userName = FullName.Create(firstName, lastName);

        return new User(userId, userName, userEmail, role, "System");
    }
}