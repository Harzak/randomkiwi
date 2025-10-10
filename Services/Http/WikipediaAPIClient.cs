using Microsoft.Extensions.Logging;
using randomkiwi.Utilities.Results;
using System.Globalization;
using System.Text.Json;

namespace randomkiwi.Services.Http;

/// <summary>
/// Provides functionality to interact with the Wikipedia API.
/// </summary>
internal sealed class WikipediaAPIClient : HttpService, IWikipediaAPIClient
{
    public WikipediaAPIClient(
        IHttpClientFactory httpClientFactory,
        IHttpClientOptionFactory httpClientOptionFactory,
        ILogger<WikipediaAPIClient> logger)
    : base(httpClientFactory, DateTimeFacade.Default, httpClientOptionFactory, logger)
    {

    }

    /// <inheritdoc />
    public async Task<OperationResultList<PageDto>> GetRandomPagesAsync(
        int limit = 20,
        string grnamespace = "0",
        CancellationToken cancellationToken = default)
    {
        OperationResultList<PageDto> result = new();

        string endpoint = String.Format(CultureInfo.InvariantCulture, WikipediaEndpoint.ENDPOINT_FORMAT_QUERY_PAGEPROPS, grnamespace, limit);
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
