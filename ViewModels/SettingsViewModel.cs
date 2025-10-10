namespace randomkiwi.ViewModels;

public sealed partial class SettingsViewModel : BaseRoutableViewModel
{
    public IAppConfiguration AppConfig { get; }

    /// <inheritdoc/>
    public override string Name => nameof(SettingsViewModel);

    /// <inheritdoc/>
    public override bool CanBeConfigured => false;

    public SettingsViewModel(IAppConfiguration appConfig, INavigationService navigationService) : base(navigationService)
    {
        this.AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
    }

    internal async void OnSettingsChanged()
    {
        await this.AppConfig.SaveAsync().ConfigureAwait(false);
    }
}