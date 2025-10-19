namespace randomkiwi.Interfaces;

internal interface IUserMetricsService
{
    void TrackNavigation(ENavigationType type, int toArticleId);
    EUserNavigationPattern AnalyzeNavigationPattern();
    int GetOptimalPoolSize();
    bool ShouldPrefetchAggressively();
}
