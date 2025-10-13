namespace randomkiwi.Interfaces;

/// <summary>
/// Provides functionality to manage and track loading states within an application.
/// </summary>
public interface ILoadingService : IDisposable
{
    event EventHandler<LoadingChangedEventArgs>? IsLoadingChanged;
    bool IsLoading { get; }

    /// <summary>
    /// Begins a loading operation and notifies listeners of the loading state change.
    /// </summary>
    /// <returns>A <see cref="IDisposable"/> token that, when disposed, ends the loading operation and notifies listeners of the loading state change.</returns>
    IDisposable BeginLoading(string? context = null);
}
