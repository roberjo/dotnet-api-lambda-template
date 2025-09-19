namespace DotNetApiLambdaTemplate.Domain.ValueObjects;

/// <summary>
/// Money value object representing currency amounts
/// </summary>
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// The amount in the smallest currency unit (e.g., cents for USD)
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// The currency code (e.g., "USD", "EUR")
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a new Money instance with validation
    /// </summary>
    /// <param name="amount">The amount</param>
    /// <param name="currency">The currency code</param>
    /// <returns>A new Money instance</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public static Money Create(decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter code", nameof(currency));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        // Round to 2 decimal places for currency
        var roundedAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

        return new Money(roundedAmount, currency.ToUpperInvariant());
    }

    /// <summary>
    /// Creates a Money instance from cents
    /// </summary>
    /// <param name="cents">Amount in cents</param>
    /// <param name="currency">The currency code</param>
    /// <returns>A new Money instance</returns>
    public static Money FromCents(long cents, string currency = "USD")
    {
        return Create(cents / 100m, currency);
    }

    /// <summary>
    /// Gets the amount in cents
    /// </summary>
    /// <returns>Amount in cents</returns>
    public long ToCents()
    {
        return (long)(Amount * 100);
    }

    /// <summary>
    /// Adds two Money amounts (must be same currency)
    /// </summary>
    /// <param name="other">Other Money amount</param>
    /// <returns>New Money instance with sum</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} and {other.Currency}");

        return Create(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts two Money amounts (must be same currency)
    /// </summary>
    /// <param name="other">Other Money amount</param>
    /// <returns>New Money instance with difference</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract {Currency} and {other.Currency}");

        return Create(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies Money by a factor
    /// </summary>
    /// <param name="factor">Multiplication factor</param>
    /// <returns>New Money instance</returns>
    public Money Multiply(decimal factor)
    {
        return Create(Amount * factor, Currency);
    }

    /// <summary>
    /// Divides Money by a factor
    /// </summary>
    /// <param name="factor">Division factor</param>
    /// <returns>New Money instance</returns>
    /// <exception cref="ArgumentException">Thrown when factor is zero</exception>
    public Money Divide(decimal factor)
    {
        if (factor == 0)
            throw new ArgumentException("Cannot divide by zero", nameof(factor));

        return Create(Amount / factor, Currency);
    }

    /// <summary>
    /// Checks if this Money is greater than another
    /// </summary>
    /// <param name="other">Other Money amount</param>
    /// <returns>True if greater</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot compare {Currency} and {other.Currency}");

        return Amount > other.Amount;
    }

    /// <summary>
    /// Checks if this Money is less than another
    /// </summary>
    /// <param name="other">Other Money amount</param>
    /// <returns>True if less</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
    public bool IsLessThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot compare {Currency} and {other.Currency}");

        return Amount < other.Amount;
    }

    /// <summary>
    /// Checks if this Money equals another
    /// </summary>
    /// <param name="other">Other Money amount</param>
    /// <returns>True if equal</returns>
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    /// <summary>
    /// Equality comparison
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Money);
    }

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    /// <summary>
    /// String representation
    /// </summary>
    public override string ToString()
    {
        return $"{Amount:C} {Currency}";
    }

    /// <summary>
    /// Addition operator
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    /// <summary>
    /// Subtraction operator
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    /// <summary>
    /// Multiplication operator
    /// </summary>
    public static Money operator *(Money money, decimal factor)
    {
        return money.Multiply(factor);
    }

    /// <summary>
    /// Division operator
    /// </summary>
    public static Money operator /(Money money, decimal factor)
    {
        return money.Divide(factor);
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    public static bool operator >(Money left, Money right)
    {
        return left.IsGreaterThan(right);
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    public static bool operator <(Money left, Money right)
    {
        return left.IsLessThan(right);
    }

    /// <summary>
    /// Greater than or equal operator
    /// </summary>
    public static bool operator >=(Money left, Money right)
    {
        return left.Equals(right) || left.IsGreaterThan(right);
    }

    /// <summary>
    /// Less than or equal operator
    /// </summary>
    public static bool operator <=(Money left, Money right)
    {
        return left.Equals(right) || left.IsLessThan(right);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Money? left, Money? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Money? left, Money? right)
    {
        return !Equals(left, right);
    }
}
