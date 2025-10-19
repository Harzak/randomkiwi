namespace randomkiwi.Views.Controls;

public partial class WikipediaWebView : ContentView
{
    public WikipediaWebView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is WikipediaWebViewViewModel viewModel)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                WebViewContainer.Content = viewModel.WebView;
            });
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            WebViewContainer.Content = null;
        });
    }
}