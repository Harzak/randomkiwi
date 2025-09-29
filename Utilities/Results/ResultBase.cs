using randomkiwi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Utilities.Results;


/// <summary>
/// Provides an abstract base class for operation results with success/failure status and error handling.
/// </summary>
public abstract class ResultBase : IResult
{
    private bool _isSuccess;
    private bool _isFailed;

    /// <summary>
    /// Gets or sets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess
    {
        get => _isSuccess;
        set
        {
            _isSuccess = value;
            _isFailed = !value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailed
    {
        get => _isFailed;
        set
        {
            _isFailed = value;
            _isSuccess = !value;
        }
    }

    /// <summary>
    /// Gets or sets the error message when the operation fails.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error code when the operation fails.
    /// </summary>
    public string ErrorCode { get; set; }

    protected ResultBase(bool success)
    {
        IsSuccess = success;
        ErrorMessage = string.Empty;
        ErrorCode = string.Empty;
    }

    protected ResultBase() : this(false)
    {

    }

    /// <summary>
    /// Affects this result with the success status of another result.
    /// </summary>
    public IResult Affect(IResult result)
    {
        if (result != null && IsSuccess)
        {
            IsSuccess = result.IsSuccess;
        }
        return this;
    }

    /// <summary>
    /// Affects this result with the success status of a result produced by a function.
    /// </summary>
    public IResult Affect(Func<IResult> result)
    {
        if (result != null)
        {
            return Affect(result());
        }
        return this;
    }

    /// <summary>
    /// Asynchronously affects this result with the success status of a result produced by an async function.
    /// </summary>
    public async Task<IResult> Affect(Func<Task<IResult>> result)
    {
        if (result != null)
        {
            return Affect(await result().ConfigureAwait(false));
        }
        return this;
    }
}
