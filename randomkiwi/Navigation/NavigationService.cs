namespace randomkiwi.Navigation;

/// <summary>
/// Unified navigation service that coordinates ViewModel and Web navigation
/// </summary>
internal sealed class NavigationService : INavigationService
{
    private readonly IViewModelNavigationService _viewModelNavigation;
    private readonly IWebPageNavigationService _webNavigation;

    public IRoutableViewModel? CurrentViewModel => _viewModelNavigation.CurrentViewModel;
    public IRoutableItem? CurrentPage => _webNavigation.CurrentPage;
    public bool CanNavigateBackViewModel => _viewModelNavigation.CanNavigateBackViewModel;
    public bool CanNavigateBackPage => _webNavigation.CanNavigateBackPage;

    public event EventHandler<EventArgs>? CurrentViewModelChanged;
    public event EventHandler<EventArgs>? CurrentPageChanged;

    public NavigationService(
        IViewModelNavigationService viewModelNavigation,
        IWebPageNavigationService webNavigation)
    {
        _viewModelNavigation = viewModelNavigation ?? throw new ArgumentNullException(nameof(viewModelNavigation));
        _webNavigation = webNavigation ?? throw new ArgumentNullException(nameof(webNavigation));

        _viewModelNavigation.CurrentViewModelChanged += OnCurrentViewModelChanged;
        _webNavigation.CurrentPageChanged += OnCurrentPageChanged;
    }

    public async Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null)
    {
        await _viewModelNavigation.NavigateToAsync(viewModel, parameters).ConfigureAwait(false);
    }

    public async Task NavigateToAsync(Uri url, NavigationParameters? parameters = null)
    {
        await _webNavigation.NavigateToAsync(url, parameters).ConfigureAwait(false);
    }


    /// <inheritdoc/>
    public async Task NavigateBackAsync(NavigationParameters? parameters = null)
    {
        if (CurrentViewModel is RandomArticleViewModel && CanNavigateBackPage)
        {
            await NavigateBackPageAsync(parameters).ConfigureAwait(false);
        }
        else if (CanNavigateBackViewModel)
        {
            await NavigateBackViewModelAsync(parameters).ConfigureAwait(false);
        }
    }

    public async Task NavigateBackViewModelAsync(NavigationParameters? parameters = null)
    {
        await _viewModelNavigation.NavigateBackViewModelAsync(parameters).ConfigureAwait(false);
    }

    public async Task NavigateBackPageAsync(NavigationParameters? parameters = null)
    {
        await _webNavigation.NavigateBackPageAsync(parameters).ConfigureAwait(false);
    }

    private void OnCurrentViewModelChanged(object? sender, EventArgs e)
    {
        CurrentViewModelChanged?.Invoke(this, e);
    }

    private void OnCurrentPageChanged(object? sender, EventArgs e)
    {
        CurrentPageChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        if (_viewModelNavigation != null)
        {
            _viewModelNavigation.CurrentViewModelChanged -= OnCurrentViewModelChanged;
            _viewModelNavigation.Dispose();
        }

        if (_webNavigation != null)
        {
            _webNavigation.CurrentPageChanged -= OnCurrentPageChanged;
            _webNavigation.Dispose();
        }
    }
}