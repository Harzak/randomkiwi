using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Net;

namespace randomkiwi.Services;

/// <summary>
/// Manages WebView functionality while providing an abstraction layer for MVVM compliance.
/// </summary>
public sealed class WebViewManager : IWebViewManager
{
    private readonly IWebViewConfigurator _webViewConfigurator;
    private readonly ILoadingService _loadingService;
    private readonly IScriptLoader _scriptLoader;
    private readonly ILogger<WebViewManager> _logger;

    private bool _isNavigated;
    private IDisposable? _loadingToken;
    private WebView? _webView;

    public event EventHandler<WebNavigatedEventArgs>? UserNavigated;

    public WebViewManager(
        IWebViewConfigurator webViewConfigurator,
        ILoadingService loadingService,
        IScriptLoader scriptLoader,
        ILogger<WebViewManager> logger)
    {
        _webViewConfigurator = webViewConfigurator ?? throw new ArgumentNullException(nameof(webViewConfigurator));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));
        _scriptLoader = scriptLoader ?? throw new ArgumentNullException(nameof(scriptLoader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Uri? Source
    {
        get => _webView?.Source is UrlWebViewSource urlSource ? new Uri(urlSource.Url) : null;
        set
        {
            if (_webView == null)
            {
                return;
                throw new InvalidOperationException("WebView is not created. Call CreateView() before setting the Source.");
            }
            if (value != null)
            {
                this.SetCookies(value);
                _webView.Source = value.ToString();
            }
        }
    }

    /// <inheritdoc/>
    public bool CanGoBack => _webView?.CanGoBack ?? false;

    /// <inheritdoc/>
    public View CreateView()
    {
        if (_webView == null)
        {
            _webView = new WebView();
            _webView.Navigated += OnWebViewNavigated;
        }

        return _webView;
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webView?.GoBack();
        });
    }

    private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        _loadingToken = _loadingService.BeginLoading();
        _isNavigated = false;
        WikipediaWebViewLogs.NavigationStarted(_logger, e.Url?.ToString());
    }

    private async void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        _isNavigated = true;

        bool isSuccess = e.Result == WebNavigationResult.Success;

        if (isSuccess)
        {
            await this.EvaluateEmbeddedPageScriptAsync().ConfigureAwait(false);
        }

        WikipediaWebViewLogs.NavigationCompleted(_logger, e.Url, isSuccess);
        _loadingToken?.Dispose();

        if (this.Source != null &&  e.Result == WebNavigationResult.Success && e.Url != this.Source.AbsoluteUri)
        {
            this.UserNavigated?.Invoke(this, e);
        }
    }

    private void SetCookies(Uri targetUri)
    {
        CookieContainer? cookies = _webViewConfigurator.CreateCookieContainer(targetUri);
        if (cookies != null && _webView != null)
        {
            _webView.Cookies = cookies;
        }
    }

    private async Task EvaluateEmbeddedPageScriptAsync()
    {
        string uiFormattingScriptContent = _scriptLoader.Load(AppConsts.SCRIPT_UI_FORMATTING_FILENAME);
        await EvaluateJavaScriptAsync(uiFormattingScriptContent).ConfigureAwait(false);
    }

    private async Task<string?> EvaluateJavaScriptAsync(string script)
    {
        if (_webView != null && _isNavigated)
        {
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                return (string?)await _webView.EvaluateJavaScriptAsync(script).ConfigureAwait(false);

            }).ConfigureAwait(false);
        }
        else
        {
            return null;
        }
    }

    private bool _disposedValue;
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _loadingToken?.Dispose();
                _loadingService?.Dispose();
                if (_webView != null)
                {
                    _webView.Navigating -= OnWebViewNavigating;
                    _webView.Navigated -= OnWebViewNavigated;
                }
                _webView = null;
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~WebViewManager() => Dispose(false);
}