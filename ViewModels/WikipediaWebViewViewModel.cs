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

    [ObservableProperty]
    private View? _webView;

    public WikipediaWebViewViewModel(IWebViewManager webViewManager)
    {
        _webViewManager = webViewManager ?? throw new ArgumentNullException(nameof(webViewManager));
    }

    public Task InitializeAsync()
    {
        this.WebView = _webViewManager.CreateView();
        return Task.CompletedTask;
    }

    public void NavigateToUrl(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if(this.WebView == null)
        {
            throw new InvalidOperationException("WebView is not initialized. Call InitializeAsync() before navigating.");
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