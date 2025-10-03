using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using randomkiwi.Configuration;
using randomkiwi.Models;
using System;
using System.Net;

namespace randomkiwi.ViewModels;

/// <summary>
/// Represents the view model for managing interactions with the WebView that displays Wikipedia articles.
/// </summary>
public sealed partial class WikipediaWebViewViewModel : ObservableObject
{
    private readonly ILogger<WikipediaWebViewViewModel> _logger;
    private readonly IWebViewConfigurator _webViewConfigurator;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private Uri? _currentUrl;

    [ObservableProperty]
    private CookieContainer? _cookiesContainer;

    public WikipediaWebViewViewModel(
        IWebViewConfigurator webViewConfigurator,
        IMessenger messenger,
        ILogger<WikipediaWebViewViewModel> logger)
    {
        _webViewConfigurator = webViewConfigurator ?? throw new ArgumentNullException(nameof(webViewConfigurator));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    partial void OnCurrentUrlChanging(Uri? oldValue, Uri? newValue)
    {
        if (newValue == null)
        {
            WikipediaWebViewLogs.UrlChangingToNull(_logger);
            return;
        }

        WikipediaWebViewLogs.UrlChanging(_logger, CurrentUrl?.ToString(), newValue?.ToString());
        _messenger.Send(new UrlChangingMessage(CurrentUrl, newValue));
    }

    internal void Navigating(WebNavigatingEventArgs args)
    {
        WikipediaWebViewLogs.NavigationStarted(_logger, args.Url?.ToString());

        if (Uri.TryCreate(args.Url, UriKind.Absolute, out Uri? uri))
        {
            CookieContainer? cookies = _webViewConfigurator.CreateCookieContainer(new Uri(args.Url));
            if (cookies != null)
            {
                this.CookiesContainer = cookies;
                WikipediaWebViewLogs.CookieCreated(_logger, uri);
            }
            else
            {
                WikipediaWebViewLogs.CookieCreationFailed(_logger, uri);
            }
            _messenger.Send(new NavigationStartedMessage(uri));
        }
    }

    internal void Navigated(WebNavigatedEventArgs args)
    {
        bool isSuccess = args.Result == WebNavigationResult.Success;

        if (Uri.TryCreate(args.Url, UriKind.Absolute, out Uri? uri))
        {
            WikipediaWebViewLogs.NavigationCompleted(_logger, args.Url, isSuccess);

            _messenger.Send(new NavigationCompletedMessage(uri, isSuccess));

            if (isSuccess)
            {
                WikipediaWebViewLogs.LinkClicked(_logger, args.Url);

                _messenger.Send(new LinkClickedMessage(uri));
            }
        }
    }
}