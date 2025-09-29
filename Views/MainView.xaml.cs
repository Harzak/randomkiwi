namespace randomkiwi.Views;

public partial class MainView : ContentPage
{
    private readonly MainViewModel _mainViewModel; 

    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _mainViewModel = viewModel; //meh
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainViewModel viewModel)
        {
            await viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}