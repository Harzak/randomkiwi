using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace randomkiwi.Utilities;

/// <summary>
/// Provides functionality to load and cache embedded JavaScript resources from the assembly.
/// </summary>
internal sealed class ScriptLoader : IScriptLoader
{
    private readonly IAppSettingsProvider _settingsProvider;
    private readonly Dictionary<string, string> _cache;

    public ScriptLoader(IAppSettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        _cache = [];
    }

    /// <inheritdoc/>
    public string Load(string scriptName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = String.Format(CultureInfo.InvariantCulture,
            "{0}.{1}.{2}.js",
            assembly.GetName().Name,
            _settingsProvider.App.ScriptNamespace,
            scriptName);

        if (_cache.TryGetValue(resourceName, out var cached))
        {
            return cached;
        }

        using Stream? stream = assembly.GetManifestResourceStream(resourceName)??throw new FileNotFoundException($"Script not found: {scriptName}");
        using StreamReader reader = new(stream);
        string script = reader.ReadToEnd();
        string scriptMinified = MinifyScript(script);

        _cache[resourceName] = scriptMinified;
        return scriptMinified;
    }

    private string MinifyScript(string script)
    {
        return Regex.Replace(script, @"\s+", " ").Trim();
    }
}