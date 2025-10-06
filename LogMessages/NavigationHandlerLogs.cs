using Microsoft.Extensions.Logging;

namespace randomkiwi.LogMessages;

internal static partial class NavigationHandlerLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Failed to destroy untracked view model {name}.")]
    public static partial void DestroyFailed(ILogger logger, string name, Exception ex);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Failed to force destroy untracked view model {name}.")]
    public static partial void ForceDestroyFailed(ILogger logger, string name, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Tracked view model: {name} successfully destroyed.")]
    public static partial void Destroyed(ILogger logger, string name);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Untracked view model: {name} successfully force-destroyed.")]
    public static partial void ForceDestroyed(ILogger logger, string name);
}

