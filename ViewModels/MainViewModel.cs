using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.ViewModels;

/// <summary>
/// Host view model that manages the main navigation and flyout menu
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _viewModelFactory;

    /// <inheritdoc/>
    public bool CanOpenOverflowMenu => this.CurrentViewModel?.CanBeConfigured ?? false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanOpenOverflowMenu))]
    private IRoutableViewModel? _currentViewModel;

    [ObservableProperty]
    private bool _isFlyoutPresented;

    public MainViewModel(IViewModelFactory viewModelFactory, INavigationService navigationService)
    {
        _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    /// <summary>
    /// Initialize the host view model with the navigation service and navigate to the main view
    /// </summary>
    public async Task InitializeAsync()
    {
        await NavigateToHomeAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NavigateToHomeAsync()
    {
        IRoutableViewModel mainViewModel = _viewModelFactory.CreateRandomArticleViewModel();
        await _navigationService.NavigateToAsync(mainViewModel).ConfigureAwait(false);
        IsFlyoutPresented = false;
    }

    [RelayCommand]
    private async Task NavigateToBookmarksAsync()
    {
        IRoutableViewModel bookmarksViewModel = _viewModelFactory.CreateBookmarkListViewModel();
        await _navigationService.NavigateToAsync(bookmarksViewModel).ConfigureAwait(false);
        IsFlyoutPresented = false;
    }

    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        IRoutableViewModel settingsViewModel = _viewModelFactory.CreateSettingsViewModel();
        await _navigationService.NavigateToAsync(settingsViewModel).ConfigureAwait(false);
        IsFlyoutPresented = false;
    }

    [RelayCommand]
    private async Task OpenOverflowMenu()
    {
        if (this.CurrentViewModel != null && this.CurrentViewModel.CanBeConfigured)
        {
            await CurrentViewModel.OpenConfigurationAsync().ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private void ToggleFlyout()
    {
        IsFlyoutPresented = !IsFlyoutPresented;
    }

    [RelayCommand]
    private void CloseFlyout()
    {
        IsFlyoutPresented = false;
    }

    private void OnCurrentViewModelChanged(object? sender, EventArgs e)
    {
        CurrentViewModel = _navigationService.CurrentViewModel;
    }

    private bool _disposed;
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (_navigationService != null)
            {
                _navigationService.CurrentViewModelChanged -= OnCurrentViewModelChanged;
                _navigationService.Dispose();
            }
        }
    }
}