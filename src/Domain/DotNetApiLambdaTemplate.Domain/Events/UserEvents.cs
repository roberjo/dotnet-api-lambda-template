using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Events;

/// <summary>
/// Event raised when a user is created
/// </summary>
public class UserCreatedEvent : BaseDomainEvent
{
    /// <summary>
    /// The created user's ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// The user's email
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// The user's name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The user's role
    /// </summary>
    public UserRole Role { get; }

    /// <summary>
    /// Who created the user
    /// </summary>
    public string CreatedBy { get; }

    public UserCreatedEvent(Guid userId, Email email, FullName name, UserRole role, string createdBy)
    {
        UserId = userId;
        Email = email.Value;
        Name = name.Value;
        Role = role;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Event raised when a user's email is updated
/// </summary>
public class UserEmailUpdatedEvent : BaseDomainEvent
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// The old email
    /// </summary>
    public string OldEmail { get; }

    /// <summary>
    /// The new email
    /// </summary>
    public string NewEmail { get; }

    /// <summary>
    /// Who updated the email
    /// </summary>
    public string UpdatedBy { get; }

    public UserEmailUpdatedEvent(Guid userId, string oldEmail, Email newEmail, string updatedBy)
    {
        UserId = userId;
        OldEmail = oldEmail;
        NewEmail = newEmail.Value;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Event raised when a user's role is updated
/// </summary>
public class UserRoleUpdatedEvent : BaseDomainEvent
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// The old role
    /// </summary>
    public UserRole OldRole { get; }

    /// <summary>
    /// The new role
    /// </summary>
    public UserRole NewRole { get; }

    /// <summary>
    /// Who updated the role
    /// </summary>
    public string UpdatedBy { get; }

    public UserRoleUpdatedEvent(Guid userId, UserRole oldRole, UserRole newRole, string updatedBy)
    {
        UserId = userId;
        OldRole = oldRole;
        NewRole = newRole;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Event raised when a user is activated
/// </summary>
public class UserActivatedEvent : BaseDomainEvent
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// The user's email
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Who activated the user
    /// </summary>
    public string ActivatedBy { get; }

    public UserActivatedEvent(Guid userId, string email, string activatedBy)
    {
        UserId = userId;
        Email = email;
        ActivatedBy = activatedBy;
    }
}

/// <summary>
/// Event raised when a user is deactivated
/// </summary>
public class UserDeactivatedEvent : BaseDomainEvent
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// The user's email
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Who deactivated the user
    /// </summary>
    public string DeactivatedBy { get; }

    public UserDeactivatedEvent(Guid userId, string email, string deactivatedBy)
    {
        UserId = userId;
        Email = email;
        DeactivatedBy = deactivatedBy;
    }
}

/// <summary>
/// Event raised when a user logs in
/// </summary>
public class UserLoggedInEvent : BaseDomainEvent
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// The user's email
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// The login timestamp
    /// </summary>
    public DateTime LoginTime { get; }

    public UserLoggedInEvent(Guid userId, string email, DateTime loginTime)
    {
        UserId = userId;
        Email = email;
        LoginTime = loginTime;
    }
}
