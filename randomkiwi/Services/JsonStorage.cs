using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace randomkiwi.Services;


/// <summary>
/// Provides functionality for storing and retrieving data in JSON format within the application's local storage.
/// </summary>
public sealed class JsonStorage<T> : IJsonStorage<T>
{
    private readonly ILogger _logger;
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public string FilePath => _filePath;
    public bool Exists => File.Exists(_filePath);

    public JsonStorage(string directory, string fileName, ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _filePath = Path.Combine(directory, fileName);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc/>
    public async Task<OperationResult<T>> LoadAsync()
    {
        OperationResult<T> result = new();

        if (!File.Exists(_filePath))
        {
            JsonStorageLogs.FileNotFound(_logger, typeof(T).Name, _filePath);
            return result.WithFailure();
        }

        try
        {
            string json = await File.ReadAllTextAsync(_filePath).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(json))
            {
                JsonStorageLogs.EmptyFile(_logger, typeof(T).Name, _filePath);
                return result.WithFailure();
            }

            T? data = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            if (!EqualityComparer<T>.Default.Equals(data, default))
            {
                return result.WithValue(data!).WithSuccess();
            }
            else
            {
                return result.WithSuccess();
            }
        }
        catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
        {
            JsonStorageLogs.FailedLoading(_logger, typeof(T).Name, _filePath, ex);
            return result.WithError($"Failed to load data from {_filePath}: {ex.Message}");
        }
        catch (Exception ex)
        {
            JsonStorageLogs.FailedLoading(_logger, typeof(T).Name, _filePath, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown if the directory cannot be created.</exception>
    /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
    public async Task SaveAsync(T data)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        this.EnsureDirectoryExists();

        try
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json).ConfigureAwait(false);
            JsonStorageLogs.SuccessFileSave(_logger, typeof(T).Name);
        }
        catch (Exception ex) when (ex is NotSupportedException)
        {
            JsonStorageLogs.FailedFileSave(_logger, typeof(T).Name, ex);
        }
        catch (Exception ex)
        {
            JsonStorageLogs.FailedFileSave(_logger, typeof(T).Name, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public void Delete()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                File.Delete(_filePath);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is IOException || ex is UnauthorizedAccessException)
            {
                JsonStorageLogs.FailedFileDelete(_logger, typeof(T).Name, _filePath, ex);
            }
            catch (Exception ex)
            {
                JsonStorageLogs.FailedFileDelete(_logger, typeof(T).Name, _filePath, ex);
                throw;
            }
        }
    }

    private void EnsureDirectoryExists()
    {
        string? appDirectory = Path.GetDirectoryName(_filePath) ?? throw new InvalidOperationException("Invalid file path.");

        if (!Directory.Exists(appDirectory))
        {
            DirectoryInfo? appDirectoryInfo = Directory.CreateDirectory(appDirectory);
            if (!appDirectoryInfo.Exists)
            {
                JsonStorageLogs.DirectoryNotCreated(_logger, typeof(T).Name, appDirectory);
                throw new InvalidOperationException($"Failed to create directory: {appDirectory}");
            }
            JsonStorageLogs.DirectoryCreated(_logger, typeof(T).Name, appDirectory);
        }
    }
}