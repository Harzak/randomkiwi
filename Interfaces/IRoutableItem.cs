using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Represents a navigable item in the application, providing metadata for UI presentation and routing.
/// </summary>
public interface IRoutableItem
{
    /// <summary>
    /// Gets the display name of the view model for UI presentation.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the URL path  identifier for this view model.
    /// </summary>
    public string UrlPath { get; }
}