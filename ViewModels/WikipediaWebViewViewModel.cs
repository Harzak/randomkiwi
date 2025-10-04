using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.ViewModels;

/// <summary>
/// Represents the view model for managing interactions with the WebView that displays Wikipedia articles.
/// </summary>
public sealed partial class WikipediaWebViewViewModel : ObservableObject, IRecipient<NavigationStartedMessage>, IRecipient<NavigationCompletedMessage>
{
    private readonly IWebViewManager _webViewManager;
    private readonly IMessenger _messenger;

    /// <summary>
    /// Gets the WebView manager for programmatic control of the WebView.
    /// </summary>
    public IWebViewManager WebViewManager => _webViewManager;

    public bool CanGoBack => _webViewManager.CanGoBack;

    public bool IsLoaded => !this.IsLoading && !this.IsInError;
    public bool IsInError => !string.IsNullOrEmpty(this.ErrorMessage);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    private bool _isLoading;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    private string? _errorMessage;

    public WikipediaWebViewViewModel(IWebViewManager webViewManager, IMessenger messenger)
    {
        _webViewManager = webViewManager ?? throw new ArgumentNullException(nameof(webViewManager));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

        _messenger.RegisterAll(this);
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

    public void Receive(NavigationStartedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsLoading = true;
            this.GoBackCommand.NotifyCanExecuteChanged();
        });
    }

    public void Receive(NavigationCompletedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsLoading = false;
            this.GoBackCommand.NotifyCanExecuteChanged();
        });
    }
}