namespace randomkiwi.Views;

public partial class BookmarksView : ContentPage
{
    public BookmarksView(BookmarksViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}