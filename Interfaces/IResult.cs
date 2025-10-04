namespace randomkiwi.Interfaces;

/// <summary>
/// Defines the contract for operation results that indicate success or failure status.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation completed successfully.
    /// </summary>
    bool IsSuccess { get; set; }
}
