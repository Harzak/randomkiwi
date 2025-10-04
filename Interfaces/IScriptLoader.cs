namespace randomkiwi.Interfaces;

/// <summary>
/// Provides functionality to load and cache embedded JavaScript resources from the assembly.
/// </summary>
public interface IScriptLoader
{
    /// <summary>
    /// Loads and returns the content of a JavaScript resource embedded in the assembly.
    /// </summary>
    string Load(string scriptName);
}

