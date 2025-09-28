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
    [ObservableProperty]
    private IReadOnlyCollection<CultureInfo> _supportedCultures;

    [ObservableProperty]
    private CultureInfo _selectedCulture;

    [ObservableProperty]
    private string _selectedCultureStr;

    [ObservableProperty]
    private IReadOnlyCollection<string> _availableThemes;

    [ObservableProperty]
    private string _currentTheme;

    public SettingsViewModel()
    {
        _supportedCultures = [new("en-US"), new("fr-FR")];
        _selectedCulture = _supportedCultures.First();
        _selectedCultureStr = _selectedCulture.NativeName;

        _availableThemes = new List<string> { "Light", "Dark", "Default (system)" };
        _currentTheme = _availableThemes.First();
    }
}

