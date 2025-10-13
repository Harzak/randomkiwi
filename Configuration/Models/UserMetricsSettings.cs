namespace randomkiwi.Configuration.Models;

/// <summary>
/// Configuration settings for user metrics tracking.
/// </summary>
public class UserMetricsSettings
{
    public const string SectionName = "UserMetrics";

    /// <summary>
    /// Gets or sets the maximum number of recent navigations to track.
    /// </summary>
    public int MaxRecentNavigations { get; set; } = 20;

    /// <summary>
    /// Gets or sets the base size for the article pool.
    /// </summary>
    public int BaseSizePool { get; set; } = 20;
}