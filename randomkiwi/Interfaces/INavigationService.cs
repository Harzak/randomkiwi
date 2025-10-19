namespace randomkiwi.Interfaces;

public interface INavigationService : IWebPageNavigationService, IViewModelNavigationService
{
    /// <summary>
    /// Smart navigation that prioritizes web navigation when in web-enabled ViewModels
    /// </summary>
    Task NavigateBackAsync(NavigationParameters? parameters = null);
}

/// <summary>
/// Service interface for web navigation
/// </summary>
public interface IWebPageNavigationService : IDisposable
{
    /// <summary>
    /// Gets whether backward navigation is possible
    /// </summary>
    bool CanNavigateBackPage { get; }

    /// <summary>
    /// Gets the current web navigation item
    /// </summary>
    IRoutableItem? CurrentPage { get; }

    /// <summary>
    /// Occurs when the current page changes.
    /// </summary>
    event EventHandler<EventArgs>? CurrentPageChanged;

    /// <summary>
    /// Navigates to the specified URL
    /// </summary>
    Task NavigateToAsync(Uri url, NavigationParameters? parameters = null);

    /// <summary>
    /// Navigates back in web history
    /// </summary>
    Task NavigateBackPageAsync(NavigationParameters? parameters = null);
}

/// <summary>
/// Service interface for managing navigation between different views and view models in the application.
/// </summary>
public interface IViewModelNavigationService : IDisposable
{

    /// <summary>
    /// Gets a value indicating whether backward navigation is possible.
    /// </summary>
    bool CanNavigateBackViewModel { get; }

    /// <summary>
    /// Gets the currently active view model.
    /// </summary>
    /// <value>
    /// The current routable view model, or null if no view model is active.
    /// </value>
    IRoutableViewModel? CurrentViewModel { get; }

    /// <summary>
    /// Occurs when the current view model changes.
    /// </summary>
    event EventHandler<EventArgs>? CurrentViewModelChanged;

    /// <summary>
    /// Navigates to the specified view model.
    /// </summary>
    Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null);

    /// <summary>
    /// Navigates back to the previous view in the navigation stack.
    /// </summary>
    Task NavigateBackViewModelAsync(NavigationParameters? parameters = null);

}