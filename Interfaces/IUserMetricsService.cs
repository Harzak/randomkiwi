using randomkiwi.Models;
using randomkiwi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

internal interface IUserMetricsService
{
    void TrackNavigation(ENavigationType type, int toArticleId);
    EUserNavigationPattern AnalyzeNavigationPattern();
    int GetOptimalPoolSize();
    bool ShouldPrefetchAggressively();
}
