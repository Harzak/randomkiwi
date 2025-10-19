using Android.App;
using Android.Content.PM;

namespace randomkiwi.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private INavigationService _navigationService;
    public MainActivity()
    {
        _navigationService = IPlatformApplication.Current?.Services.GetRequiredService<INavigationService>() ?? throw new ArgumentException();
    }

    public async override void OnBackPressed()
    {
        await _navigationService.NavigateBackAsync().ConfigureAwait(false);
    }
}