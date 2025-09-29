using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace randomkiwi.Utilities;

/// <summary>
/// Provides functionality to execute actions with a guaranteed minimum duration.
/// </summary>
internal sealed class DebounceAction : IDebounceAction
{
    private readonly int _minDurationMs;
    public DebounceAction(int minDurationMs)
    {
        _minDurationMs = minDurationMs > 0 ? minDurationMs : throw new ArgumentOutOfRangeException(nameof(minDurationMs), "Minimum duration must be greater than zero.");
    }

    /// <summary>
    /// Executes an async action with return value ensuring it runs for at least the minimum duration.
    /// For UI loading scenarios to avoid flash effects.
    /// </summary>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        Stopwatch sw = Stopwatch.StartNew();
        T result = await action().ConfigureAwait(false);
        sw.Stop();
        
        int remainingTime = _minDurationMs - (int)sw.ElapsedMilliseconds;
        if (remainingTime > 0)
        {
            await Task.Delay(remainingTime).ConfigureAwait(false);
        }
        
        return result;
    }

    /// <summary>
    /// Executes an async action with return value ensuring it runs for at least the minimum duration.
    /// For UI loading scenarios to avoid flash effects.
    /// </summary>
    public T Execute<T>(Func<T> action)
    {
        Stopwatch sw = Stopwatch.StartNew();
        T result = action();
        sw.Stop();

        int remainingTime = _minDurationMs - (int)sw.ElapsedMilliseconds;
        if (remainingTime > 0)
        {
            Task.Delay(remainingTime).Wait();
        }

        return result;
    }
}
