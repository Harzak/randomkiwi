namespace randomkiwi.Utilities.Results;


/// <summary>
/// Represents the result of an operation that returns a list of strongly-typed items with enhanced content validation.
/// </summary>
public class OperationResultList<T> : OperationResult<IList<T>>
{
    /// <summary>
    /// Gets a value indicating whether the result contains valid content and the list is not empty.
    /// </summary>
    public override bool HasContent => base.HasContent && base.Content.Any();

    public OperationResultList()
    {
        this.Content = [];
    }

    /// <summary>
    /// Sets the operation result to successful status.
    /// </summary>
    public new OperationResultList<T> WithSuccess()
    {
        base.WithSuccess();
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status.
    /// </summary>
    public new OperationResultList<T> WithFailure()
    {
        base.WithFailure();
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public new OperationResultList<T> WithError(string message)
    {
        base.WithError(message);
        return this;
    }

    /// <summary>
    /// Sets the list content value for this operation result.
    /// </summary>
    /// <param name="value">The list of items that represent the value.</param>
    public new OperationResultList<T> WithValue(IList<T> value)
    {
        this.Content = value;
        return this;
    }
}