using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
internal sealed record ShowWikipediaRandomSettingsPopupMessage(ObservableObject ViewModel);

/// <summary>
/// Represents a message used to signal the closure of a popup related to Wikipedia random settings.
/// </summary>
internal sealed record ClosePopupWikipediaRandomSettingsMessage();
