using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.Models;
using DotNetApiLambdaTemplate.Domain.Entities;
using MediatR;

namespace DotNetApiLambdaTemplate.Application.Users.Queries.GetUsers;

/// <summary>
/// Query to get a list of users with pagination
/// </summary>
public record GetUsersQuery : IQuery<Result<GetUsersResponse>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Search term for filtering users
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by user role
    /// </summary>
    public Domain.Enums.UserRole? Role { get; init; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string? SortDirection { get; init; } = "asc";
}

/// <summary>
/// Response for the GetUsersQuery
/// </summary>
public record GetUsersResponse
{
    /// <summary>
    /// List of users
    /// </summary>
    public required IEnumerable<UserDto> Users { get; init; }

    /// <summary>
    /// Total number of users
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Data transfer object for User
/// </summary>
public record UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// User's full name
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// User's role
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// User's department
    /// </summary>
    public string? Department { get; init; }

    /// <summary>
    /// User's job title
    /// </summary>
    public string? JobTitle { get; init; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public required string TimeZone { get; init; }

    /// <summary>
    /// User's preferred language
    /// </summary>
    public required string Language { get; init; }

    /// <summary>
    /// Date when the user was created
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Date when the user was last updated
    /// </summary>
    public required DateTime UpdatedAt { get; init; }
}
