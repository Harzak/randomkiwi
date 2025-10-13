using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Globalization;

namespace randomkiwi.ViewModels;

public sealed partial class SettingsViewModel : BaseRoutableViewModel
{
    public IAppConfiguration AppConfig { get; }

    /// <inheritdoc/>
    public override string Name => nameof(SettingsViewModel);

    /// <inheritdoc/>
    public override bool CanBeConfigured => false;

    [ObservableProperty]
    private CultureInfo _selectedCulture;

    [ObservableProperty]
    private AppTheme _selectedTheme;

    public SettingsViewModel(IAppConfiguration appConfig, INavigationService navigationService) : base(navigationService)
    {
        this.AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));

        _selectedCulture = this.AppConfig.CurrentCulture;
        _selectedTheme = this.AppConfig.CurrentTheme;
    }

    async partial void OnSelectedCultureChanged(CultureInfo value)
    {
        this.AppConfig.CurrentCulture = value;
        WeakReferenceMessenger.Default.Send(new ShowNotification
        (
            Message: Languages.LangAppliedAfterRestart,
            Level: EAlertLevel.Warning
        ));
        await this.AppConfig.SaveAsync().ConfigureAwait(false);

    }

    async partial void OnSelectedThemeChanged(AppTheme value)
    {
        this.AppConfig.CurrentTheme = value;
        await this.AppConfig.SaveAsync().ConfigureAwait(false);
    }
}