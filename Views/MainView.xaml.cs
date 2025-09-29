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

    protected override void OnAppearing()
    {
        _mainViewModel?.Initialize(); //meh, todo: app lifecyle
    }
}