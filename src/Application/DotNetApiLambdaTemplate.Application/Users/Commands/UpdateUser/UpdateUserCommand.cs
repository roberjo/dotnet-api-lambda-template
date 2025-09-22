using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Users.Commands.UpdateUser;

/// <summary>
/// Command to update an existing user
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    /// <summary>
    /// User ID to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// New first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// New last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// New email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// New role
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// New phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// New department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// New job title
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// New timezone
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// New language preference
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Whether to activate/deactivate the user
    /// </summary>
    public bool? IsActive { get; set; }
}
