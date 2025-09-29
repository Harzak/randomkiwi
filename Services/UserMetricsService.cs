using randomkiwi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Services;

/// <summary>
/// Service for tracking and analyzing user navigation patterns and behavior
/// </summary>
internal sealed class UserMetricsService : IUserMetricsService
{
    private const int MAX_RECENT_NAVIGATIONS = 20;

    private readonly UserSessionMetrics _sessionMetrics;

    public UserMetricsService()
    {
        _sessionMetrics = new UserSessionMetrics();
    }

    public void TrackNavigation(ENavigationType type, int? fromArticleId, int? toArticleId)
    {
        _sessionMetrics.LastActivity = DateTime.UtcNow;
        _sessionMetrics.TotalNavigations++;

        // Track consecutive navigations
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

        // Record the navigation event
        var navigationEvent = new NavigationEvent
        {
            Type = type,
            FromArticleId = fromArticleId,
            ToArticleId = toArticleId
        };

        _sessionMetrics.RecentNavigations.Enqueue(navigationEvent);

        // Keep only recent navigations
        while (_sessionMetrics.RecentNavigations.Count > MAX_RECENT_NAVIGATIONS)
        {
            _sessionMetrics.RecentNavigations.Dequeue();
        }
    }


    public EUserNavigationPattern AnalyzeNavigationPattern()
    {
        var totalNavigations = _sessionMetrics.TotalNavigations;
        if (totalNavigations == 0)
            return EUserNavigationPattern.Unknown;

        var forwardRatio = (double)_sessionMetrics.ForwardNavigations / totalNavigations;
        var consecutiveForward = _sessionMetrics.ConsecutiveForwardNavigations;

        if (forwardRatio > 0.8 && consecutiveForward > 3)
            return EUserNavigationPattern.Reader;

        if (_sessionMetrics.ConsecutiveBackwardNavigations > 2)
            return EUserNavigationPattern.Reviewer;

        return EUserNavigationPattern.Explorer;
    }

    public int GetOptimalPoolSize()
    {
        var pattern = AnalyzeNavigationPattern();
        var baseSize = 20;

        return pattern switch
        {
            EUserNavigationPattern.Reviewer => baseSize, // Standard size is fine
            EUserNavigationPattern.Explorer => (int)(baseSize * 1.5), // Moderate increase
            EUserNavigationPattern.Reader => baseSize * 2, // Needs more forward articles
            _ => baseSize
        };
    }

    public bool ShouldPrefetchAggressively()
    {
        return _sessionMetrics.IsActivelyNavigating &&
               _sessionMetrics.ConsecutiveForwardNavigations > 2;
    }
}