using randomkiwi.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Configuration;

/// <summary>
/// Provides application-wide configuration settings
/// </summary>
internal sealed class AppConfiguration : IAppConfiguration
{
    private CultureInfo _currentCulture;

    ///<inheritdoc/>    
    public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }

    ///<inheritdoc/>    
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (!_currentCulture.Equals(value))
            {
                if (!this.SupportedCultures.Contains(value))
                {
                    throw new InvalidOperationException("Unsupported culture");
                }
                _currentCulture = value;
                CultureInfo.DefaultThreadCurrentCulture = value;
            }
        }
    }

    /// <summary>
    /// Gets the two-letter ISO 639-1 language code for the current culture.
    /// </summary>
    public string LanguageCode => this.CurrentCulture.TwoLetterISOLanguageName;

    public Version AppVersion { get; }

    public AppConfiguration()
    {
        _currentCulture = CultureInfo.InvariantCulture;

        this.SupportedCultures = [new("en-US"), new("fr-FR")];
        this.AppVersion = Assembly.GetEntryAssembly()?.GetName()?.Version ?? new Version(0, 0, 0, 0);
    }

    public Task InitializeAsync()
    {
        this.CurrentCulture = this.GetDefaultCulture();
        return Task.CompletedTask;
    }

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }

    private CultureInfo GetDefaultCulture()
    {
        return this.SupportedCultures.First();
    }
}

