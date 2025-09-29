using randomkiwi.Dto;
using randomkiwi.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Provides functionality to interact with the Wikipedia API.
/// </summary>
internal interface IWikipediaAPIClient : IHttpService
{
    /// <summary>
    /// Retrieves a list of random Wikipedia pages based on the specified namespace and limit.
    /// </summary>
    /// <param name="limit">The maximum number of pages to retrieve. Must be a positive integer. The default value is 20.</param>
    /// <param name="grnamespace">The namespace to filter the pages by. The default value is "0", which represents the main namespace.</param>
    Task<OperationResultList<PageDto>> GetRandomPagesAsync(int limit = 20, string grnamespace = "0", CancellationToken cancellationToken = default);
}
