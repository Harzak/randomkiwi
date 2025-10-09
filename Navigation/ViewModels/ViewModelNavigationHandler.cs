using Microsoft.Extensions.Logging;
using randomkiwi.Navigation.Base;

namespace randomkiwi.Navigation.ViewModels;

/// <summary>
/// Handles navigation between view models within the application, managing the navigation stack, enforcing navigation
/// guards, and ensuring proper cleanup of resources.
/// </summary>
internal sealed class ViewModelNavigationHandler : INavigationHandler<IRoutableViewModel>
{
    private readonly ILogger _logger;
    private readonly IEnumerable<INavigationGuard> _navigationGuards;
    private IHostViewModel? _host;
    private readonly Timer _cleanupTimer;
    private readonly NavigationStack<IRoutableViewModel> _stack;

    public event EventHandler<EventArgs>? ActiveItemChanging;
    public event EventHandler<EventArgs>? ActiveItemChanged;

    /// <inheritdoc/>
    public IRoutableViewModel? ActiveItem => _stack.Items.FirstOrDefault();

    /// <inheritdoc/>
    public bool CanPop => _stack.Items.Count > 1;

    public ViewModelNavigationHandler(ILogger<ViewModelNavigationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stack = new();
        _navigationGuards = [];
        _cleanupTimer = new Timer(callback: OnCleanupTimerElapsed,
                                  state: null,
                                  dueTime: (int)TimeSpan.FromMinutes(2).TotalMilliseconds,
                                  period: (int)TimeSpan.FromMinutes(2).TotalMilliseconds);
    }

    /// <inheritdoc/>
    public Task InitializeAsync(IHostViewModel host)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task ClearAsync()
    {
        _stack.Clear();
        await GracefullyCleanupAllAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task PushAsync(IRoutableViewModel viewModel, NavigationContext context)
    {
        if (_host == null)
        {
            throw new InvalidOperationException("NavigationHandler is not initialized. Call InitializeAsync with a valid IHostViewModel before using.");
        }

        _host.IsBusy = true;

        foreach (INavigationGuard guard in _navigationGuards)
        {
            NavigationGuardResult result = await guard.CanNavigateAsync(ActiveItem, viewModel, context).ConfigureAwait(false);
            if (!result.CanNavigate)
            {
                return;
            }
        }

        ActiveItemChanging?.Invoke(this, EventArgs.Empty);

        _stack.Push(viewModel);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        _host.IsBusy = false;
        ActiveItemChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task PopAsync(NavigationContext context)
    {
        IRoutableViewModel? previousViewModel = _stack.Pop();
        if (previousViewModel != null)
        {
            ActiveItemChanging?.Invoke(this, EventArgs.Empty);

            await GracefullyCleanupAsync(previousViewModel).ConfigureAwait(false);
            if (ActiveItem != null)
            {
                await ActiveItem.OnResumeAsync().ConfigureAwait(false);
            }

            ActiveItemChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private async void OnCleanupTimerElapsed(object? state)
    {
        await ForceCleanupAllAsync().ConfigureAwait(false);
    }

    private async Task GracefullyCleanupAllAsync()
    {
        foreach (IRoutableViewModel viewModel in _stack.Untracked)
        {
            await GracefullyCleanupAsync(viewModel).ConfigureAwait(false);
        }
    }

    private async Task GracefullyCleanupAsync(IRoutableViewModel viewModel)
    {
        try
        {
            await viewModel.OnDestroyAsync().ConfigureAwait(false);
            viewModel.Dispose();
            _stack.ClearUntrack();
            NavigationHandlerLogs.Destroyed(_logger, viewModel.Name);
        }
        catch (Exception ex)
        {
            NavigationHandlerLogs.DestroyFailed(_logger, viewModel.Name, ex);
        }
    }

    private async Task ForceCleanupAllAsync()
    {
        foreach (IRoutableViewModel viewModel in _stack.Untracked)
        {
            await ForceCleanupAsync(viewModel).ConfigureAwait(false);
        }
    }

    private async Task ForceCleanupAsync(IRoutableViewModel viewModel)
    {
        try
        {
            await viewModel.OnForceDestroyAsync().ConfigureAwait(false);
            viewModel.Dispose();
            _stack.ClearUntrack();
            NavigationHandlerLogs.ForceDestroyed(_logger, viewModel.Name);
        }
        catch (Exception ex)
        {
            NavigationHandlerLogs.ForceDestroyFailed(_logger, viewModel.Name, ex);
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _stack?.Dispose();
    }
}
