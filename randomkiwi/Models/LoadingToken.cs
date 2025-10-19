namespace randomkiwi.Models;

/// <summary>
/// Represents a token that manages the lifetime of a loading operation.
/// </summary>
public sealed class LoadingToken : IDisposable
{
    private bool _disposed;
    private readonly LoadingService _service;

    public LoadingToken(LoadingService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Releases resources used by the current instance and signals the end of the loading process.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _service.EndLoading();
            _disposed = true;
        }
    }
}
