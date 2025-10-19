namespace randomkiwi.Services;

/// <summary>
/// Service for tracking and analyzing user navigation patterns and behavior
/// </summary>
internal sealed class UserMetricsService : IUserMetricsService
{
    private readonly IAppSettingsProvider _settingsProvider;
    private readonly UserSessionMetrics _sessionMetrics;

    public UserMetricsService(IAppSettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        _sessionMetrics = new UserSessionMetrics();
    }

    public void TrackNavigation(ENavigationType type, int toArticleId)
    {
        _sessionMetrics.LastActivity = DateTime.UtcNow;
        _sessionMetrics.TotalNavigations++;

        switch (type)
        {
            case ENavigationType.Next:
                _sessionMetrics.ForwardNavigations++;
                _sessionMetrics.ConsecutiveForwardNavigations++;
                _sessionMetrics.ConsecutiveBackwardNavigations = 0;
                break;
            case ENavigationType.Previous:
                _sessionMetrics.BackwardNavigations++;
                _sessionMetrics.ConsecutiveBackwardNavigations++;
                _sessionMetrics.ConsecutiveForwardNavigations = 0;
                break;
        }

        NavigationEvent navigationEvent = new()
        {
            Type = type,
            ToArticleId = toArticleId
        };

        _sessionMetrics.RecentNavigations.Enqueue(navigationEvent);

        while (_sessionMetrics.RecentNavigations.Count > _settingsProvider.UserMetrics.MaxRecentNavigations)
        {
            _sessionMetrics.RecentNavigations.Dequeue();
        }
    }

    public EUserNavigationPattern AnalyzeNavigationPattern()
    {
        int totalNavigations = _sessionMetrics.TotalNavigations;
        if (totalNavigations == 0)
        {
            return EUserNavigationPattern.Unknown;
        }

        double forwardRatio = (double)_sessionMetrics.ForwardNavigations / totalNavigations;
        int consecutiveForward = _sessionMetrics.ConsecutiveForwardNavigations;

        if (forwardRatio > 0.8 && consecutiveForward > 3)
        {
            return EUserNavigationPattern.Reader;
        }

        if (_sessionMetrics.ConsecutiveBackwardNavigations > 2)
        {
            return EUserNavigationPattern.Reviewer;
        }

        return EUserNavigationPattern.Explorer;
    }

    public int GetOptimalPoolSize()
    {
        EUserNavigationPattern pattern = AnalyzeNavigationPattern();

        return pattern switch
        {
            EUserNavigationPattern.Reviewer => _settingsProvider.UserMetrics.BaseSizePool, // Standard size is fine
            EUserNavigationPattern.Explorer => (int)(_settingsProvider.UserMetrics.BaseSizePool * 1.5), // Moderate increase
            EUserNavigationPattern.Reader => _settingsProvider.UserMetrics.BaseSizePool * 2, // Needs more forward articles
            _ => _settingsProvider.UserMetrics.BaseSizePool
        };
    }

    public bool ShouldPrefetchAggressively()
    {
        return _sessionMetrics.IsActivelyNavigating &&
               _sessionMetrics.ConsecutiveForwardNavigations > 2;
    }
}