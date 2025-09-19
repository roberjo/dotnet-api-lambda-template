namespace DotNetApiLambdaTemplate.Domain.ValueObjects;

/// <summary>
/// Full name value object with validation
/// </summary>
public sealed class FullName : IEquatable<FullName>
{
    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Full name as a single string
    /// </summary>
    public string Value => $"{FirstName} {LastName}".Trim();

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    /// Creates a new FullName instance with validation
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <returns>A new FullName instance</returns>
    /// <exception cref="ArgumentException">Thrown when names are invalid</exception>
    public static FullName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));

        var trimmedFirstName = firstName.Trim();
        var trimmedLastName = lastName.Trim();

        if (trimmedFirstName.Length < 2)
            throw new ArgumentException("First name must be at least 2 characters long", nameof(firstName));

        if (trimmedLastName.Length < 2)
            throw new ArgumentException("Last name must be at least 2 characters long", nameof(lastName));

        if (trimmedFirstName.Length > 50)
            throw new ArgumentException("First name cannot exceed 50 characters", nameof(firstName));

        if (trimmedLastName.Length > 50)
            throw new ArgumentException("Last name cannot exceed 50 characters", nameof(lastName));

        // Check for valid characters (letters, spaces, hyphens, apostrophes)
        if (!IsValidName(trimmedFirstName))
            throw new ArgumentException("First name contains invalid characters", nameof(firstName));

        if (!IsValidName(trimmedLastName))
            throw new ArgumentException("Last name contains invalid characters", nameof(lastName));

        return new FullName(trimmedFirstName, trimmedLastName);
    }

    /// <summary>
    /// Creates a FullName from a single string (assumes "FirstName LastName" format)
    /// </summary>
    /// <param name="fullName">Full name string</param>
    /// <returns>A new FullName instance</returns>
    public static FullName CreateFromString(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be null or empty", nameof(fullName));

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new ArgumentException("Full name must contain at least first and last name", nameof(fullName));

        var firstName = parts[0];
        var lastName = string.Join(" ", parts.Skip(1));

        return Create(firstName, lastName);
    }

    private static bool IsValidName(string name)
    {
        return name.All(c => char.IsLetter(c) || c == ' ' || c == '-' || c == '\'');
    }

    /// <summary>
    /// Implicit conversion to string
    /// </summary>
    public static implicit operator string(FullName fullName) => fullName.Value;

    /// <summary>
    /// Equality comparison
    /// </summary>
    public bool Equals(FullName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return FirstName == other.FirstName && LastName == other.LastName;
    }

    /// <summary>
    /// Equality comparison
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as FullName);
    }

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
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
    public static bool operator ==(FullName? left, FullName? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(FullName? left, FullName? right)
    {
        return !Equals(left, right);
    }
}
