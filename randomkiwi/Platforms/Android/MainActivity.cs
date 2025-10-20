using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidGraphics = Android.Graphics;

namespace randomkiwi.Platforms.Android;

[Activity(Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private readonly INavigationService _navigationService;

    public MainActivity()
    {
        _navigationService = IPlatformApplication.Current?.Services.GetRequiredService<INavigationService>() ?? throw new ArgumentException();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        AndroidGraphics.Color color = new(GetColor(Resource.Color.colorPrimaryDark));
        Window?.SetNavigationBarColor(color);
    }

    public async override void OnBackPressed()
    {
        await _navigationService.NavigateBackAsync().ConfigureAwait(false);
    }
}