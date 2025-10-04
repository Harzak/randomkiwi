namespace randomkiwi.Models;

/// <summary>
/// Message sent when navigation completes.
/// </summary>
public sealed record NavigationCompletedMessage(string Url, bool IsSuccess);
