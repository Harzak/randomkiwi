namespace randomkiwi.Interfaces;

/// <summary>
/// Defines the contract for HTTP service operations providing asynchronous GET and POST functionality.
/// </summary>
public interface IHttpService
{
    void Initialize();

    /// <summary>
    /// Asynchronously performs an HTTP GET request to the specified endpoint.
    /// </summary>
    Task<OperationResult<string>> GetAsync(string endpoint, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously performs an HTTP POST request to the specified endpoint with the provided content.
    /// </summary>
    Task<OperationResult<string>> PostAsync(string endpoint, HttpContent content, CancellationToken cancellationToken);
}
