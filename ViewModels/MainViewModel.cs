using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using randomkiwi.Interfaces;
using randomkiwi.Models;
using randomkiwi.Utilities;
using randomkiwi.Utilities.Results;

namespace randomkiwi.ViewModels;

public sealed partial class MainViewModel : ObservableObject, IRecipient<NavigationStartedMessage>, IRecipient<NavigationCompletedMessage>
{
    private readonly IArticleCatalog _articleCatalog;
    private readonly IMessenger _messenger;
    private readonly IDebounceAction _debounceAction;

    public bool IsLoaded => !this.IsLoading && !this.IsInError;
    public bool IsInError => !string.IsNullOrEmpty(this.ErrorMessage);

    [ObservableProperty]
    private WikipediaWebViewViewModel _webViewViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    private string? _errorMessage;

    public MainViewModel(
        WikipediaWebViewViewModel webViewViewModel,
        IArticleCatalog articleCatalog,
        IMessenger messenger,
        Func<int, IDebounceAction> createDebounceAction)
    {
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _articleCatalog = articleCatalog ?? throw new ArgumentNullException(nameof(articleCatalog));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _debounceAction = createDebounceAction?.Invoke(500) ?? throw new ArgumentNullException(nameof(createDebounceAction));

        _messenger.RegisterAll(this);
    }

    public async Task InitializeAsync()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(_articleCatalog.InitializeAsync);
        await ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task PreviousArticle()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(() => Task.FromResult(_articleCatalog.Previous()));
        await ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NextArticle()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(_articleCatalog.NextAsync);
        await ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddBookmark()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(_articleCatalog.BookmarkAsync);
        await ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    private async Task ExecuteWithLoadingAsync(Func<Task<OperationResult>> operation)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.ErrorMessage = null;
            this.IsLoading = true;
        });

        OperationResult result = await operation().ConfigureAwait(false);
        this.HandleResult(result);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsLoading = false;
        });
    }

    private void HandleResult(OperationResult result)
    {
        if (result.IsSuccess && _articleCatalog.Current?.Url != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.WebViewViewModel.NavigateToUrl(_articleCatalog.Current.Url);
            });
        }
        else
        {
            SetError(result.ErrorMessage);
        }
    }

    private void SetError(string? message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ErrorMessage = string.IsNullOrWhiteSpace(message) ? "Unknown error." : message;
        });
    }

    public void Receive(NavigationStartedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsLoading = true;
        });
    }

    public void Receive(NavigationCompletedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsLoading = false;
        });
    }
}
