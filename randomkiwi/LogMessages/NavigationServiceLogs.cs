using Microsoft.Extensions.Logging;

namespace randomkiwi.LogMessages;

internal static partial class NavigationServiceLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Failed to initialize view model {viewModel}")]
    public static partial void ViewModelInitializationFailed(ILogger logger, string viewModel, Exception ex);
}
