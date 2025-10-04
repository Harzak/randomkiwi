using randomkiwi.Interfaces;
using System.Net;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using randomkiwi.Models;
using randomkiwi.LogMessages;

namespace randomkiwi.Services;

/// <summary>
/// Manages WebView functionality while providing an abstraction layer for MVVM compliance.
/// </summary>
public sealed class WebViewManager : IWebViewManager
{
    private readonly IWebViewConfigurator _webViewConfigurator;
    private readonly IMessenger _messenger;
    private readonly IScriptLoader _scriptLoader;
    private readonly ILogger<WebViewManager> _logger;
    private bool _isNavigated;

    private WebView? _webView;

    /// <inheritdoc/>
    public event EventHandler<WebNavigatingEventArgs>? Navigating;

    /// <inheritdoc/>
    public event EventHandler<WebNavigatedEventArgs>? Navigated;

    /// <inheritdoc/>
    public event EventHandler? NavigationStateChanged;

    public WebViewManager(
        IWebViewConfigurator webViewConfigurator,
        IMessenger messenger,
        IScriptLoader scriptLoader,
        ILogger<WebViewManager> logger)
    {
        _webViewConfigurator = webViewConfigurator ?? throw new ArgumentNullException(nameof(webViewConfigurator));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _scriptLoader = scriptLoader ?? throw new ArgumentNullException(nameof(scriptLoader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Uri? Source
    {
        get => _webView?.Source is UrlWebViewSource urlSource ? new Uri(urlSource.Url) : null;
        set
        {
            if (_webView != null && value != null)
            {
                SetCookies(value);
                _webView.Source = value.ToString();
            }
        }
    }

    /// <inheritdoc/>
    public bool CanGoBack => _webView?.CanGoBack ?? false;

    /// <inheritdoc/>
    public bool CanGoForward => _webView?.CanGoForward ?? false;

    /// <inheritdoc/>
    public View CreateView()
    {
        if (_webView == null)
        {
            _webView = new WebView();
            _webView.Navigating += OnWebViewNavigating;
            _webView.Navigated += OnWebViewNavigated;
        }

        return _webView;
    }

    /// <inheritdoc/>
    public async Task<string?> EvaluateJavaScriptAsync(string script)
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

    /// <inheritdoc/>
    public void GoBack()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webView?.GoBack();
        });
    }

    /// <inheritdoc/>
    public void GoForward()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webView?.GoForward();
        });
    }

    /// <inheritdoc/>
    public void Reload()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _webView?.Reload();
        });
    }

    private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        _isNavigated = false;
        HandleNavigating(e);
        Navigating?.Invoke(sender, e);
    }

    private void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        _isNavigated = true;
        HandleNavigated(e);
        Navigated?.Invoke(sender, e);
        NavigationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleNavigating(WebNavigatingEventArgs args)
    {
        WikipediaWebViewLogs.NavigationStarted(_logger, args.Url?.ToString());

        _messenger.Send(new NavigationStartedMessage(args.Url?.ToString() ?? ""));
    }

    private void HandleNavigated(WebNavigatedEventArgs args)
    {
        bool isSuccess = args.Result == WebNavigationResult.Success;

        if (isSuccess && Uri.TryCreate(args.Url, UriKind.Absolute, out Uri? uri))
        {
            Task.Run(async () =>
            {
                string scriptContent = _scriptLoader.Load(AppConsts.SCRIPT_UI_FORMATTING_FILENAME);
                await EvaluateJavaScriptAsync(scriptContent).ConfigureAwait(false);
            });
            WikipediaWebViewLogs.NavigationCompleted(_logger, args.Url, isSuccess);
            _messenger.Send(new NavigationCompletedMessage(uri.ToString(), isSuccess));
            WikipediaWebViewLogs.LinkClicked(_logger, args.Url);
            _messenger.Send(new LinkClickedMessage(uri));
        }
    }

    private void SetCookies(Uri targetUri)
    {
        CookieContainer? cookies = _webViewConfigurator.CreateCookieContainer(targetUri);
        if (cookies != null && _webView != null)
        {
            _webView.Cookies = cookies;
            WikipediaWebViewLogs.CookieCreated(_logger, targetUri);
        }
        else
        {
            WikipediaWebViewLogs.CookieCreationFailed(_logger, targetUri);
        }
    }

    private bool _disposedValue;
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_webView != null)
                {
                    _webView.Navigating -= OnWebViewNavigating;
                    _webView.Navigated -= OnWebViewNavigated;
                }
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