using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.LogMessages;

internal static partial class HttpClientLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Unable to parse: {input} to: {output}")]
    public static partial void UnableToParse(ILogger logger, string input, Type output, System.Exception ex);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Request failed due to invalid request URI: {uri}")]
    public static partial void InvalidRequest(ILogger logger, string uri, System.Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Request failed due to network issue. Uri: {uri}")]
    public static partial void NetworkIssue(ILogger logger, string uri, System.Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Request was canceled. Uri: {uri}")]
    public static partial void RequestCanceled(ILogger logger, string uri, System.Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "date: {date} | requestUri: {requestUri} | httpMethod: {httpMethod} | httpResponse: {httpResponse} | userAgent: {userAgent} | jobID: {jobID} | requestDate {requestDate} | token: {token}")]
    public static partial void HttpRequestLogHandler(ILogger logger, DateTime date, string requestUri, string httpMethod, HttpStatusCode httpResponse, string userAgent, string jobID, DateTimeOffset? requestDate, string token);

}
