namespace randomkiwi;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        IAppConfiguration? appConfig = Handler?.MauiContext?.Services.GetRequiredService<IAppConfiguration>();
        appConfig?.InitializeAsync().Wait();

        IWikipediaAPIClient? httpService = Handler?.MauiContext?.Services.GetRequiredService<IWikipediaAPIClient>();
        httpService?.Initialize();

        MainView view = Handler?.MauiContext?.Services.GetRequiredService<MainView>() ?? throw new InvalidOperationException("MainHostPage not found in DI container");

        return new Window(view)
        {
#if DEBUG && WINDOWS
            Height = 640,
            Width = 360,
#endif
        };
    }
}