using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Services;

/// <summary>
/// Domain service interface for user-related business logic
/// </summary>
public interface IUserDomainService
{
    /// <summary>
    /// Validates if a user can be created with the given email
    /// </summary>
    /// <param name="email">Email to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email is available for use</returns>
    Task<bool> IsEmailAvailableAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user can update their email to the new email
    /// </summary>
    /// <param name="userId">Current user ID</param>
    /// <param name="newEmail">New email to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if new email is available</returns>
    Task<bool> CanUpdateEmailAsync(Guid userId, Email newEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user can be assigned a specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="newRole">New role to assign</param>
    /// <param name="assignedBy">User ID of who is assigning the role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if role can be assigned</returns>
    Task<bool> CanAssignRoleAsync(Guid userId, UserRole newRole, Guid assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user can be deactivated
    /// </summary>
    /// <param name="userId">User ID to deactivate</param>
    /// <param name="deactivatedBy">User ID of who is deactivating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user can be deactivated</returns>
    Task<bool> CanDeactivateUserAsync(Guid userId, Guid deactivatedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique external ID for a user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unique external ID</returns>
    Task<string> GenerateUniqueExternalIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates user permissions for a specific action
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="action">Action to validate</param>
    /// <param name="resourceId">Optional resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has permission</returns>
    Task<bool> HasPermissionAsync(Guid userId, string action, Guid? resourceId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user's effective permissions
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of permissions</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user can access a specific resource
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="resourceType">Type of resource</param>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user can access the resource</returns>
    Task<bool> CanAccessResourceAsync(Guid userId, string resourceType, Guid resourceId, CancellationToken cancellationToken = default);
}
