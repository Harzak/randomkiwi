using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace randomkiwi.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Hello, World!";

    [RelayCommand]
    private async Task PreviousArticle()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task NextArticle()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }


    [RelayCommand]
    private async Task AddBookmark()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
