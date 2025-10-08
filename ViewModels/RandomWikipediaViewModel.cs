using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using randomkiwi.Events;
using randomkiwi.Utilities.Results;

namespace randomkiwi.ViewModels;

public sealed partial class RandomWikipediaViewModel : BaseRoutableViewModel
{
    private readonly IArticleCatalog _articleCatalog;
    private readonly ILoadingService _loadingService;

    public override string Name => nameof(RandomWikipediaViewModel);
    public bool IsInError => !string.IsNullOrEmpty(this.ErrorMessage);
    public bool IsLoaded => !this.IsLoading && !this.IsInError;

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

    public RandomWikipediaViewModel(
        WikipediaWebViewViewModel webViewViewModel,
        IArticleCatalog articleCatalog,
        ILoadingService loadingService,
        INavigationService navigationService)
    : base(navigationService)
    {
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _articleCatalog = articleCatalog ?? throw new ArgumentNullException(nameof(articleCatalog));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));

        _loadingService.IsLoadingChanged += OnIsLoadingChanged;
    }

    public async override Task OnInitializedAsync()
    {
        await this.WebViewViewModel.InitializeAsync().ConfigureAwait(false);
        await this.ExecuteWithLoadingAsync(_articleCatalog.InitializeAsync).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task PreviousArticle()
    {
        await this.ExecuteWithLoadingAsync(() => Task.FromResult(_articleCatalog.Previous())).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NextArticle()
    {
        await this.ExecuteWithLoadingAsync(_articleCatalog.NextAsync).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddBookmark()
    {
        await this.ExecuteWithLoadingAsync(_articleCatalog.BookmarkAsync).ConfigureAwait(false);
    }

    private async Task ExecuteWithLoadingAsync(Func<Task<OperationResult>> operation)
    {
        this.ErrorMessage = null;
        using IDisposable loadingToken = _loadingService.BeginLoading();

        OperationResult result = await operation().ConfigureAwait(false);
        this.HandleResult(result);
    }

    private void HandleResult(OperationResult result)
    {
        if (result.IsSuccess && _articleCatalog.Current?.Url != null)
        {
            this.WebViewViewModel.NavigateToUrl(_articleCatalog.Current.Url);
        }
        else
        {
            this.ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Unknown error." : result.ErrorMessage;
        }
    }

    private void OnIsLoadingChanged(object? sender, LoadingChangedEventArgs e)
    {
        this.IsLoading = e.IsLoading;
    }

    private bool _disposed;
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;

            if (_loadingService != null)
            {
                _loadingService.IsLoadingChanged -= OnIsLoadingChanged;
            }
            _articleCatalog?.Dispose();
        }
        base.Dispose(disposing);
    }
}
