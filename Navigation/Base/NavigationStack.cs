using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace randomkiwi.Navigation.Base;



/// <summary>
/// Represents a thread-safe stack for managing navigation states, with support for tracking and untracking view models.
/// </summary>
internal sealed class NavigationStack<T> : IDisposable where T : IRoutableItem
{
    private readonly Lock _lock;

    private readonly ConcurrentStack<T> _stack;
    private readonly ConcurrentBag<T> _untracked;

    public ReadOnlyCollection<T> Items => _stack.ToList().AsReadOnly();
    public ReadOnlyCollection<T> Untracked => _untracked.ToList().AsReadOnly();

    public NavigationStack()
    {
        _lock = new();
        _stack = [];
        _untracked = [];
    }

    public void Clear()
    {
        lock (_lock)
        {
            foreach (T item in _stack)
            {
                _untracked.Add(item);
            }
            _stack.Clear();
        }
    }

    public void Push(T item)
    {
        _stack.Push(item);
    }

    public T? Pop()
    {
        lock (_lock)
        {
            if (_stack.TryPop(out T? item))
            {
                _untracked.Add(item);
                return item;
            }
        }
        return default;
    }

    public void ClearUntrack()
    {
        _untracked.Clear();
    }

    public void Dispose()
    {
        _stack?.Clear();
        _untracked?.Clear();
    }
}
