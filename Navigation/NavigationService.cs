namespace randomkiwi.Navigation;

/// <summary>
/// Implementation of the navigation service that manages view model navigation and maintains a navigation stack.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly INavigationHandler _handler;
    private readonly IServiceProvider _viewModelProvider;

    /// <inheritdoc/>
    public IRoutableViewModel? CurrentViewModel => _handler.ActiveViewModel;

    /// <inheritdoc/>
    public bool CanNavigateBack => _handler.CanPop;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? CurrentViewModelChanging;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? CurrentViewModelChanged;

    public NavigationService(INavigationHandler handler, IServiceProvider viewModelProvider)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _viewModelProvider = viewModelProvider ?? throw new ArgumentNullException(nameof(viewModelProvider));

        _handler.ActiveViewModelChanging += OnActiveViewModelChanging;
        _handler.ActiveViewModelChanged += OnActiveViewModelChanged;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync(IHostViewModel host)
    {
        ArgumentNullException.ThrowIfNull(host);
        await _handler.InitializeAsync(host).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateToHomeAsync(NavigationParameters? parameters = null)
    {
        IRoutableViewModel homeViewModel = _viewModelProvider.GetRequiredService<RandomWikipediaViewModel>();
        await _handler.ClearAsync().ConfigureAwait(false);
        await NavigateToAsync(homeViewModel).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        NavigationContext context = new(parameters);
        await _handler.PushAsync(viewModel, context).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateBackAsync(NavigationParameters? parameters = null)
    {
        if (!CanNavigateBack)
        {
            return;
        }

        NavigationContext context = new(parameters);
        await _handler.PopAsync(context).ConfigureAwait(false);
    }

    private void OnActiveViewModelChanging(object? sender, EventArgs e)
    {
        this.CurrentViewModelChanging?.Invoke(this, e);
    }

    private void OnActiveViewModelChanged(object? sender, EventArgs e)
    {
        this.CurrentViewModelChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        if (_handler != null)
        {
            _handler.ActiveViewModelChanging -= OnActiveViewModelChanging;
            _handler.ActiveViewModelChanged -= OnActiveViewModelChanged;
            _handler.Dispose();
        }
    }
}