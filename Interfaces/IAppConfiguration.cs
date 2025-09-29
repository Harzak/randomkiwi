using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;


/// <summary>
/// Represents the configuration settings for the app.
/// </summary>
public interface IAppConfiguration
{
    /// <summary>
    /// Gets the collection of cultures supported by the application.
    /// </summary>
    IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
    /// <summary>
    /// Gets or sets the culture information used for formatting and parsing operations.
    /// </summary>
    CultureInfo CurrentCulture { get; set; }
    string LanguageCode { get; }
    Version AppVersion { get; }

    Task InitializeAsync();
    Task SaveAsync();
}