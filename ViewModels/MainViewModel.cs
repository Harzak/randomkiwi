using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Hello, World!";
}
