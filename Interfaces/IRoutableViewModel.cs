namespace randomkiwi.Interfaces;

/// <summary>
/// Interface for view models that can be navigated to and managed by the navigation service.
/// </summary>
public interface IRoutableViewModel : IRoutableItem, IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the viewmodel can be configured by the user.
    /// </summary>
    public bool CanBeConfigured { get; }

    /// <summary>
    /// Asynchronously performs initialization logic when the component is first rendered.
    /// </summary>
    Task OnInitializedAsync();

    /// <summary>
    /// Performs asynchronous cleanup operations when the object is being activated again (mostly in the context of go back navigation).
    /// </summary>
    Task OnResumeAsync();

    /// <summary>
    /// Performs asynchronous cleanup operations when the object is being destroyed by the navigation flow.
    /// </summary>
    /// <remarks>This method is not intended for resource disposal, which should be handled in the Dispose method.</remarks>
    Task OnDestroyAsync();

    /// <summary>
    /// Performs asynchronous cleanup and resource disposal when the object is forcibly destroyed.
    /// </summary>
    /// <remarks>This method is not intended for resource disposal, which should be handled in the Dispose method.</remarks>
    Task OnForceDestroyAsync();

    /// <summary>
    /// Asynchronously opens the configuration interface for the view model.
    /// </summary>
    Task OpenConfigurationAsync();
}