namespace DotNetApiLambdaTemplate.Domain.Enums;

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular user with basic permissions
    /// </summary>
    User = 0,

    /// <summary>
    /// Manager with elevated permissions
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Administrator with full system access
    /// </summary>
    Admin = 2
}
