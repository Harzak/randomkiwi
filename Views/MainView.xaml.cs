namespace randomkiwi.Views;

public partial class MainView : ContentPage
{
    public MainView(MainViewModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}