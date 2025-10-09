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
    private readonly NavigationStack<IRoutableItem> _stack;

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
    public Task ClearAsync()
    {
        _stack.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task PushAsync(IRoutableItem page, NavigationContext context)
    {
        foreach (INavigationGuard guard in _navigationGuards)
        {
            NavigationGuardResult result = await guard.CanNavigateAsync(ActiveItem, page, context).ConfigureAwait(false);
            if (!result.CanNavigate)
            {
                return;
            }
        }

        _stack.Push(page);
        ActiveItemChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public Task PopAsync(NavigationContext context)
    {
        if (!CanPop)
        {
            return Task.CompletedTask;
        }

        _stack.Pop();
        ActiveItemChanged?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _stack?.Dispose();
    }
}
