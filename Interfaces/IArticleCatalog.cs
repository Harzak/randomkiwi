using randomkiwi.Utilities.Results;

namespace randomkiwi.Interfaces;

/// <summary>
/// Represents a catalog of articles that allows navigation, bookmarking
/// </summary>
public interface IArticleCatalog : IDisposable
{
    /// <summary>
    /// Gets the metadata of the current Wikipedia article being processed.
    /// </summary>
    WikipediaArticleMetadata? Current { get; }

    /// <summary>
    /// Asynchronously initializes the system and prepares it for operation.
    /// </summary>
    Task<OperationResult> InitializeAsync();

    /// <summary>
    /// Refreshes the internal state by clearing existing data and reinitializing resources.
    /// </summary>
    Task<OperationResult> RefreshAsync();

    /// <summary>
    /// Retrieves the previous item in the sequence asynchronously.
    /// </summary>
    OperationResult Previous();

    /// <summary>
    /// Retrieves the next item in the sequence asynchronously.
    /// </summary>
    Task<OperationResult> NextAsync();
}
