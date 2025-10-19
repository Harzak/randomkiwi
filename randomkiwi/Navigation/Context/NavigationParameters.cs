namespace randomkiwi.Navigation.Context;

/// <summary>
/// Container for navigation parameters that can be passed between views
/// </summary>
public sealed class NavigationParameters : Dictionary<string, object?>
{
    public NavigationParameters() : base() { }

    public NavigationParameters(IDictionary<string, object?> parameters) : base(parameters) { }

    public T? GetValue<T>(string key) where T : class
    {
        return TryGetValue(key, out object? value) ? value as T : null;
    }

    public T GetValueOrDefault<T>(string key, T defaultValue = default!) where T : struct
    {
        return TryGetValue(key, out object? value) && value is T typedValue ? typedValue : defaultValue;
    }
}
