namespace randomkiwi.Navigation.WebPage;

public sealed class WebPageNavigationService : IWebPageNavigationService
{
    private readonly INavigationHandler<IRoutableItem> _handler;

    /// <inheritdoc/>
    public IRoutableItem? CurrentPage => _handler.ActiveItem;

    /// <inheritdoc/>
    public bool CanNavigateBackPage => _handler.CanPop;


    /// <inheritdoc/>
    public event EventHandler<EventArgs>? CurrentPageChanged;

    public WebPageNavigationService(INavigationHandler<IRoutableItem> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _handler.ActiveItemChanged += OnActiveViewModelChanged;
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(Uri url, NavigationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(url);

        WebNavigationItem item = new(url);
        NavigationContext context = new(parameters);
        await _handler.PushAsync(item, context).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateBackPageAsync(NavigationParameters? parameters = null)
    {
        if (!CanNavigateBackPage)
        {
            return;
        }

        NavigationContext context = new(parameters);
        await _handler.PopAsync(context).ConfigureAwait(false);
    }

    private void OnActiveViewModelChanged(object? sender, EventArgs e)
    {
        CurrentPageChanged?.Invoke(this, e);
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
