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
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Gets the WebView manager for programmatic control of the WebView.
    /// </summary>
    public IWebViewManager WebViewManager => _webViewManager;

    public bool CanGoBack => _webViewManager.CanGoBack;

    [ObservableProperty]
    private View? _webView;

    public WikipediaWebViewViewModel(IWebViewManager webViewManager, INavigationService navigationService)
    {
        _webViewManager = webViewManager ?? throw new ArgumentNullException(nameof(webViewManager));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        _webViewManager.UserNavigated += OnWebViewManagerUserNavigated;
    }

    private async void OnWebViewManagerUserNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (Uri.TryCreate(e.Url, UriKind.Absolute, out Uri? uri))
        {
            await _navigationService.NavigateToAsync(uri).ConfigureAwait(false);
        }
    }

    public async Task InitializeAsync()
    {
        this.WebView = await MainThread.InvokeOnMainThreadAsync(_webViewManager.CreateView).ConfigureAwait(false);
    }

    public async Task NavigateToUrlAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (this.WebView == null)
        {
            throw new InvalidOperationException("WebView is not initialized. Call InitializeAsync() before navigating.");
        }
        await _navigationService.NavigateToAsync(uri).ConfigureAwait(false);

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

    private bool _disposedValue;
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _webViewManager?.Dispose();
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~WikipediaWebViewViewModel() => Dispose(false);
}