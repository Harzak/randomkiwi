using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace randomkiwi.Navigation;

/// <summary>
/// Represents a thread-safe stack for managing navigation states, with support for tracking and untracking view models.
/// </summary>
internal sealed class NavigationStack : IDisposable
{
    private readonly Lock _lock;

    private readonly ConcurrentStack<IRoutableViewModel> _stack;
    private readonly ConcurrentBag<IRoutableViewModel> _untracked;

    public ReadOnlyCollection<IRoutableViewModel> Items => _stack.ToList().AsReadOnly();
    public ReadOnlyCollection<IRoutableViewModel> Untracked => _untracked.ToList().AsReadOnly();

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
            foreach (IRoutableViewModel viewModel in _stack)
            {
                _untracked.Add(viewModel);
            }
            _stack.Clear();
        }
    }

    public void Push(IRoutableViewModel viewModel)
    {
        _stack.Push(viewModel);
    }

    public IRoutableViewModel? Pop()
    {
        lock (_lock)
        {
            if (_stack.TryPop(out IRoutableViewModel? viewModel))
            {
                _untracked.Add(viewModel);
                return viewModel;
            }
        }
        return null;
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
