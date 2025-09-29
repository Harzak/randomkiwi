using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Provides functionality to construct Wikipedia URLs.
/// </summary>
internal interface IWikipediaUrlBuilder
{
    /// <summary>
    /// Constructs the base URI for accessing the Wikipedia API, formatted with the configured language code.
    /// </summary>
    Uri BuildBaseUri();

    /// <summary>
    /// Constructs an URL for an article based on the provided title.
    /// </summary>
    Uri BuildArticleUrl(string title);
}
