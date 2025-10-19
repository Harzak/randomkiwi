namespace randomkiwi.Utilities.Results;


/// <summary>
/// Represents the result of an operation with success/failure status and error handling capabilities.
/// </summary>
public class OperationResult : ResultBase
{
    public OperationResult() : base()
    {

    }
    public OperationResult(bool success) : base(success)
    {

    }

    /// <summary>
    /// Sets the operation result to successful status.
    /// </summary>
    /// <returns>The current <see cref="OperationResult"/> instance with the success status set.</returns>
    public OperationResult WithSuccess()
    {
        IsSuccess = true;
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status.
    /// </summary>
    /// <returns>The current <see cref="OperationResult"/> instance with the failure status set.</returns>
    public OperationResult WithFailure()
    {
        IsSuccess = false;
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>The current <see cref="OperationResult"/> instance with the failure status and error message set.</returns>
    public OperationResult WithError(string message)
    {
        ErrorMessage = message;
        return WithFailure();
    }

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <returns>A new <see cref="OperationResult"/> instance representing a successful operation.</returns>
    public static OperationResult Success()
    {
        return new OperationResult(true);
    }

    /// <summary>
    /// Creates a failed operation result.
    /// </summary>
    /// <returns>A new <see cref="OperationResult"/> instance representing a failed operation.</returns>
    public static OperationResult Failure()
    {
        return new OperationResult(false);
    }

    /// <summary>
    /// Creates a failed operation result with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="OperationResult"/> instance representing a failed operation with the specified error message.</returns>
    public static OperationResult Failure(string message)
    {
        return new OperationResult(false).WithError(message);
    }
}
