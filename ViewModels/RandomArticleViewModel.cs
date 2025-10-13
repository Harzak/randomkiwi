using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.ViewModels;

public sealed partial class RandomArticleViewModel : BaseRoutableViewModel
{
    private readonly IArticleCatalog _articleCatalog;
    private readonly ILoadingService _loadingService;
    private readonly IBookmarksRepository _bookmarksRepository;
    private readonly IAppConfiguration _appConfiguration;

    public override string Name => nameof(RandomArticleViewModel);
    public override bool CanBeConfigured => true;
    public bool IsInError => !string.IsNullOrWhiteSpace(this.ErrorMessage);
    public bool IsLoaded => !this.IsLoading && !this.IsInError;
    public bool CanGoNext => !this.IsLoading;
    public bool CanGoPrevious => !this.IsLoading && base.NavigationService.CanNavigateBackPage;
    public bool CanAddBookmark => IsLoaded;
    public bool IsCurrentArticleBookmarked => _articleCatalog.Current != null && _bookmarksRepository.IsBookmarked(_articleCatalog.Current.Id);

    [ObservableProperty]
    private WikipediaWebViewViewModel _webViewViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    [NotifyPropertyChangedFor(nameof(IsCurrentArticleBookmarked))]
    [NotifyCanExecuteChangedFor(nameof(NextArticleCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousArticleCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddBookmarkCommand))]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    [NotifyPropertyChangedFor(nameof(IsInError))]
    private string? _errorMessage;

    public RandomArticleViewModel(
        WikipediaWebViewViewModel webViewViewModel,
        IArticleCatalog articleCatalog,
        ILoadingService loadingService,
        INavigationService navigationService,
        IBookmarksRepository bookmarksRepository,
        IAppConfiguration appConfiguration)
    : base(navigationService)
    {
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _articleCatalog = articleCatalog ?? throw new ArgumentNullException(nameof(articleCatalog));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));
        _bookmarksRepository = bookmarksRepository ?? throw new ArgumentNullException(nameof(bookmarksRepository));
        _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));

        _loadingService.IsLoadingChanged += OnIsLoadingChanged;
    }

    public async override Task OnInitializedAsync()
    {
        await this.WebViewViewModel.InitializeAsync().ConfigureAwait(false);
        await this.ExecuteWithLoadingAsync(_articleCatalog.InitializeAsync).ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousArticle()
    {
        await base.NavigationService.NavigateBackAsync().ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextArticle()
    {
        await this.ExecuteWithLoadingAsync(_articleCatalog.NextAsync).ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(CanAddBookmark))]
    private void AddBookmark()
    {
        if (_articleCatalog.Current == null)
        {
            return;
        }

        Bookmark bookmark = new()
        {
            WikipediaIdentifier = _articleCatalog.Current.Id,
            Identifier = Guid.NewGuid(),
            Title = _articleCatalog.Current.Title,
            Url = _articleCatalog.Current.Url,
            Description = _articleCatalog.Current.Description ?? string.Empty,
            DateAddedUtc = DateTime.UtcNow
        };

        _ = _bookmarksRepository.SaveAsync(bookmark).ContinueWith(async (x) =>
        {
            OperationResult result = await x.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                base.OnPropertyChanged(nameof(IsCurrentArticleBookmarked));

                WeakReferenceMessenger.Default.Send(new ShowNotification
                (
                    Message: Languages.BookmarkAdded,
                    Level: EAlertLevel.Info
                ));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new ShowNotification
                (
                    Message: string.IsNullOrWhiteSpace(result.ErrorMessage) ? Languages.UnknownError : result.ErrorMessage,
                    Level: EAlertLevel.Error
                ));
            }

        }, TaskScheduler.Current);
    }

    public override Task OpenConfigurationAsync()
    {
        RandomArticleSettingsViewModel vm = new(_appConfiguration, callback: OnArticleDetailChanged);
        WeakReferenceMessenger.Default.Send(new ShowRandomArticleSettingsPopupMessage(vm));
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
