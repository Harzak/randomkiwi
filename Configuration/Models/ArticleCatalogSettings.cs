namespace randomkiwi.Configuration.Models;

/// <summary>
/// Configuration settings for the article catalog management.
/// </summary>
public class ArticleCatalogSettings
{
    public const string SectionName = "ArticleCatalog";

    /// <summary>
    /// Gets or sets the threshold for the article pool size.
    /// </summary>
    public int PoolThreshold { get; set; } = 20;

    /// <summary>
    /// Gets or sets the threshold for the article catalog size.
    /// </summary>
    public int CatalogThreshold { get; set; } = 20;
}