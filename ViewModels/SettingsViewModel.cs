using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

