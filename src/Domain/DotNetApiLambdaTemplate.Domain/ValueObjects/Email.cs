using System.Text.RegularExpressions;

namespace DotNetApiLambdaTemplate.Domain.ValueObjects;

/// <summary>
/// Email value object with validation
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// The email address value
    /// </summary>
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Email instance with validation
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <returns>A new Email instance</returns>
    /// <exception cref="ArgumentException">Thrown when email format is invalid</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        var trimmedEmail = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(trimmedEmail))
            throw new ArgumentException($"Invalid email format: {email}", nameof(email));

        if (trimmedEmail.Length > 254)
            throw new ArgumentException("Email cannot exceed 254 characters", nameof(email));

        return new Email(trimmedEmail);
    }

    /// <summary>
    /// Implicit conversion to string
    /// </summary>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Explicit conversion from string
    /// </summary>
    public static explicit operator Email(string email) => Create(email);

    /// <summary>
    /// Equality comparison
    /// </summary>
    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Equality comparison
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Email);
    }

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// String representation
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Email? left, Email? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Email? left, Email? right)
    {
        return !Equals(left, right);
    }
}
