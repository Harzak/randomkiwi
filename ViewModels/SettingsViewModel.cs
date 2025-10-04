using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    public IAppConfiguration AppConfig { get; }

    public SettingsViewModel(IAppConfiguration appConfig)
    {
        this.AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
    }

    internal void OnSelectedCultureChanged()
    {


    }
}

