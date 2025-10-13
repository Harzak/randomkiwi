namespace randomkiwi.Configuration.Models;

/// <summary>
/// Configuration settings for the application.
/// </summary>
public class AppSettings
{
    public const string SectionName = "App";

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filename for user preferences storage.
    /// </summary>
    public string UserPreferencesFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filename for bookmarks storage.
    /// </summary>
    public string BookmarksFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the namespace for scripts.
    /// </summary>
    public string ScriptNamespace { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filename for UI formatting scripts.
    /// </summary>
    public string ScriptUIFormattingFilename { get; set; } = string.Empty;
}