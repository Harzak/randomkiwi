using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.Models;

/// <summary>
/// Message sent when navigation starts.
/// </summary>
internal sealed record NavigationStartedMessage(string Url);

/// <summary>
/// Message sent when navigation completes.
/// </summary>
internal sealed record NavigationCompletedMessage(string Url, bool IsSuccess);

/// <summary>
/// Represents a message used to display the Wikipedia Random Settings popup.
/// </summary>
internal sealed record ShowRandomArticleSettingsPopupMessage(ObservableObject ViewModel);

/// <summary>
/// Represents a message used to signal the closure of a popup related to Wikipedia random settings.
/// </summary>
internal sealed record ClosePopupRandomArticleSettingsMessage();

/// <summary>
/// Represents a notification to be displayed in the UI.
/// </summary>
internal sealed record ShowNotification(string Message, EAlertLevel Level, int DurationMs = 5000);
