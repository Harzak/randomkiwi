using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Service interface for web navigation
/// </summary>
public interface IWebPageNavigationService : IDisposable
{
    /// <summary>
    /// Gets whether backward navigation is possible
    /// </summary>
    bool CanNavigateBack { get; }

    /// <summary>
    /// Gets the current web navigation item
    /// </summary>
    IRoutableItem? CurrentPage { get; }

    /// <summary>
    /// Occurs when the current page is about to change.
    /// </summary>
    event EventHandler<EventArgs>? CurrentPageChanging;

    /// <summary>
    /// Occurs when the current page changes.
    /// </summary>
    event EventHandler<EventArgs>? CurrentPageChanged;

    /// <summary>
    /// Asynchronously initializes the navigation service with the specified host view model.
    /// </summary>
    Task InitializeAsync(IHostViewModel host);

    /// <summary>
    /// Navigates to the specified URL
    /// </summary>
    Task NavigateToAsync(Uri url, NavigationParameters? parameters = null);

    /// <summary>
    /// Navigates back in web history
    /// </summary>
    Task NavigateBackAsync(NavigationParameters? parameters = null);
}