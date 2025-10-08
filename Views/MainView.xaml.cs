using randomkiwi.ViewModels;
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
            UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (_viewModel?.CurrentViewModel == null)
        {
            ContentArea.Content = null;
            return;
        }

        IRoutableViewModel currentViewModel = _viewModel.CurrentViewModel;
        
        View? view = currentViewModel switch
        {
            RandomWikipediaViewModel mainVM => new RandomWikipediaView { BindingContext = mainVM },
            BookmarksViewModel bookmarksVM => new BookmarksView { BindingContext = bookmarksVM },
            SettingsViewModel settingsVM => new SettingsView { BindingContext = settingsVM },
            _ => null
        };
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ContentArea.Content = view;
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
}