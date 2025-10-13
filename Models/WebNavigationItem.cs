namespace randomkiwi.Models;

public sealed class WebNavigationItem : IRoutableItem
{
    public string Name { get; set; }
    public string UrlPath => Url?.ToString() ?? string.Empty;
    public Uri Url { get; }
    public DateTime NavigatedAt { get; }

    public WebNavigationItem(Uri url)
    {
        this.Url = url ?? throw new ArgumentNullException(nameof(url));
        this.Name = url.ToString();
        this.NavigatedAt = DateTime.UtcNow;
    }
}
