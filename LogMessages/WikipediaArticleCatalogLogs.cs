using Microsoft.Extensions.Logging;

namespace randomkiwi.LogMessages;

internal static partial class WikipediaArticleCatalogLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Article pool replenished. Current pool size: {poolSize}.")]
    public static partial void PoolReplenished(ILogger logger, int poolSize);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Failed to replenish the article pool: {errorMessage}")]
    public static partial void FailedReplenishPool(ILogger logger, string errorMessage);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Article pool is empty.")]
    public static partial void EmptyPool(ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Optimal pool size determined: {poolsize}.")]
    public static partial void OptimalPoolSize(ILogger logger, int poolsize);
}
