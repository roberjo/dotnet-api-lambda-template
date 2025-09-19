using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Services;

/// <summary>
/// Domain service for user-related business logic
/// </summary>
public class UserDomainService : IUserDomainService
{
    private readonly IUserRepository _userRepository;

    public UserDomainService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Validates if a user can be created with the given email
    /// </summary>
    public async Task<bool> IsEmailAvailableAsync(Email email, CancellationToken cancellationToken = default)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return existingUser == null;
    }

    /// <summary>
    /// Validates if a user can update their email to the new email
    /// </summary>
    public async Task<bool> CanUpdateEmailAsync(Guid userId, Email newEmail, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (newEmail == null)
            throw new ArgumentNullException(nameof(newEmail));

        // Check if the new email is already taken by another user
        var existingUser = await _userRepository.GetByEmailAsync(newEmail, cancellationToken);
        return existingUser == null || existingUser.Id == userId;
    }

    /// <summary>
    /// Validates if a user can be assigned a specific role
    /// </summary>
    public async Task<bool> CanAssignRoleAsync(Guid userId, UserRole newRole, Guid assignedBy, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (assignedBy == Guid.Empty)
            throw new ArgumentException("AssignedBy cannot be empty", nameof(assignedBy));

        // Get the user being assigned the role
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return false;

        // Get the user assigning the role
        var assigner = await _userRepository.GetByIdAsync(assignedBy, cancellationToken);
        if (assigner == null)
            return false;

        // Business rules for role assignment
        return CanAssignRoleInternal(user, newRole, assigner);
    }

    /// <summary>
    /// Validates if a user can be deactivated
    /// </summary>
    public async Task<bool> CanDeactivateUserAsync(Guid userId, Guid deactivatedBy, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (deactivatedBy == Guid.Empty)
            throw new ArgumentException("DeactivatedBy cannot be empty", nameof(deactivatedBy));

        // Cannot deactivate yourself
        if (userId == deactivatedBy)
            return false;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var deactivator = await _userRepository.GetByIdAsync(deactivatedBy, cancellationToken);

        if (user == null || deactivator == null)
            return false;

        // Business rules for user deactivation
        return CanDeactivateUserInternal(user, deactivator);
    }

    /// <summary>
    /// Generates a unique external ID for a user
    /// </summary>
    public async Task<string> GenerateUniqueExternalIdAsync(CancellationToken cancellationToken = default)
    {
        string externalId;
        bool isUnique = false;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            externalId = GenerateExternalId();
            var existingUser = await _userRepository.GetByExternalIdAsync(externalId, cancellationToken);
            isUnique = existingUser == null;
            attempts++;
        }
        while (!isUnique && attempts < maxAttempts);

        if (!isUnique)
            throw new InvalidOperationException("Unable to generate unique external ID after multiple attempts");

        return externalId;
    }

    /// <summary>
    /// Validates user permissions for a specific action
    /// </summary>
    public async Task<bool> HasPermissionAsync(Guid userId, string action, Guid? resourceId = null, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty", nameof(action));

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || !user.IsActive)
            return false;

        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(action);
    }

    /// <summary>
    /// Gets the user's effective permissions
    /// </summary>
    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || !user.IsActive)
            return Enumerable.Empty<string>();

        return GetPermissionsForRole(user.Role);
    }

    /// <summary>
    /// Validates if a user can access a specific resource
    /// </summary>
    public async Task<bool> CanAccessResourceAsync(Guid userId, string resourceType, Guid resourceId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(resourceType))
            throw new ArgumentException("ResourceType cannot be null or empty", nameof(resourceType));
        if (resourceId == Guid.Empty)
            throw new ArgumentException("ResourceId cannot be empty", nameof(resourceId));

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || !user.IsActive)
            return false;

        // Admin users can access all resources
        if (user.IsAdmin())
            return true;

        // For user resources, users can only access their own
        if (resourceType.Equals("User", StringComparison.OrdinalIgnoreCase))
            return userId == resourceId;

        // For order resources, users can access their own orders
        if (resourceType.Equals("Order", StringComparison.OrdinalIgnoreCase))
        {
            // This would require checking if the order belongs to the user
            // For now, we'll implement a basic check
            return await CanAccessOrderAsync(userId, resourceId, cancellationToken);
        }

        // For product resources, all active users can access
        if (resourceType.Equals("Product", StringComparison.OrdinalIgnoreCase))
            return true;

        // Default: no access
        return false;
    }

    #region Private Methods

    /// <summary>
    /// Internal method to validate role assignment business rules
    /// </summary>
    private static bool CanAssignRoleInternal(User user, UserRole newRole, User assigner)
    {
        // Only admins can assign roles
        if (!assigner.IsAdmin())
            return false;

        // Cannot assign admin role to yourself
        if (user.Id == assigner.Id && newRole == UserRole.Admin)
            return false;

        // Cannot demote the last admin
        if (user.Role == UserRole.Admin && newRole != UserRole.Admin)
        {
            // This would require checking if there are other admins
            // For now, we'll allow it but this should be enhanced
            return true;
        }

        return true;
    }

    /// <summary>
    /// Internal method to validate user deactivation business rules
    /// </summary>
    private static bool CanDeactivateUserInternal(User user, User deactivator)
    {
        // Only admins and managers can deactivate users
        if (!deactivator.IsManagerOrAdmin())
            return false;

        // Admins can deactivate anyone
        if (deactivator.IsAdmin())
            return true;

        // Managers can only deactivate regular users
        if (deactivator.Role == UserRole.Manager)
            return user.Role == UserRole.User;

        return false;
    }

    /// <summary>
    /// Generates a random external ID
    /// </summary>
    private static string GenerateExternalId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random().Next(1000, 9999);
        return $"EXT_{timestamp}_{random}";
    }

    /// <summary>
    /// Gets permissions for a specific role
    /// </summary>
    private static IEnumerable<string> GetPermissionsForRole(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => new[]
            {
                "users.create", "users.read", "users.update", "users.delete",
                "products.create", "products.read", "products.update", "products.delete",
                "orders.create", "orders.read", "orders.update", "orders.delete",
                "system.admin", "system.manage"
            },
            UserRole.Manager => new[]
            {
                "users.read", "users.update",
                "products.create", "products.read", "products.update",
                "orders.create", "orders.read", "orders.update",
                "system.manage"
            },
            UserRole.User => new[]
            {
                "users.read", "users.update",
                "products.read",
                "orders.create", "orders.read", "orders.update"
            },
            _ => Enumerable.Empty<string>()
        };
    }

    /// <summary>
    /// Checks if a user can access a specific order
    /// </summary>
    private async Task<bool> CanAccessOrderAsync(Guid userId, Guid orderId, CancellationToken cancellationToken)
    {
        // This would require an order repository to check ownership
        // For now, we'll return true for demonstration
        // In a real implementation, this would check if the order belongs to the user
        return true;
    }

    #endregion
}

/// <summary>
/// Repository interface for user data access
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
