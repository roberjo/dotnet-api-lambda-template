using DotNetApiLambdaTemplate.Domain.Enums;

namespace DotNetApiLambdaTemplate.Application.Common.DTOs;

/// <summary>
/// Data Transfer Object for User entity
/// </summary>
public class UserDto
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's role
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Indicates if the user is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// User's job title
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// User's external system ID
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// User's preferred language
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// User who last updated this record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}
