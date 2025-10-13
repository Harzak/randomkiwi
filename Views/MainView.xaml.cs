using CommunityToolkit.Mvvm.Messaging;
using randomkiwi.Views;

namespace randomkiwi;

public partial class MainView : ContentPage
{
    private readonly MainViewModel? _viewModel;

    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        WeakReferenceMessenger.Default.Register<ShowRandomArticleSettingsPopupMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                popupRandomArticleSettings.BindingContext = m.ViewModel;
                popupRandomArticleSettings.Show();
            });
        });

        WeakReferenceMessenger.Default.Register<ClosePopupRandomArticleSettingsMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                popupRandomArticleSettings.IsOpen = false;
            });
        });

        WeakReferenceMessenger.Default.Register<ShowNotification>(this, ShowNotification);

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel != null)
        {
            await _viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentViewModel))
        {
            UpdateMainContent();
        }
    }

    private void UpdateMainContent()
    {
        if (_viewModel?.CurrentViewModel == null)
        {
            ContentArea.Content = null;
            return;
        }

        IRoutableViewModel currentViewModel = _viewModel.CurrentViewModel;

        View? view = currentViewModel switch
        {
            RandomArticleViewModel mainVM => new RandomArticleView { BindingContext = mainVM },
            BookmarkListViewModel bookmarkListVM => new BookmarksListView { BindingContext = bookmarkListVM },
            SettingsViewModel settingsVM => new SettingsView { BindingContext = settingsVM },
            BookmarkViewModel bookmarkVM => new BookmarkView { BindingContext = bookmarkVM },
            _ => throw new NotImplementedException($"No view implemented for view model type {currentViewModel.GetType().FullName}"),
        };
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ContentArea.Content = view;
        });
    }

    private async void ShowNotification(object recipient, ShowNotification e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            NotificationLabel.Text = e.Message;
            NotificationContainer.IsVisible = true;
            NotificationContainer.Opacity = 0;
            NotificationContainer.BackgroundColor = e.Level switch
            {
                EAlertLevel.Info => (Color)Application.Current!.Resources["InfoBlue"],
                EAlertLevel.Success => (Color)Application.Current!.Resources["SuccessGreen"],
                EAlertLevel.Warning => (Color)Application.Current!.Resources["WarningOrange"],
                EAlertLevel.Error => (Color)Application.Current!.Resources["ErrorRed"],
                _ => (Color)Application.Current!.Resources["InfoBlue"]
            };
        });

        await NotificationContainer.FadeTo(1, 500).ConfigureAwait(false);
        await Task.Delay(e.DurationMs).ConfigureAwait(false);
        await NotificationContainer.FadeTo(0, 500).ConfigureAwait(false);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            NotificationContainer.IsVisible = false;
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}