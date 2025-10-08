using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace randomkiwi.ViewModels;

/// <summary>
/// Host view model that manages the main navigation and flyout menu
/// </summary>
public sealed partial class MainViewModel : ObservableObject, IHostViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private IRoutableViewModel? _currentViewModel;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isFlyoutPresented;

    public MainViewModel(IServiceProvider serviceProvider, INavigationService navigationService)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    /// <summary>
    /// Initialize the host view model with the navigation service and navigate to the main view
    /// </summary>
    public async Task InitializeAsync()
    {
        await _navigationService.InitializeAsync(this).ConfigureAwait(false);
        await NavigateToHomeAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NavigateToHomeAsync()
    {
        IRoutableViewModel mainViewModel = _serviceProvider.GetRequiredService<RandomWikipediaViewModel>();
        await _navigationService.NavigateToAsync(mainViewModel).ConfigureAwait(false);
        IsFlyoutPresented = false;
    }

    [RelayCommand]
    private async Task NavigateToBookmarksAsync()
    {
        IRoutableViewModel bookmarksViewModel = _serviceProvider.GetRequiredService<BookmarksViewModel>();
        await _navigationService.NavigateToAsync(bookmarksViewModel).ConfigureAwait(false);
        IsFlyoutPresented = false;
    }

    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        IRoutableViewModel settingsViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
        await _navigationService.NavigateToAsync(settingsViewModel).ConfigureAwait(false);
        IsFlyoutPresented = false;
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