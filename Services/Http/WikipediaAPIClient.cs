using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace randomkiwi.Services.Http;

/// <summary>
/// Provides functionality to interact with the Wikipedia API.
/// </summary>
internal sealed class WikipediaAPIClient : HttpService, IWikipediaAPIClient
{
    private readonly IAppSettingsProvider _settingsProvider;

    public WikipediaAPIClient(
        IHttpClientFactory httpClientFactory,
        IHttpClientOptionFactory httpClientOptionFactory,
        IAppSettingsProvider settingsProvider,
        ILogger<WikipediaAPIClient> logger)
    : base(httpClientFactory, DateTimeFacade.Default, httpClientOptionFactory, settingsProvider, logger)
    {
        _settingsProvider = settingsProvider;
    }

    /// <inheritdoc />
    public async Task<OperationResultList<PageDto>> GetRandomPagesAsync(
        int limit = 20,
        string grnamespace = "0",
        CancellationToken cancellationToken = default)
    {
        OperationResultList<PageDto> result = new();

        string endpoint = String.Format(CultureInfo.InvariantCulture, _settingsProvider.Wikipedia.QueryEndpointFormat, grnamespace, limit);
        OperationResult<string> response = await base.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

        if (response != null && response.IsSuccess && !string.IsNullOrWhiteSpace(response.Content))
        {
            ResultDto? wikipediaResult;
            try
            {
                wikipediaResult = JsonSerializer.Deserialize<ResultDto>(response.Content);
            }
            catch (JsonException)
            {
                return result.WithError("Failed to deserialize the Wikipedia API response.");
            }
            catch (Exception)
            {
                throw;
            }

            if (wikipediaResult?.Query?.Pages?.Count > 0)
            {
                return result.WithSuccess().WithValue(wikipediaResult.Query.Pages.Values.ToList());
            }
        }
        return result.WithFailure();
    }
}
