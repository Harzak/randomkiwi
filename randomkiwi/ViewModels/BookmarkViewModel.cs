using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.ViewModels;

public sealed partial class BookmarkViewModel : BaseRoutableViewModel
{
    private readonly ILoadingService _loadingService;
    private readonly Bookmark _bookmark;

    public override string Name => nameof(BookmarkViewModel);
    public override bool CanBeConfigured => false;
    public bool IsInError => !string.IsNullOrWhiteSpace(this.ErrorMessage);
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

    public BookmarkViewModel(
        Bookmark bookmark,
        WikipediaWebViewViewModel webViewViewModel,
        INavigationService navigationService,
        ILoadingService loadingService)
    : base(navigationService)
    {
        _bookmark = bookmark ?? throw new ArgumentNullException(nameof(bookmark));
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));

        _loadingService.IsLoadingChanged += OnIsLoadingChanged;
    }

    public async override Task OnInitializedAsync()
    {
        await this.WebViewViewModel.InitializeAsync().ConfigureAwait(false);
        await this.WebViewViewModel.NavigateToUrlAsync(_bookmark.Url).ConfigureAwait(false);
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
        }
        base.Dispose(disposing);
    }
}

