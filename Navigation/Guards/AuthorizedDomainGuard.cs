namespace randomkiwi.Navigation.Guards;

/// <summary>
/// Guard that prevents navigation to unauthorized domains
/// </summary>
public sealed class AuthorizedDomainGuard : INavigationGuard
{
    private static readonly HashSet<string> AuthorizedDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "https://fr.m.wikipedia.org/wiki/",
        "https://en.m.wikipedia.org/wiki/"
    };

    /// <inheritdoc/>
    public Task<NavigationGuardResult> CanNavigateAsync(IRoutableItem? from, IRoutableItem to, NavigationContext context)
    {
        ArgumentNullException.ThrowIfNull(to);

        if (string.IsNullOrEmpty(to.UrlPath))
        {
            return Task.FromResult(NavigationGuardResult.Allow());
        }

        if (!Uri.TryCreate(to.UrlPath, UriKind.Absolute, out Uri? targetUri))
        {
            return Task.FromResult(NavigationGuardResult.Deny("Invalid URL format"));
        }

        string targetUrl = targetUri.ToString();
        bool isAuthorized = AuthorizedDomains.Any(domain => targetUrl.StartsWith(domain, StringComparison.OrdinalIgnoreCase));

        if (isAuthorized)
        {
            return Task.FromResult(NavigationGuardResult.Allow());
        }

        return Task.FromResult(NavigationGuardResult.Deny($"Navigation to '{targetUri.Host}' is not allowed. Only Wikipedia domains are authorized."));
    }
}
