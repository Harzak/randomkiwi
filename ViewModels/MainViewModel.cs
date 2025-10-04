using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using randomkiwi.Interfaces;
using randomkiwi.Models;
using randomkiwi.Utilities;
using randomkiwi.Utilities.Results;

namespace randomkiwi.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IArticleCatalog _articleCatalog;
    private readonly IDebounceAction _debounceAction;

    [ObservableProperty]
    private WikipediaWebViewViewModel _webViewViewModel;

    public MainViewModel(
        WikipediaWebViewViewModel webViewViewModel,
        IArticleCatalog articleCatalog,
        Func<int, IDebounceAction> createDebounceAction)
    {
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _articleCatalog = articleCatalog ?? throw new ArgumentNullException(nameof(articleCatalog));
        _debounceAction = createDebounceAction?.Invoke(500) ?? throw new ArgumentNullException(nameof(createDebounceAction));
    }

    public async Task InitializeAsync()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(_articleCatalog.InitializeAsync);
        await this.ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task PreviousArticle()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(() => Task.FromResult(_articleCatalog.Previous()));
        await this.ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NextArticle()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(_articleCatalog.NextAsync);
        await this.ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddBookmark()
    {
        Task<OperationResult> operation() => _debounceAction.ExecuteAsync(_articleCatalog.BookmarkAsync);
        await this.ExecuteWithLoadingAsync(operation).ConfigureAwait(false);
    }

    private async Task ExecuteWithLoadingAsync(Func<Task<OperationResult>> operation)
    {
        this.WebViewViewModel.ErrorMessage = null;
        this.WebViewViewModel.IsLoading = true; //todo: global loading indicator

        OperationResult result = await operation().ConfigureAwait(false);
        this.HandleResult(result);

        this.WebViewViewModel.IsLoading = false;
    }

    private void HandleResult(OperationResult result)
    {
        if (result.IsSuccess && _articleCatalog.Current?.Url != null)
        {
            this.WebViewViewModel.NavigateToUrl(_articleCatalog.Current.Url);
        }
        else
        {
            this.WebViewViewModel.ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Unknown error." : result.ErrorMessage;
        }
    }
}
