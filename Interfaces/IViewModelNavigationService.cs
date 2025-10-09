namespace randomkiwi.Interfaces;

/// <summary>
/// Service interface for managing navigation between different views and view models in the application.
/// </summary>
public interface IViewModelNavigationService : IDisposable
{

    /// <summary>
    /// Gets a value indicating whether backward navigation is possible.
    /// </summary>
    bool CanNavigateBack { get; }

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
    /// Asynchronously initializes the navigation service with the specified host view model.
    /// </summary>
    Task InitializeAsync(IHostViewModel host);

    /// <summary>
    /// Navigates to the home page and clears the navigation stack.
    /// </summary>
    Task NavigateToHomeAsync(NavigationParameters? parameters = null);

    /// <summary>
    /// Navigates to the specified view model.
    /// </summary>
    Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null);

    /// <summary>
    /// Navigates back to the previous view in the navigation stack.
    /// </summary>
    Task NavigateBackAsync(NavigationParameters? parameters = null);

}