using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace randomkiwi.Utilities;

/// <summary>
/// Provides functionality to load and cache embedded JavaScript resources from the assembly.
/// </summary>
internal sealed class ScriptLoader : IScriptLoader
{
    private const string SCRIPT_NAMESPACE = "Scripts";

    private readonly Dictionary<string, string> _cache;

    public ScriptLoader()
    {
        _cache = [];
    }

    /// <inheritdoc/>
    public string Load(string scriptName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = String.Format(CultureInfo.InvariantCulture,
            "{0}.{1}.{2}.js",
            assembly.GetName().Name,
            SCRIPT_NAMESPACE,
            scriptName);

        if (_cache.TryGetValue(resourceName, out var cached))
        {
            return cached;
        }

        using Stream? stream = assembly.GetManifestResourceStream(resourceName)??throw new FileNotFoundException($"Script not found: {scriptName}");
        using StreamReader reader = new(stream);
        string script = reader.ReadToEnd();

        _cache[resourceName] = script;

        return MinifyScript(script);
    }

    private string MinifyScript(string script)
    {
        return Regex.Replace(script, @"\s+", " ").Trim();
    }
}