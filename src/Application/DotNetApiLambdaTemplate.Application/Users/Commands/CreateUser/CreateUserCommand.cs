using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.Models;
using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using MediatR;

namespace DotNetApiLambdaTemplate.Application.Users.Commands.CreateUser;

/// <summary>
/// Command to create a new user
/// </summary>
public record CreateUserCommand : ICommand<Result<User>>
{
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
    /// User's role
    /// </summary>
    public UserRole Role { get; init; } = UserRole.User;

    /// <summary>
    /// User's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// User's department (optional)
    /// </summary>
    public string? Department { get; init; }

    /// <summary>
    /// User's job title (optional)
    /// </summary>
    public string? JobTitle { get; init; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public string TimeZone { get; init; } = "UTC";

    /// <summary>
    /// User's preferred language
    /// </summary>
    public string Language { get; init; } = "en-US";

    /// <summary>
    /// User or system creating this user
    /// </summary>
    public required string CreatedBy { get; init; }
}
