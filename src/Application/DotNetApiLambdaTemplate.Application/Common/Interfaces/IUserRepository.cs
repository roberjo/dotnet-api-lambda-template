using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;

namespace DotNetApiLambdaTemplate.Application.Common.Interfaces;

/// <summary>
/// Repository interface for user data access
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for filtering</param>
    /// <param name="role">Filter by role</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction (asc/desc)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    Task<IEnumerable<User>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        bool? isActive = null,
        string? sortBy = null,
        string? sortDirection = "asc",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of users matching the filter criteria
    /// </summary>
    /// <param name="searchTerm">Search term for filtering</param>
    /// <param name="role">Filter by role</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count</returns>
    Task<int> GetCountAsync(
        string? searchTerm = null,
        UserRole? role = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user
    /// </summary>
    /// <param name="user">User to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Added user</returns>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="user">User to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">User ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
