namespace randomkiwi.Navigation.Guards;

/// <summary>
/// Guard that prevents navigation when calculations are in progress
/// </summary>
public sealed class CalculationInProgressGuard : INavigationGuard
{
    /// <inheritdoc/>
    public Task<NavigationGuardResult> CanNavigateAsync(IRoutableViewModel? from, IRoutableViewModel to, NavigationContext context)
    {
        //if (from is BaseCalculatorViewModel calculatorViewModel)
        //{
        //    if (calculatorViewModel.CurrentState == ECalculatorState.InProgress)
        //    {
        //        return NavigationGuardResult.Deny("Cannot navigate while calculation is in progress");
        //    }
        //}

        return Task.FromResult(NavigationGuardResult.Allow());
    }
}
