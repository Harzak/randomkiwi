using Microsoft.Extensions.Logging;

namespace randomkiwi.Services;

/// <summary>
/// Represents a catalog of Wikipedia articles that supports navigation through a sequence of articles and maintains a history of visited articles.
/// </summary>
internal sealed class WikipediaArticleCatalog : IArticleCatalog
{
    private readonly ILogger _logger;
    private readonly IWikipediaAPIClient _apiClient;
    private readonly IWikipediaUrlBuilder _urlBuilder;
    private readonly IUserMetricsService _userMetricsService;
    private readonly IAppConfiguration _appConfiguration;
    private readonly IAppSettingsProvider _settingsProvider;

    private readonly Queue<WikipediaArticleMetadata> _pool;
    private readonly List<WikipediaArticleMetadata> _catalog;
    private int _currentIndex;
    private readonly Lock _lock;

    public WikipediaArticleMetadata? Current => _catalog.Count > _currentIndex ? _catalog[_currentIndex] : null;

    public WikipediaArticleCatalog(
        IWikipediaAPIClient apiClient,
        IWikipediaUrlBuilder urlBuilder,
        IUserMetricsService userMetricsService,
        IAppConfiguration appConfiguration,
        IAppSettingsProvider settingsProvider,
        ILogger<WikipediaArticleCatalog> logger)
    {
        _apiClient = apiClient;
        _urlBuilder = urlBuilder;
        _userMetricsService = userMetricsService;
        _appConfiguration = appConfiguration;
        _settingsProvider = settingsProvider;
        _logger = logger;
        _lock = new Lock();
        _pool = [];
        _catalog = [];
    }

    /// <inheritdoc />
    public async Task<OperationResult> InitializeAsync()
    {
        OperationResult feedResult = await this.FeedPoolAsync().ConfigureAwait(false);
        if (feedResult.IsSuccess)
        {
            OperationResult nextResult = this.NextInternal();
            if (nextResult.IsFailed)
            {
                return nextResult;
            }
            return OperationResult.Success();
        }
        return OperationResult.Failure("Failed to initialize article catalog.");
    }

    /// <inheritdoc />
    public async Task<OperationResult> RefreshAsync()
    {
        lock (_lock)
        {
            _pool.Clear();
            _catalog.Clear();
            _currentIndex = 0;
        }
        return await this.InitializeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<OperationResult> NextAsync()
    {
        OperationResult result = this.NextInternal();
        if (result.IsFailed)
        {
            return result;
        }

        int optimalPoolSize = _userMetricsService.GetOptimalPoolSize();
        WikipediaArticleCatalogLogs.OptimalPoolSize(_logger, optimalPoolSize);

        OperationResult feedResult = await this.FeedPoolAsync(optimalPoolSize).ConfigureAwait(false);

        if (!feedResult.IsSuccess)
        {
            return result.WithError("Failed to replenish the article pool.");
        }

        return result.WithSuccess();
    }

    /// <inheritdoc />
    public OperationResult Previous()
    {
        if (_currentIndex > 0 && _catalog.Count >= _currentIndex -1)
        {
            _currentIndex--;

            _userMetricsService.TrackNavigation(ENavigationType.Previous, Current!.Id);
            return OperationResult.Success();
        }
        return OperationResult.Failure("No previous article in the catalog.");
    }

    private OperationResult NextInternal()
    {
        WikipediaArticleMetadata? article = this.DequeueFromPool();
        if (article is null)
        {
            WikipediaArticleCatalogLogs.EmptyPool(_logger);
            return OperationResult.Failure("No more articles in the pool.");
        }

        this.AddToCatalog(article);

        _userMetricsService.TrackNavigation(ENavigationType.Next, article.Id);
        return OperationResult.Success();
    }

    private void AddToCatalog(WikipediaArticleMetadata article)
    {
        lock (_lock)
        {
            if (_catalog.Count > _settingsProvider.ArticleCatalog.CatalogThreshold)
            {
                _catalog.RemoveAt(0);
            }
            _catalog.Add(article);
            _currentIndex = _catalog.Count - 1;
        }
    }

    private WikipediaArticleMetadata? DequeueFromPool()
    {
        lock (_lock)
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }
        }
        return null;
    }

    private async Task<OperationResult> FeedPoolAsync(int targetPoolSize = -1)
    {
        if (targetPoolSize == -1)
        {
            targetPoolSize = _settingsProvider.ArticleCatalog.PoolThreshold;
        }

        OperationResult result = new();
        int added = 0;

        if (_pool.Count >= targetPoolSize / 2)
        {
            return result.WithSuccess();
        }

        int optimalFetchSize = this.GetOptimalFetchSize(targetPoolSize);
        OperationResultList<PageDto> apiResult = await _apiClient.GetRandomPagesAsync(optimalFetchSize).ConfigureAwait(false);

        if (apiResult.IsSuccess && apiResult.HasContent)
        {
            lock (_lock)
            {
                foreach (PageDto page in apiResult.Content.Where(ApplyFilter))
                {
                    _pool.Enqueue(new WikipediaArticleMetadata
                    {
                        Id = page.PageId,
                        Namespace = page.Namespace,
                        Title = page.Title,
                        Description = string.Empty,
                        Url = _urlBuilder.BuildArticleUrl(page.Title)
                    });
                    added++;
                }
            }

            if (_pool.Count < targetPoolSize)
            {
                await this.FeedPoolAsync(targetPoolSize - added).ConfigureAwait(false);
            }

            WikipediaArticleCatalogLogs.PoolReplenished(_logger, _pool.Count);
            return result.WithSuccess();
        }

        WikipediaArticleCatalogLogs.FailedReplenishPool(_logger, apiResult.ErrorMessage);
        return apiResult;
    }

    private bool ApplyFilter(PageDto page)
    {
        return page.Length > this.GetDesiredArticleLength() && (page.PageProps == null || page.PageProps.Disambiguation == null);
    }

    private int GetOptimalFetchSize(int poolSize)
    {
        return _appConfiguration.ArticleDetail switch
        {
            EArticleDetail.Any => poolSize * 2,
            EArticleDetail.Medium => poolSize * 10,
            EArticleDetail.Detailed => poolSize * 30,
            _ => 0,
        };
    }

    private int GetDesiredArticleLength()
    {
        return _appConfiguration.ArticleDetail switch
        {
            EArticleDetail.Any => 1000,
            EArticleDetail.Medium => 10000,
            EArticleDetail.Detailed => 30000,
            _ => 0,
        };
    }

    private bool _disposed;
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _pool?.Clear();
                _catalog?.Clear();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
