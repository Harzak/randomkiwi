namespace randomkiwi.Events;

/// <summary>
/// Provides data for an event that signals a change in loading state.
/// </summary>
public sealed class LoadingChangedEventArgs : EventArgs
{
    public bool IsLoading { get; }

    public LoadingChangedEventArgs(bool isLoading)
    {
        this.IsLoading = isLoading;
    }
}
