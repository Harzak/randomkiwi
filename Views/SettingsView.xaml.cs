using System.Globalization;

namespace randomkiwi.Views;

public partial class SettingsView : Grid
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void SelecteduCultureIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker
            && picker.SelectedItem is CultureInfo culture
            && picker.IsLoaded
            && BindingContext is SettingsViewModel viewModel)
        {
            viewModel.OnSelectedCultureChanged();
        }
    }
}