using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.ViewModels;

/// <summary>
/// Represents the view model for managing interactions with the WebView that displays Wikipedia articles.
/// </summary>
public sealed partial class WikipediaWebViewViewModel : ObservableObject
{
    private readonly IWebViewManager _webViewManager;

    /// <summary>
    /// Gets the WebView manager for programmatic control of the WebView.
    /// </summary>
    public IWebViewManager WebViewManager => _webViewManager;

    public bool CanGoBack => _webViewManager.CanGoBack;

    public WikipediaWebViewViewModel(IWebViewManager webViewManager)
    {
        _webViewManager = webViewManager ?? throw new ArgumentNullException(nameof(webViewManager));
    }

    public void NavigateToUrl(Uri uri)
    {
        if (uri == null)
        {
            return;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webViewManager.Source = uri;
        });
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    public void GoBack()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webViewManager.GoBack();
        });
    }
}