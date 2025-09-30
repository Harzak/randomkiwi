using System.Globalization;

namespace randomkiwi.Views;

public partial class SettingsView : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private void SelecteduCultureIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker 
            && picker.SelectedItem is CultureInfo culture
            && picker.IsLoaded)
        {
            _viewModel?.OnSelectedCultureChanged();
        }
    }
}