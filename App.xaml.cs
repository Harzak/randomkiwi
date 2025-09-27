namespace randomkiwi;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        AppShell appShell = new();
        return new Window(appShell)
        {
#if DEBUG && WINDOWS
            Height = 640,
            Width = 360,
#endif
        };
    }
}