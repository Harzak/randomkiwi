using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Defines the contract for date and time operations providing abstraction over DateTime and DateTimeOffset functionality.
/// </summary>
public interface IDateTimeFacade
{
    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    DateTime DateTimeNow();

    /// <summary>
    /// Gets the current Coordinated Universal Time (UTC) date and time.
    /// </summary>
    DateTime DateTimeUTCNow();

    /// <summary>
    /// Gets the current date and time with offset information.
    /// </summary>
    DateTimeOffset DateTimeOffsetNow();

    /// <summary>
    /// Gets the minimum value for DateTimeOffset.
    /// </summary>
    DateTimeOffset DateTimeOffsetMinValue();
}