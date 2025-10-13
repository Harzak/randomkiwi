using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

public interface IViewModelFactory
{
    IRoutableViewModel CreateBookmarkViewModel(Bookmark bookmark);
    IRoutableViewModel CreateRandomArticleViewModel();
    IRoutableViewModel CreateBookmarkListViewModel();
    IRoutableViewModel CreateSettingsViewModel();
}