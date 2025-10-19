namespace randomkiwi.Navigation.ViewModels;

/// <summary>
/// Implementation of the navigation service that manages view model navigation and maintains a navigation stack.
/// </summary>
public sealed class ViewModelNavigationService : IViewModelNavigationService
{
    private readonly INavigationHandler<IRoutableViewModel> _handler;

    /// <inheritdoc/>
    public IRoutableViewModel? CurrentViewModel => _handler.ActiveItem;

    /// <inheritdoc/>
    public bool CanNavigateBackViewModel => _handler.CanPop;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? CurrentViewModelChanged;

    public ViewModelNavigationService(INavigationHandler<IRoutableViewModel> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        _handler.ActiveItemChanged += OnActiveViewModelChanged;
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        NavigationContext context = new(parameters);
        await _handler.PushAsync(viewModel, context).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateBackViewModelAsync(NavigationParameters? parameters = null)
    {
        if (!CanNavigateBackViewModel)
        {
            return;
        }

        NavigationContext context = new(parameters);
        await _handler.PopAsync(context).ConfigureAwait(false);
    }

    private void OnActiveViewModelChanged(object? sender, EventArgs e)
    {
        CurrentViewModelChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        if (_handler != null)
        {
            _handler.ActiveItemChanged -= OnActiveViewModelChanged;
            _handler.Dispose();
        }
    }
}