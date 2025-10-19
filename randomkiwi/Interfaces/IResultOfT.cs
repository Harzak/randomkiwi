namespace randomkiwi.Interfaces;

/// <summary>
/// Defines the contract for operation results that contain strongly-typed content along with success status.
/// </summary>
public interface IResult<T> : IResult
{
    /// <summary>
    /// Gets the strongly-typed content of the operation result.
    /// </summary>
    T Content { get; }
}