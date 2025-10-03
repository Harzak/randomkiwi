using randomkiwi.ViewModels;

namespace randomkiwi.Views.Controls;

public partial class WikipediaWebView : ContentView
{
    public WikipediaWebView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is WikipediaWebViewViewModel viewModel)
        {
            WebViewContainer.Content = viewModel.WebViewManager.CreateView();
        }
    }
}