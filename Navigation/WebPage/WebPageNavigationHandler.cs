using Microsoft.Extensions.Logging;
using randomkiwi.Navigation.Base;
using randomkiwi.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Navigation.WebPage;

internal sealed class WebPageNavigationHandler : INavigationHandler<IRoutableItem>
{
    private readonly ILogger _logger;
    private readonly IEnumerable<INavigationGuard> _navigationGuards;
    private IHostViewModel? _host;
    private readonly NavigationStack<IRoutableItem> _stack;

    public event EventHandler<EventArgs>? ActiveItemChanging;
    public event EventHandler<EventArgs>? ActiveItemChanged;

    /// <inheritdoc/>
    public IRoutableItem? ActiveItem => _stack.Items.FirstOrDefault();

    /// <inheritdoc/>
    public bool CanPop => _stack.Items.Count > 1;

    public WebPageNavigationHandler(ILogger<WebPageNavigationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stack = new();
        _navigationGuards = [new AuthorizedDomainGuard()];
    }

    /// <inheritdoc/>
    public Task InitializeAsync(IHostViewModel host)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ClearAsync()
    {
        _stack.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task PushAsync(IRoutableItem page, NavigationContext context)
    {
        if (_host == null)
        {
            throw new InvalidOperationException("NavigationHandler is not initialized. Call InitializeAsync with a valid IHostViewModel before using.");
        }

        _host.IsBusy = true;

        foreach (INavigationGuard guard in _navigationGuards)
        {
            NavigationGuardResult result = await guard.CanNavigateAsync(ActiveItem, page, context).ConfigureAwait(false);
            if (!result.CanNavigate)
            {
                return;
            }
        }

        ActiveItemChanging?.Invoke(this, EventArgs.Empty);

        _stack.Push(page);

        _host.IsBusy = false;
        ActiveItemChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public Task PopAsync(NavigationContext context)
    {
        IRoutableItem? previousPage = _stack.Pop();
        if (previousPage != null)
        {
            ActiveItemChanging?.Invoke(this, EventArgs.Empty);
            ActiveItemChanged?.Invoke(this, EventArgs.Empty);
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _stack?.Dispose();
    }
}
