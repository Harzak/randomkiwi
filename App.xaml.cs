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
        return new Window(appShell);
    }
}