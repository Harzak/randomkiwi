using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using randomkiwi.Interfaces;
using randomkiwi.Models;

namespace randomkiwi.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IArticleCatalog _articleCatalog;

    public WikipediaArticleMetadata? Article => _articleCatalog.Current;

    public MainViewModel(IArticleCatalog articleCatalog)
    {
        _articleCatalog = articleCatalog;
    }

    public void Initialize()
    {
        _articleCatalog.InitializeAsync().Wait();
        base.OnPropertyChanged(nameof(Article));
    }

    [RelayCommand]
    private void PreviousArticle()
    {
        _articleCatalog.Previous();
        base.OnPropertyChanged(nameof(Article));
    }

    [RelayCommand]
    private async Task NextArticle()
    {
        await _articleCatalog.NextAsync().ConfigureAwait(false);
        base.OnPropertyChanged(nameof(Article));
    }


    [RelayCommand]
    private async Task AddBookmark()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
