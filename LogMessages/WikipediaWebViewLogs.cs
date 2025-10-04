using Microsoft.Extensions.Logging;

namespace randomkiwi.LogMessages;

internal static partial class WikipediaWebViewLogs
{
    [LoggerMessage(EventId = 104, Level = LogLevel.Debug, Message = "Navigation started to {Url}")]
    public static partial void NavigationStarted(ILogger logger, string? url);

    [LoggerMessage(EventId = 105, Level = LogLevel.Debug, Message = "Navigation completed to {Url}, Success: {IsSuccess}")]
    public static partial void NavigationCompleted(ILogger logger, string? url, bool isSuccess);
}
