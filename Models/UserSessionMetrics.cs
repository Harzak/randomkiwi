using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Represents user navigation and interaction metrics for the Wikipedia article catalog
/// </summary>
public sealed record UserSessionMetrics
{
    public DateTime LastActivity { get; set; } 
    public int TotalNavigations { get; set; }
    public int ForwardNavigations { get; set; }
    public int BackwardNavigations { get; set; }
    public int ConsecutiveForwardNavigations { get; set; }
    public int ConsecutiveBackwardNavigations { get; set; }
    public Queue<NavigationEvent> RecentNavigations { get; init; }
    public bool IsActivelyNavigating => (DateTime.UtcNow - LastActivity).TotalSeconds < 30;

    public UserSessionMetrics()
    {
        this.LastActivity = DateTime.UtcNow;
        this.RecentNavigations = [];
    }
}