using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using randomkiwi.Interfaces;
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
    private readonly IMessenger _messenger;
    private readonly IWebViewManager _webViewManager;

    /// <summary>
    /// Gets the WebView manager for programmatic control of the WebView.
    /// </summary>
    public IWebViewManager WebViewManager => _webViewManager;

    public bool CanGoBack => _webViewManager.CanGoBack;

    public bool CanGoForward => _webViewManager.CanGoForward;

    public WikipediaWebViewViewModel(
        IWebViewManager webViewManager,
        IMessenger messenger,
        ILogger<WikipediaWebViewViewModel> logger)
    {
        _webViewManager = webViewManager ?? throw new ArgumentNullException(nameof(webViewManager));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _webViewManager.NavigationStateChanged += OnNavigationStateChanged;
    }

    public void NavigateToUrl(Uri uri)
    {
        if (uri == null)
        {
            WikipediaWebViewLogs.UrlChangingToNull(_logger);
            return;
        }

        WikipediaWebViewLogs.UrlChanging(_logger, _webViewManager.Source?.ToString(), uri.ToString());
        _messenger.Send(new UrlChangingMessage(_webViewManager.Source?.ToString() ?? "", uri.ToString()));

        _webViewManager.Source = uri;
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    public void GoBack()
    {
        _webViewManager.GoBack();
    }

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    public void GoForward()
    {
        _webViewManager.GoForward();
    }

    [RelayCommand]
    public void Reload()
    {
        _webViewManager.Reload();
    }

    private void OnNavigationStateChanged(object? sender, EventArgs e)
    {
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }
}