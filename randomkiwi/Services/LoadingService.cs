namespace randomkiwi.Services;

/// <summary>
/// Provides functionality to manage and track loading operations, including debouncing and ensuring a minimum display duration.
/// </summary>
public sealed class LoadingService : ILoadingService
{
    private readonly Lock _lock;
    private readonly System.Timers.Timer _debounceTimer;
    private DateTime? _loadingStartTime;
    private readonly int _debounceMilliseconds;
    private readonly int _minimumDisplayMilliseconds;
    private int _activeOperations;

    public bool IsLoading => _activeOperations > 0;

    public event EventHandler<LoadingChangedEventArgs>? IsLoadingChanged;

    public LoadingService(int debounceMilliseconds, int minimumDisplayMilliseconds)
    {
        _lock = new();
        _debounceMilliseconds = debounceMilliseconds > 0
            ? debounceMilliseconds
            : throw new ArgumentOutOfRangeException(nameof(debounceMilliseconds), "Must be non-negative");
        _minimumDisplayMilliseconds = minimumDisplayMilliseconds > 0
            ? minimumDisplayMilliseconds
            : throw new ArgumentOutOfRangeException(nameof(minimumDisplayMilliseconds), "Must be non-negative");

        _debounceTimer = new System.Timers.Timer(_debounceMilliseconds)
        {
            AutoReset = false,
            Enabled = false
        };
        _debounceTimer.Elapsed += OnDebounceTimerElapsed;
    }

    /// <inheritdoc />
    public IDisposable BeginLoading(string? context = null)
    {
        lock (_lock)
        {
            _activeOperations++;
            _debounceTimer.Stop();
            _loadingStartTime = DateTime.UtcNow;
            this.InvokeIsLoadingChanged(isLoading: true);
        }

        return new LoadingToken(this);
    }

    /// <summary>
    /// Signals the completion of a loading operation and updates the loading state accordingly.
    /// </summary>
    public void EndLoading()
    {
        DateTime? loadingStartTime;

        lock (_lock)
        {
            _activeOperations--;
            if (_activeOperations != 0)
            {
                return;
            }

            loadingStartTime = _loadingStartTime;
        }

        _ = this.CompleteLoadingAsync(loadingStartTime);
    }

    private async Task CompleteLoadingAsync(DateTime? loadingStartTime)
    {
        await this.WaitMinimumDurationAsync(loadingStartTime).ConfigureAwait(false);
        this.StartDebounceTimer();
    }

    /// <summary>
    /// Ensures that a minimum display duration is met by delaying execution if necessary.
    /// </summary>
    private async Task WaitMinimumDurationAsync(DateTime? startTime)
    {
        if (startTime.HasValue)
        {
            double elapsed = (DateTime.UtcNow - startTime.Value).TotalMilliseconds;
            double remaining = _minimumDisplayMilliseconds - elapsed;
            if (remaining > 0)
            {
                await Task.Delay((int)remaining).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Starts the debounce timer that will fire the loading end event after the specified interval.
    /// </summary>
    private void StartDebounceTimer()
    {
        lock (_lock)
        {
            if (_activeOperations > 0)
            {
                return;
            }

            _debounceTimer.Start();
        }
    }

    /// <summary>
    /// Handles the debounce timer elapsed event to finalize loading state.
    /// </summary>
    private void OnDebounceTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        lock (_lock)
        {
            if (_activeOperations == 0)
            {
                this.InvokeIsLoadingChanged(isLoading: false);
            }
        }
    }

    private void InvokeIsLoadingChanged(bool isLoading)
    {
        LoadingChangedEventArgs args = new(isLoading);
        this.IsLoadingChanged?.Invoke(this, args);
    }

    private bool _disposed;
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    _debounceTimer?.Stop();
                    _debounceTimer?.Dispose();
                }
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

