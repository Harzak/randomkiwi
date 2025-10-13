using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.ViewModels;

/// <summary>
/// Represents the view model for managing interactions with the WebView that displays Wikipedia articles.
/// </summary>
public sealed partial class WikipediaWebViewViewModel : ObservableObject, IDisposable
{
    private readonly IWebViewManager _webViewManager;
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private View? _webView;

    public bool CanGoBack => _navigation.CanNavigateBackPage;

    public WikipediaWebViewViewModel(IWebViewManager webViewManager, INavigationService navigation)
    {
        _webViewManager = webViewManager;
        _navigation = navigation;

        _navigation.CurrentPageChanged += OnCurrentPageChanged;
        _webViewManager.UserNavigated += OnUserNavigated;
    }

    public async Task InitializeAsync()
    {
        this.WebView = await MainThread.InvokeOnMainThreadAsync(_webViewManager.CreateView).ConfigureAwait(false);
    }

    public async Task NavigateToUrlAsync(Uri uri)
    {
        await _navigation.NavigateToAsync(uri).ConfigureAwait(false);
    }

    private async void OnCurrentPageChanged(object? sender, EventArgs e)
    {
        if (Uri.TryCreate(_navigation.CurrentPage?.UrlPath, UriKind.Absolute, out Uri? uri) && uri != null)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _webViewManager.Source = uri;
            })
            .ConfigureAwait(false);
        }
    }

    private async void OnUserNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (Uri.TryCreate(e.Url, UriKind.Absolute, out Uri? uri) && uri != null)
        {
            await _navigation.NavigateToAsync(uri).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _navigation.CurrentPageChanged -= OnCurrentPageChanged;
        _webViewManager.UserNavigated -= OnUserNavigated;
        _webViewManager?.Dispose();
    }
}