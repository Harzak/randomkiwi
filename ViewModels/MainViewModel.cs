using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using randomkiwi.Interfaces;
using randomkiwi.Models;
using randomkiwi.Utilities;
using randomkiwi.Utilities.Results;

namespace randomkiwi.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IArticleCatalog _articleCatalog;
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
        Func<int, IDebounceAction> createDebounceAction)
    {
        _webViewViewModel = webViewViewModel;
        _articleCatalog = articleCatalog;
        _debounceAction = createDebounceAction?.Invoke(500) ?? throw new ArgumentNullException(nameof(createDebounceAction));
    }

    public async Task InitializeAsync()
    {
        await ExecuteWithLoadingAsync(() => _debounceAction.ExecuteAsync(_articleCatalog.InitializeAsync)).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task PreviousArticle()
    {
        await ExecuteWithLoadingAsync(() => _debounceAction.ExecuteAsync(() => Task.FromResult(_articleCatalog.Previous()))).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NextArticle()
    {
        await ExecuteWithLoadingAsync(() => _debounceAction.ExecuteAsync(_articleCatalog.NextAsync)).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddBookmark()
    {
        await ExecuteWithLoadingAsync(() => _debounceAction.ExecuteAsync(_articleCatalog.BookmarkAsync)).ConfigureAwait(false);
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
        if (result.IsSuccess)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.WebViewViewModel.CurrentUrl = _articleCatalog.Current?.Url;
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
}
