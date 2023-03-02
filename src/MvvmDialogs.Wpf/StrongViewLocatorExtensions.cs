namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Provide extension methods to customize registrations.
/// </summary>
public static class StrongViewExtensions
{
    /// <summary>
    /// Registers specified views as being associated with specified view model type.
    /// </summary>
    /// <param name="locator">The StrongViewLocator to register into.</param>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    /// <typeparam name="TView">The view type to associate with the view model.</typeparam>
    public static StrongViewLocator Register<TViewModel, TView>(this StrongViewLocator locator)
        where TViewModel : INotifyPropertyChanged
        where TView : Control, new()
    {
        return locator.Register<TViewModel>(
            new ViewDefinition(typeof(TView), () => new TView()));
    }
}
