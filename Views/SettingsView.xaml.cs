using System.Globalization;

namespace randomkiwi.Views;

public partial class SettingsView : Grid
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void SelectedCultureIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker
            && picker.SelectedItem is CultureInfo
            && picker.IsLoaded
            && BindingContext is SettingsViewModel viewModel)
        {
            popup.Show();
            viewModel.OnSettingsChanged();
        }
    }

    private void SelectedThemeIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker
            && picker.SelectedItem is AppTheme
            && picker.IsLoaded
            && BindingContext is SettingsViewModel viewModel)
        {
            viewModel.OnSettingsChanged();
        }
    }

    private void ClosePopup_Clicked(object sender, EventArgs e)
    {
        popup.Dismiss();
    }
}