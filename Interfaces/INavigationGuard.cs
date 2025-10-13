namespace randomkiwi.Interfaces;

/// <summary>
/// Interface for navigation guards that control navigation flow
/// </summary>
public interface INavigationGuard
{
    /// <summary>
    /// Determines whether navigation from the specified source view model to the target view model is allowed.
    /// </summary>
    Task<NavigationGuardResult> CanNavigateAsync(IRoutableItem? from, IRoutableItem to, NavigationContext context);
}
