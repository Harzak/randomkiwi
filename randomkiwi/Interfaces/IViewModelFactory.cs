namespace randomkiwi.Interfaces;

public interface IViewModelFactory
{
    IRoutableViewModel CreateBookmarkViewModel(Bookmark bookmark);
    IRoutableViewModel CreateRandomArticleViewModel();
    IRoutableViewModel CreateBookmarkListViewModel();
    IRoutableViewModel CreateSettingsViewModel();
}