using Microsoft.Extensions.Logging;

namespace randomkiwi.LogMessages;

internal static partial class JsonStorageLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "{type} json storage directory created: {directory}")]
    public static partial void DirectoryCreated(ILogger logger, string type, string directory);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "{type} json storage directory not created: {directory}")]
    public static partial void DirectoryNotCreated(ILogger logger, string type, string directory);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "{type} json storage saved")]
    public static partial void SuccessFileSave(ILogger logger, string type);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "{type} json storage save failed")]
    public static partial void FailedFileSave(ILogger logger, string type, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "{type} json storage not found at: {path}")]
    public static partial void FileNotFound(ILogger logger, string type, string path);

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, Message = "{type} json storage is empty at: {path}")]
    public static partial void EmptyFile(ILogger logger, string type, string path);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "{type} json storage load failed at: {path}")]
    public static partial void FailedLoading(ILogger logger, string type, string path, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "{type} json storage delete failed at: {path}")]
    public static partial void FailedFileDelete(ILogger logger, string type, string path, Exception ex);
}
