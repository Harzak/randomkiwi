using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.LogMessages;

internal static partial class WikipediaWebViewLogs
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Debug, Message = "URL changing to null, allowing navigation")]
    public static partial void UrlChangingToNull(ILogger logger);

    [LoggerMessage(EventId = 101, Level = LogLevel.Debug, Message = "URL changing from {CurrentUrl} to {NewUrl}")]
    public static partial void UrlChanging(ILogger logger, string? currentUrl, string? newUrl);

    [LoggerMessage(EventId = 102, Level = LogLevel.Debug, Message = "Created cookie for {url}")]
    public static partial void CookieCreated(ILogger logger, Uri url);

    [LoggerMessage(EventId = 103, Level = LogLevel.Warning, Message = "Failed to create cookies for {Uri}")]
    public static partial void CookieCreationFailed(ILogger logger, Uri uri);

    [LoggerMessage(EventId = 104, Level = LogLevel.Debug, Message = "Navigation started to {Url}")]
    public static partial void NavigationStarted(ILogger logger, string? url);

    [LoggerMessage(EventId = 105, Level = LogLevel.Debug, Message = "Navigation completed to {Url}, Success: {IsSuccess}")]
    public static partial void NavigationCompleted(ILogger logger, string? url, bool isSuccess);

    [LoggerMessage(EventId = 106, Level = LogLevel.Debug, Message = "Link clicked: {ClickedUrl}")]
    public static partial void LinkClicked(ILogger logger, string? clickedUrl);
}
