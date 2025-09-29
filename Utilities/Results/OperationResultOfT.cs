using randomkiwi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Utilities.Results;


/// <summary>
/// Represents the result of an operation that returns strongly-typed content along with success/failure status.
/// </summary>
[Serializable]
public class OperationResult<T> : OperationResult, IResult<T>
{
    /// <summary>
    /// Gets or sets the strongly-typed content of the operation result.
    /// </summary>
    public T Content { get; set; }

    /// <summary>
    /// Gets a value indicating whether the result contains valid content.
    /// </summary>
    public virtual bool HasContent
    {
        get => !EqualityComparer<T>.Default.Equals(Content, default);
    }

    public OperationResult() : base()
    {
        Content = default!;
    }

    public OperationResult(T content) : base()
    {
        Content = content;
    }

    public OperationResult(T content, bool result) : base(result)
    {
        Content = content;
    }

    /// <summary>
    /// Sets the operation result to successful status.
    /// </summary>
    public new OperationResult<T> WithSuccess()
    {
        base.WithSuccess();
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status.
    /// </summary>
    public new OperationResult<T> WithFailure()
    {
        base.WithFailure();
        return this;
    }

    /// <summary>
    /// Sets the operation result to failed status with an error message.
    /// </summary>
    public new OperationResult<T> WithError(string message)
    {
        base.WithError(message);
        return this;
    }

    /// <summary>
    /// Sets the content value for this operation result.
    /// </summary>
    public OperationResult<T> WithValue(T value)
    {
        Content = value;
        return this;
    }

    /// <summary>
    /// Affects this result with the status and error information from another operation result.
    /// </summary>
    public OperationResult<T> Affect<TDifferent>(OperationResult<TDifferent> operationResult)
    {
        ArgumentNullException.ThrowIfNull(operationResult, nameof(operationResult));
        IsSuccess = operationResult.IsSuccess;
        ErrorMessage = operationResult.ErrorMessage;
        ErrorCode = operationResult.ErrorCode;
        return this;

    }

    /// <summary>
    /// Converts this generic operation result to a non-generic operation result.
    /// </summary>
    public OperationResult ToOperationResult()
    {
        return new OperationResult(IsSuccess)
        {
            ErrorMessage = ErrorMessage,
            ErrorCode = ErrorCode
        };
    }
}

