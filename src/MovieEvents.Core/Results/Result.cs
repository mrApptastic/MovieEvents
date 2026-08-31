namespace MovieEvents.Core.Results;

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">A value indicating whether the operation succeeded.</param>
    /// <param name="error">The error message for failed operations.</param>
    protected Result(bool isSuccess, string error)
    {
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("A failure result must include an error message.", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = isSuccess ? string.Empty : error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message when the operation fails.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> instance.</returns>
    public static Result Success() => new(true, string.Empty);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error message to associate with the failure.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Failure(string error) => new(false, error);
}

/// <summary>
/// Represents the outcome of an operation that returns a value.
/// </summary>
/// <typeparam name="T">The value type returned by the operation.</typeparam>
public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value returned by the operation when it succeeds.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The value to associate with the successful result.</param>
    /// <returns>A successful <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Success(T value) => new(true, value, string.Empty);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error message to associate with the failure.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static new Result<T> Failure(string error) => new(false, default, error);
}
