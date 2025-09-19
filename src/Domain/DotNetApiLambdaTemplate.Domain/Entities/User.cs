using DotNetApiLambdaTemplate.Domain.Common;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using DotNetApiLambdaTemplate.Domain.Enums;

namespace DotNetApiLambdaTemplate.Domain.Entities;

/// <summary>
/// User entity representing a system user
/// </summary>
public class User : BaseEntity<Guid>
{
    /// <summary>
    /// User's full name
    /// </summary>
    public FullName Name { get; private set; } = null!;

    /// <summary>
    /// User's email address
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// User's role in the system
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Last login date and time
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// User's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// User's department (optional)
    /// </summary>
    public string? Department { get; private set; }

    /// <summary>
    /// User's job title (optional)
    /// </summary>
    public string? JobTitle { get; private set; }

    /// <summary>
    /// External user ID from identity provider (optional)
    /// </summary>
    public string? ExternalId { get; private set; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public string TimeZone { get; private set; } = "UTC";

    /// <summary>
    /// User's preferred language
    /// </summary>
    public string Language { get; private set; } = "en-US";

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private User() { }

    /// <summary>
    /// Creates a new User instance
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="name">User's full name</param>
    /// <param name="email">User's email address</param>
    /// <param name="role">User's role</param>
    /// <param name="createdBy">User or system that created this user</param>
    /// <param name="phoneNumber">Optional phone number</param>
    /// <param name="department">Optional department</param>
    /// <param name="jobTitle">Optional job title</param>
    /// <param name="externalId">Optional external ID</param>
    /// <param name="timeZone">User's timezone</param>
    /// <param name="language">User's preferred language</param>
    public User(
        Guid id,
        FullName name,
        Email email,
        UserRole role,
        string createdBy,
        string? phoneNumber = null,
        string? department = null,
        string? jobTitle = null,
        string? externalId = null,
        string timeZone = "UTC",
        string language = "en-US") : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Role = role;
        IsActive = true;
        PhoneNumber = phoneNumber;
        Department = department;
        JobTitle = jobTitle;
        ExternalId = externalId;
        TimeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        Language = language ?? throw new ArgumentNullException(nameof(language));

        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the user's name
    /// </summary>
    /// <param name="name">New full name</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateName(FullName name, string updatedBy)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Name = name;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the user's email
    /// </summary>
    /// <param name="email">New email address</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateEmail(Email email, string updatedBy)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Email = email;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the user's role
    /// </summary>
    /// <param name="role">New role</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateRole(UserRole role, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Role = role;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the user's contact information
    /// </summary>
    /// <param name="phoneNumber">New phone number</param>
    /// <param name="department">New department</param>
    /// <param name="jobTitle">New job title</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateContactInfo(string? phoneNumber, string? department, string? jobTitle, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        PhoneNumber = phoneNumber;
        Department = department;
        JobTitle = jobTitle;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the user's preferences
    /// </summary>
    /// <param name="timeZone">New timezone</param>
    /// <param name="language">New language</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdatePreferences(string timeZone, string language, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(timeZone)) throw new ArgumentException("TimeZone cannot be null or empty", nameof(timeZone));
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language cannot be null or empty", nameof(language));
        if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        TimeZone = timeZone;
        Language = language;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates the user account
    /// </summary>
    /// <param name="activatedBy">User or system activating the account</param>
    public void Activate(string activatedBy)
    {
        if (string.IsNullOrWhiteSpace(activatedBy)) throw new ArgumentException("ActivatedBy cannot be null or empty", nameof(activatedBy));

        IsActive = true;
        SetUpdated(activatedBy);
    }

    /// <summary>
    /// Deactivates the user account
    /// </summary>
    /// <param name="deactivatedBy">User or system deactivating the account</param>
    public void Deactivate(string deactivatedBy)
    {
        if (string.IsNullOrWhiteSpace(deactivatedBy)) throw new ArgumentException("DeactivatedBy cannot be null or empty", nameof(deactivatedBy));

        IsActive = false;
        SetUpdated(deactivatedBy);
    }

    /// <summary>
    /// Records a successful login
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        SetUpdated("System");
    }

    /// <summary>
    /// Updates the external ID
    /// </summary>
    /// <param name="externalId">External ID from identity provider</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateExternalId(string? externalId, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        ExternalId = externalId;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the user has a specific role
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role</returns>
    public bool HasRole(UserRole role)
    {
        return Role == role;
    }

    /// <summary>
    /// Checks if the user is an administrator
    /// </summary>
    /// <returns>True if user is an admin</returns>
    public bool IsAdmin()
    {
        return Role == UserRole.Admin;
    }

    /// <summary>
    /// Checks if the user is a manager or admin
    /// </summary>
    /// <returns>True if user is a manager or admin</returns>
    public bool IsManagerOrAdmin()
    {
        return Role == UserRole.Admin || Role == UserRole.Manager;
    }
}
