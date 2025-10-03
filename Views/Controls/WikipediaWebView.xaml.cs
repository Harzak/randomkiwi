using randomkiwi.ViewModels;
using System.Net;

namespace randomkiwi.Views.Controls;

public partial class WikipediaWebView : ContentView
{
    public WikipediaWebView()
    {
        InitializeComponent();
    }

    private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        if (BindingContext is WikipediaWebViewViewModel viewModel)
        {
            viewModel.Navigated(e);
        }
    }

    private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (BindingContext is WikipediaWebViewViewModel viewModel)
        {
            viewModel.Navigating(e);
        }
    }
}