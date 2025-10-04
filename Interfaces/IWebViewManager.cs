using System.Net;
using CommunityToolkit.Mvvm.Messaging;

namespace randomkiwi.Interfaces;

/// <summary>
/// Provides an abstraction for managing WebView functionality while maintaining MVVM separation.
/// </summary>
public interface IWebViewManager  : IDisposable
{
    /// <summary>
    /// Occurs when the WebView starts navigating to a new page.
    /// </summary>
    event EventHandler<WebNavigatingEventArgs>? Navigating;
    
    /// <summary>
    /// Occurs when the WebView has finished navigating to a new page.
    /// </summary>
    event EventHandler<WebNavigatedEventArgs>? Navigated;
    
    /// <summary>
    /// Gets or sets the source URI of the WebView.
    /// </summary>
    Uri? Source { get; set; }
    
    /// <summary>
    /// Evaluates JavaScript code asynchronously in the WebView.
    /// </summary>
    Task<string?> EvaluateJavaScriptAsync(string script);
    
    /// <summary>
    /// Navigates the WebView back to the previous page.
    /// </summary>
    void GoBack();
    
    /// <summary>
    /// Navigates the WebView forward to the next page.
    /// </summary>
    void GoForward();
    
    /// <summary>
    /// Reloads the current page in the WebView.
    /// </summary>
    void Reload();
    
    /// <summary>
    /// Creates and returns the actual WebView control.
    /// </summary>
    View CreateView();
    
    /// <summary>
    /// Gets a value indicating whether the WebView can navigate back.
    /// </summary>
    bool CanGoBack { get; }
    
    /// <summary>
    /// Gets a value indicating whether the WebView can navigate forward.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Event that fires when navigation commands state changes.
    /// </summary>
    event EventHandler? NavigationStateChanged;
}