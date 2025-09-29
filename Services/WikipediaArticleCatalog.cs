using randomkiwi.Dto;
using randomkiwi.Interfaces;
using randomkiwi.Models;
using randomkiwi.Services.Http;
using randomkiwi.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Services;

/// <summary>
/// Represents a catalog of Wikipedia articles that supports navigation through a sequence of articles and maintains a history of visited articles.
/// </summary>
internal sealed class WikipediaArticleCatalog : IArticleCatalog
{
    private const int POOL_TRESHOLD = 30;
    private const int CATALOG_TRESHOLD = 100;

    private readonly IWikipediaAPIClient _apiClient;
    private readonly IWikipediaUrlBuilder _urlBuilder;

    private readonly Queue<WikipediaArticleMetadata> _pool;
    private readonly List<WikipediaArticleMetadata> _catalog;
    private int _currentIndex;
    private Lock _lock;

    public WikipediaArticleMetadata? Current => _catalog.Count > _currentIndex ? _catalog[_currentIndex] : null;

    public WikipediaArticleCatalog(IWikipediaAPIClient apiClient, IWikipediaUrlBuilder urlBuilder)
    {
        _apiClient = apiClient;
        _urlBuilder = urlBuilder;
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
            WikipediaArticleMetadata? article = this.DequeueFromPool();
            if (article is null)
            {
                return OperationResult.Failure("No articles available in the pool.");
            }

            this.AddToCatalog(article);
            return OperationResult.Success();
        }
        return OperationResult.Failure("Failed to initialize article catalog.");
    }

    /// <inheritdoc />
    public OperationResult Previous()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            return OperationResult.Success();
        }
        return OperationResult.Failure("No previous article in the catalog.");
    }

    /// <inheritdoc />
    public async Task<OperationResult> NextAsync()
    {
        OperationResult result = new();

        WikipediaArticleMetadata? article = this.DequeueFromPool();
        if (article is null)
        {
            return result.WithError("No more articles in the pool.");
        }

        this.AddToCatalog(article);
        OperationResult feedResult = await this.FeedPoolAsync().ConfigureAwait(false);
        if (!feedResult.IsSuccess)
        {
            return result.WithError("Failed to replenish the article pool.");
        }

        this.CleanupCatalog();

        return result.WithSuccess();
    }

    /// <inheritdoc />
    public Task<OperationResult> BookmarkAsync()
    {
        throw new NotImplementedException();
    }

    private void AddToCatalog(WikipediaArticleMetadata article)
    {
        lock (_lock)
        {
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

    private async Task<OperationResult> FeedPoolAsync()
    {
        OperationResult result = new();

        if (_pool.Count > 0)
        {
            return result.WithSuccess();
        }

        OperationResultList<PageDto> apiResult = await _apiClient.GetRandomPagesAsync(POOL_TRESHOLD).ConfigureAwait(false);

        if (apiResult.IsSuccess && apiResult.HasContent)
        {
            lock (_lock)
            {
                foreach (PageDto page in apiResult.Content)
                {
                    _pool.Enqueue(new WikipediaArticleMetadata
                    {
                        Id = page.PageId,
                        Namespace = page.Namespace,
                        Title = page.Title,
                        Description = string.Empty,
                        Url = _urlBuilder.BuildArticleUrl(page.Title)
                    });
                }
            }
            return result.WithSuccess();
        }
        return apiResult;
    }

    private void CleanupCatalog()
    {
        lock (_lock)
        {
            if (_catalog.Count > CATALOG_TRESHOLD)
            {
                int itemsToRemove = _catalog.Count - POOL_TRESHOLD;
                _catalog.RemoveRange(0, itemsToRemove);
                _currentIndex = Math.Max(0, _currentIndex - itemsToRemove);
            }
        }
    }

    public void Dispose()
    {
        _pool?.Clear();
        _catalog?.Clear();
    }
}
