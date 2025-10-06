namespace randomkiwi.Navigation.Guards;

/// <summary>
/// Result of navigation guard evaluation
/// </summary>
public sealed class NavigationGuardResult
{
    public bool CanNavigate { get; init; }
    public string? Reason { get; init; }
    public string? RedirectTo { get; init; }

    public static NavigationGuardResult Allow()
    {
        return new()
        {
            CanNavigate = true
        };
    }

    public static NavigationGuardResult Deny(string reason)
    {
        return new()
        {
            CanNavigate = false,
            Reason = reason
        };
    }

    public static NavigationGuardResult Redirect(string redirectTo, string reason)
    {
        return new()
        {
            CanNavigate = false,
            Reason = reason,
            RedirectTo = redirectTo
        };
    }
}