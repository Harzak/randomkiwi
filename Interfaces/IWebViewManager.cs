using System.Net;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.Interfaces;

/// <summary>
/// Provides an abstraction for managing WebView functionality while maintaining MVVM separation.
/// </summary>
public interface IWebViewManager  : IDisposable
{
    /// <summary>
    /// Gets or sets the source URI of the WebView.
    /// </summary>
    Uri? Source { get; set; }

    /// <summary>
    /// Gets a value indicating whether the WebView can navigate back.
    /// </summary>
    bool CanGoBack { get; }
    
    /// <summary>
    /// Navigates the WebView back to the previous page.
    /// </summary>
    void GoBack();
    
    /// <summary>
    /// Creates and returns the actual WebView control.
    /// </summary>
    View CreateView();
}