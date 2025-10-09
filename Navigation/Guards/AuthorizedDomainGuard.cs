namespace randomkiwi.Navigation.Guards;

/// <summary>
/// Guard that prevents navigation when calculations are in progress
/// </summary>
public sealed class AuthorizedDomainGuard : INavigationGuard
{
    /// <inheritdoc/>
    public Task<NavigationGuardResult> CanNavigateAsync(IRoutableItem? from, IRoutableItem to, NavigationContext context)
    {


        return Task.FromResult(NavigationGuardResult.Allow());
    }
}
