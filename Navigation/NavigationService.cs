using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Navigation;

public interface INavigationService : IDisposable
{
    public IRoutableViewModel? CurrentViewModel { get; }
    public bool CanNavigateBackViewModel { get; }

    public string? CurrentWebUrl { get; }
    public bool CanNavigateBackWeb { get; }

    public event EventHandler<EventArgs>? CurrentViewModelChanged;
    public event EventHandler<EventArgs>? CurrentWebUrlChanged;

    Task InitializeAsync(IHostViewModel host);

    Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null);
    Task NavigateToAsync(Uri url, NavigationParameters? parameters = null);
    Task NavigateBackAsync(NavigationParameters? parameters = null);
}

/// <summary>
/// Unified navigation service that coordinates ViewModel and Web navigation
/// </summary>
internal sealed class NavigationService : INavigationService
{
    private readonly IViewModelNavigationService _viewModelNavigation;
    private readonly IWebPageNavigationService _webNavigation;

    public IRoutableViewModel? CurrentViewModel => _viewModelNavigation.CurrentViewModel;
    public bool CanNavigateBackViewModel => _viewModelNavigation.CanNavigateBack;

    public string? CurrentWebUrl => _webNavigation.CurrentPage?.UrlPath;
    public bool CanNavigateBackWeb => _webNavigation.CanNavigateBack;

    public event EventHandler<EventArgs>? CurrentViewModelChanged;
    public event EventHandler<EventArgs>? CurrentWebUrlChanged;

    public NavigationService(
        IViewModelNavigationService viewModelNavigation,
        IWebPageNavigationService webNavigation)
    {
        _viewModelNavigation = viewModelNavigation ?? throw new ArgumentNullException(nameof(viewModelNavigation));
        _webNavigation = webNavigation ?? throw new ArgumentNullException(nameof(webNavigation));

        _viewModelNavigation.CurrentViewModelChanged += OnCurrentViewModelChanged;
        _webNavigation.CurrentPageChanged += OnCurrentWebPageChanged;
    }

    public async Task InitializeAsync(IHostViewModel host)
    {
        await _viewModelNavigation.InitializeAsync(host).ConfigureAwait(false);
        await _webNavigation.InitializeAsync(host).ConfigureAwait(false);
    }

    public async Task NavigateToAsync(IRoutableViewModel viewModel, NavigationParameters? parameters = null)
    {
        await _viewModelNavigation.NavigateToAsync(viewModel, parameters).ConfigureAwait(false);
    }

    public async Task NavigateToAsync(Uri url, NavigationParameters? parameters = null)
    {
        await _webNavigation.NavigateToAsync(url, parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Smart navigation that prioritizes web navigation when in web-enabled ViewModels
    /// </summary>
    public async Task NavigateBackAsync(NavigationParameters? parameters = null)
    {
        if (CurrentViewModel is RandomWikipediaViewModel && CanNavigateBackWeb)
        {
            await NavigateBackWebAsync(parameters).ConfigureAwait(false);
        }
        else if (CanNavigateBackViewModel)
        {
            await NavigateBackViewModelAsync(parameters).ConfigureAwait(false);
        }
    }

    private async Task NavigateBackViewModelAsync(NavigationParameters? parameters = null)
    {
        await _viewModelNavigation.NavigateBackAsync(parameters).ConfigureAwait(false);
    }

    private async Task NavigateBackWebAsync(NavigationParameters? parameters = null)
    {
        await _webNavigation.NavigateBackAsync(parameters).ConfigureAwait(false);
    }

    private void OnCurrentViewModelChanged(object? sender, EventArgs e)
    {
        CurrentViewModelChanged?.Invoke(this, e);
    }

    private void OnCurrentWebPageChanged(object? sender, EventArgs e)
    {
        CurrentWebUrlChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        if (_viewModelNavigation != null)
        {
            _viewModelNavigation.CurrentViewModelChanged -= OnCurrentViewModelChanged;
            _viewModelNavigation.Dispose();
        }

        if (_webNavigation != null)
        {
            _webNavigation.CurrentPageChanged -= OnCurrentWebPageChanged;
            _webNavigation.Dispose();
        }
    }
}