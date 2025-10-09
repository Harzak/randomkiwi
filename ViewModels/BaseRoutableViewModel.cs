using CommunityToolkit.Mvvm.ComponentModel;

namespace randomkiwi.ViewModels;

/// <summary>
/// Abstract base class for view models that can be navigated to and support routing functionality.
/// </summary>
public abstract partial class BaseRoutableViewModel : ObservableValidator, IRoutableViewModel
{
    /// <inheritdoc/>
    protected INavigationService NavigationService { get; }

    /// <inheritdoc/>
    public string UrlPath { get; }

    /// <inheritdoc/>
    public abstract string Name { get; }

    protected BaseRoutableViewModel(INavigationService navigationService)
    {
        this.NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        this.UrlPath = Guid.NewGuid().ToString()[..5];
    }

    /// <inheritdoc/>
    public virtual Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnResumeAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnDestroyAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnForceDestroyAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {

    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}