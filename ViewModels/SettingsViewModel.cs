using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.ViewModels;

public sealed partial class SettingsViewModel : BaseRoutableViewModel
{
    public IAppConfiguration AppConfig { get; }

    public override string Name => nameof(SettingsViewModel);

    public SettingsViewModel(IAppConfiguration appConfig, INavigationService navigationService) : base(navigationService)
    {
        this.AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
    }

    internal void OnSelectedCultureChanged()
    {


    }

    public async override Task OnDestroyAsync()
    {
        await this.AppConfig.SaveAsync().ConfigureAwait(false);
        await base.OnDestroyAsync().ConfigureAwait(false);
    }
}

