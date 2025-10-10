using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using randomkiwi.Events;
using randomkiwi.Utilities.Results;

namespace randomkiwi.ViewModels;

public sealed partial class RandomWikipediaViewModel : BaseRoutableViewModel
{
    private readonly IArticleCatalog _articleCatalog;
    private readonly ILoadingService _loadingService;
    private readonly IAppConfiguration _appConfiguration;

    public override string Name => nameof(RandomWikipediaViewModel);
    public override bool CanBeConfigured => true;
    public bool IsInError => !string.IsNullOrWhiteSpace(this.ErrorMessage);
    public bool IsLoaded => !this.IsLoading && !this.IsInError;
    public bool CanGoNext => !this.IsLoading;
    public bool CanGoPrevious => !this.IsLoading && base.NavigationService.CanNavigateBackPage;

    [ObservableProperty]
    private WikipediaWebViewViewModel _webViewViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(CanGoPrevious))]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    private string? _errorMessage;

    public RandomWikipediaViewModel(
        WikipediaWebViewViewModel webViewViewModel,
        IArticleCatalog articleCatalog,
        ILoadingService loadingService,
        INavigationService navigationService,
        IAppConfiguration appConfiguration)
    : base(navigationService)
    {
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _articleCatalog = articleCatalog ?? throw new ArgumentNullException(nameof(articleCatalog));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));
        _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));

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
        await base.NavigationService.NavigateBackAsync().ConfigureAwait(false);
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

    public override Task OpenConfigurationAsync()
    {
        WikipediaRandomSettingsViewModel vm = new(_appConfiguration, callback: OnArticleDetailChanged);
        WeakReferenceMessenger.Default.Send(new ShowWikipediaRandomSettingsPopupMessage(vm));
        return Task.CompletedTask;
    }

    private async void OnArticleDetailChanged(EArticleDetail selectedArticleDetail)
    {
        await this.ExecuteWithLoadingAsync(_articleCatalog.RefreshAsync).ConfigureAwait(false);
    }

    private async Task ExecuteWithLoadingAsync(Func<Task<OperationResult>> operation)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.ErrorMessage = null;
        });

        using IDisposable loadingToken = _loadingService.BeginLoading();

        OperationResult result = await operation().ConfigureAwait(false);
        await this.HandleResultAsync(result).ConfigureAwait(false);
    }

    private async Task HandleResultAsync(OperationResult result)
    {
        if (result.IsSuccess && _articleCatalog.Current?.Url != null)
        {
            await this.WebViewViewModel.NavigateToUrlAsync(_articleCatalog.Current.Url).ConfigureAwait(false);
        }
        else
        {
            this.ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Unknown error." : result.ErrorMessage;
        }
    }

    private void OnIsLoadingChanged(object? sender, LoadingChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsLoading = e.IsLoading;
        });
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
            this.WebViewViewModel?.Dispose();
            _articleCatalog?.Dispose();
        }
        base.Dispose(disposing);
    }
}
