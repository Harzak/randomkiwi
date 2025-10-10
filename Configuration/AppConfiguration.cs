using randomkiwi.Utilities.Results;
using System.Globalization;
using System.Reflection;

namespace randomkiwi.Configuration;

/// <summary>
/// Provides application-wide configuration settings
/// </summary>
internal sealed class AppConfiguration : IAppConfiguration
{
    private readonly IUserPreferenceRepository _userPreferenceRepository;

    private CultureInfo _currentCulture;
    private AppTheme _currentTheme;

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
                CultureInfo.DefaultThreadCurrentUICulture = value;

                Resources.Languages.Culture = value;
            }
        }
    }

    /// <summary>
    /// Gets the two-letter ISO 639-1 language code for the current culture.
    /// </summary>
    public string LanguageCode => this.CurrentCulture.TwoLetterISOLanguageName;

    ///<inheritdoc/>    
    public IReadOnlyCollection<AppTheme> AvailableThemes { get; }

    ///<inheritdoc/>    
    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                if (!this.AvailableThemes.Contains(value))
                {
                    throw new InvalidOperationException("Unsupported theme");
                }
                _currentTheme = value;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current != null && Application.Current.UserAppTheme != value)
                    {
                        Application.Current.UserAppTheme = value;
                    }
                });
            }
        }
    }

    public Version AppVersion { get; }

    public AppConfiguration(IUserPreferenceRepository userPreferenceRepository)
    {
        _userPreferenceRepository = userPreferenceRepository ?? throw new ArgumentNullException(nameof(userPreferenceRepository));
        _currentCulture = CultureInfo.InvariantCulture;

        this.SupportedCultures = [new("en-US"), new("fr-FR")];
        this.AvailableThemes = [AppTheme.Light, AppTheme.Dark, AppTheme.Unspecified];
        this.AppVersion = Assembly.GetEntryAssembly()?.GetName()?.Version ?? new Version(0, 0, 0, 0);
    }

    public async Task InitializeAsync()
    {
        if (!await this.TryLoadSavedPreferencesAsync().ConfigureAwait(false))
        {
            this.LoadDefaultPreferences();
        }
    }

    private async Task<bool> TryLoadSavedPreferencesAsync()
    {
        OperationResult<UserPreferenceModel> preferencesResult = await _userPreferenceRepository.LoadAsync().ConfigureAwait(false);
        if (preferencesResult.IsSuccess && preferencesResult.HasContent)
        {
            this.CurrentCulture = this.SupportedCultures.FirstOrDefault(c => c.Name.Equals(preferencesResult.Content.AppLanguage, StringComparison.OrdinalIgnoreCase))
                                        ?? this.GetDefaultCulture();

            this.CurrentTheme = this.AvailableThemes.FirstOrDefault(x => x == preferencesResult.Content.Theme);

            return true;
        }
        return false;
    }

    private void LoadDefaultPreferences()
    {
        this.CurrentCulture = this.GetDefaultCulture();
        this.CurrentTheme = this.GetDefaultTheme();
    }

    public async Task SaveAsync()
    {
        UserPreferenceModel preferences = new()
        {
            AppLanguage = this.CurrentCulture.Name,
            Theme = this.CurrentTheme,
        };
        await _userPreferenceRepository.SaveAsync(preferences).ConfigureAwait(false);
    }

    private CultureInfo GetDefaultCulture()
    {
        return this.SupportedCultures.First();
    }

    private AppTheme GetDefaultTheme()
    {
        return Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
    }

    /// <inheritdoc/>
    public AppTheme GetEffectiveThemeCode()
    {
        if (this.CurrentTheme == AppTheme.Unspecified)
        {
            return Application.Current?.RequestedTheme ?? AppTheme.Light;
        }
        else
        {
            return this.CurrentTheme;
        }
    }
}

