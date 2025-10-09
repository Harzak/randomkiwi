using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace randomkiwi.ViewModels;

/// <summary>
/// Represents the view model for managing interactions with the WebView that displays Wikipedia articles.
/// </summary>
public sealed partial class WikipediaWebViewViewModel : ObservableObject, IDisposable
{
    private readonly IWebViewManager _webViewManager;
    private readonly IWebPageNavigationService _webNavigation;

    [ObservableProperty]
    private View? _webView;

    public bool CanGoBack => _webNavigation.CanNavigateBack;

    public WikipediaWebViewViewModel(IWebViewManager webViewManager, IWebPageNavigationService webNavigation)
    {
        _webViewManager = webViewManager;
        _webNavigation = webNavigation;

        _webNavigation.CurrentPageChanged += OnCurrentPageChanged;
        _webViewManager.UserNavigated += OnUserNavigated;
    }

    private async void OnCurrentPageChanged(object? sender, EventArgs e)
    {
        if (Uri.TryCreate(_webNavigation.CurrentPage?.UrlPath, UriKind.Absolute, out Uri? uri) && uri != null)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _webViewManager.Source = uri;
            }).ConfigureAwait(false);
        }
    }

    private async void OnUserNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (Uri.TryCreate(e.Url, UriKind.Absolute, out Uri? uri) && uri != null)
        {
            await _webNavigation.NavigateToAsync(uri).ConfigureAwait(false);
        }
    }

    public async Task InitializeAsync()
    {
        this.WebView = await MainThread.InvokeOnMainThreadAsync(_webViewManager.CreateView).ConfigureAwait(false);
    }

    public async Task NavigateToUrlAsync(Uri uri)
    {
        await _webNavigation.NavigateToAsync(uri).ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    public async Task GoBack()
    {
        await _webNavigation.NavigateBackAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _webNavigation.CurrentPageChanged -= OnCurrentPageChanged;
        _webViewManager.UserNavigated -= OnUserNavigated;
        _webViewManager?.Dispose();
    }
}