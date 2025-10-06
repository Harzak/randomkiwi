using randomkiwi.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Services;

/// <summary>
/// Provides functionality to manage and track loading operations, including debouncing and ensuring a minimum display duration.
/// </summary>
public sealed class LoadingService : ILoadingService
{
    private readonly Lock _lock;
    private CancellationTokenSource? _debounceTokenSource;
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
    }

    /// <inheritdoc />
    public IDisposable BeginLoading(string? context = null)
    {
        lock (_lock)
        {
            _activeOperations++;

            _debounceTokenSource?.Cancel();
            _debounceTokenSource = null;

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

        _ = CompleteLoadingAsync(loadingStartTime);
    }

    private async Task CompleteLoadingAsync(DateTime? loadingStartTime)
    {
        await WaitMinimumDurationAsync(loadingStartTime).ConfigureAwait(false);
        await WaitDebounceAsync().ConfigureAwait(false);

        lock (_lock)
        {
            if (_activeOperations == 0)
            {
                InvokeIsLoadingChanged(isLoading: false);
            }
        }
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
    /// Waits for the specified debounce interval, canceling any previous pending wait if invoked again.
    /// </summary>
    private async Task WaitDebounceAsync()
    {
        if (_debounceTokenSource != null)
        {
            await _debounceTokenSource.CancelAsync().ConfigureAwait(false);
            _debounceTokenSource.Dispose();
        }

        _debounceTokenSource = new CancellationTokenSource();
        CancellationToken token = _debounceTokenSource.Token;

        try
        {
            await Task.Delay(_debounceMilliseconds, token).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            return;
        }
        catch (Exception)
        {
            throw;
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
                _debounceTokenSource?.Cancel();
                _debounceTokenSource?.Dispose();
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

