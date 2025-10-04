namespace randomkiwi.Utilities;


/// <summary>
/// Provides a concrete implementation of date and time operations with abstraction over DateTime and DateTimeOffset functionality.
/// </summary>
public class DateTimeFacade : IDateTimeFacade
{
    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    public DateTime DateTimeNow() => DateTime.Now;

    /// <summary>
    /// Gets the current Coordinated Universal Time (UTC) date and time.
    /// </summary>
    public DateTime DateTimeUTCNow() => DateTime.UtcNow;

    /// <summary>
    /// Gets the current date and time with offset information.
    /// </summary>
    public DateTimeOffset DateTimeOffsetNow() => DateTimeOffset.Now;

    /// <summary>
    /// Gets the minimum value for DateTimeOffset.
    /// </summary>
    public DateTimeOffset DateTimeOffsetMinValue() => DateTimeOffset.MinValue;

    public static DateTimeFacade Default { get; } = new DateTimeFacade();
}