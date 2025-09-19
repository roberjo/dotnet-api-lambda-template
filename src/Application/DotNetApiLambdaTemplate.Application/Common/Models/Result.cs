namespace DotNetApiLambdaTemplate.Application.Common.Models;

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="T">The type of data returned</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Gets the data if the operation was successful
    /// </summary>
    public T? Value { get; }

    private Result(bool isSuccess, string error, T? value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <param name="value">The value to return</param>
    /// <returns>A successful result</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, string.Empty, value);
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, error, default);
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    /// <param name="errors">The error messages</param>
    /// <returns>A failed result</returns>
    public static Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(false, string.Join("; ", errors), default);
    }

    /// <summary>
    /// Implicit conversion from value to successful result
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A successful result</returns>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    /// <summary>
    /// Implicit conversion from error string to failed result
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static implicit operator Result<T>(string error)
    {
        return Failure(error);
    }
}

/// <summary>
/// Represents the result of an operation without a return value
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    public string Error { get; }

    private Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <returns>A successful result</returns>
    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    /// <param name="errors">The error messages</param>
    /// <returns>A failed result</returns>
    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, string.Join("; ", errors));
    }

    /// <summary>
    /// Implicit conversion from error string to failed result
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static implicit operator Result(string error)
    {
        return Failure(error);
    }
}
