namespace randomkiwi.Interfaces;

/// <summary>
/// Defines a contract for handling navigation between view models.
/// </summary>
public interface INavigationHandler : IDisposable
{
    /// <summary>
    /// Gets the currently active view model in the navigation system.
    /// </summary>
    IRoutableViewModel? ActiveViewModel { get; }

    /// <summary>
    /// Gets a value indicating whether there are items available to be removed from the stack.
    /// </summary>
    public bool CanPop { get; }

    /// <summary>
    /// Asynchronously clears all items from the navigation stack.
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Pushes the specified view model onto the navigation stack.
    /// </summary>
    Task PushAsync(IRoutableViewModel viewModel, NavigationContext context);

    /// <summary>
    /// Removes the top page from the navigation stack.
    /// </summary>
    Task PopAsync(NavigationContext context);

    event EventHandler<EventArgs>? ActiveViewModelChanging;

    event EventHandler<EventArgs>? ActiveViewModelChanged;
}