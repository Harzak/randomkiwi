using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.ViewModels;

public sealed partial class RandomArticleSettingsViewModel : ObservableObject
{
    private readonly IAppConfiguration _appConfiguration;
    private readonly Action<EArticleDetail>? _callback;

    [ObservableProperty]
    private EArticleDetail _selectedArticleDetail;

    public RandomArticleSettingsViewModel(IAppConfiguration appConfiguration, Action<EArticleDetail>? callback)
    {
        _appConfiguration = appConfiguration;
        _callback = callback;
        this.SelectedArticleDetail = _appConfiguration.ArticleDetail;
    }

    [RelayCommand]
    private async Task Close()
    {
        if (_appConfiguration.ArticleDetail != this.SelectedArticleDetail)
        {
            _appConfiguration.ArticleDetail = this.SelectedArticleDetail;
            await _appConfiguration.SaveAsync().ConfigureAwait(false);
            _callback?.Invoke(this.SelectedArticleDetail);
        }
        WeakReferenceMessenger.Default.Send(new ClosePopupRandomArticleSettingsMessage());
    }
}