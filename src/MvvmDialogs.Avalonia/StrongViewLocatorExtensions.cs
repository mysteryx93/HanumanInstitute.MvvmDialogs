namespace HanumanInstitute.MvvmDialogs.Avalonia;

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
        where TView : Control
    {
        return locator.Register<TViewModel>(typeof(TView));
    }
    
    /// <summary>
    /// Registers specified views as being associated with specified view model type.
    /// DesktopWindow or NavigationView will be selected based on runtime needs.  
    /// </summary>
    /// <param name="locator">The StrongViewLocator to register into.</param>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    /// <typeparam name="TNavView">The UserControl view associated with the view model for navigation mode.</typeparam>
    /// <typeparam name="TDeskView">The Window view associated with the view model for desktop applications.</typeparam>
    public static StrongViewLocator Register<TViewModel, TNavView, TDeskView>(this StrongViewLocator locator)
        where TViewModel : INotifyPropertyChanged
        where TNavView : UserControl
        where TDeskView : Window
    {
        return locator.Register<TViewModel>(locator.UseSinglePageNavigation ? typeof(TNavView) : typeof(TDeskView));
    }
    
    /// <summary>
    /// Registers specified views as being associated with specified view model type.
    /// DesktopWindow or NavigationView will be selected based on runtime needs.  
    /// </summary>
    /// <param name="locator">The StrongViewLocator to register into.</param>
    /// <param name="navigationView">The UserControl view associated with the view model for navigation mode.</param>
    /// <param name="desktopWindow">The Window view associated with the view model for desktop applications.</param>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    public static StrongViewLocator Register<TViewModel>(this StrongViewLocator locator, 
        Type navigationView, Type desktopWindow)
        where TViewModel : INotifyPropertyChanged
    {
        return locator.Register<TViewModel>(locator.UseSinglePageNavigation ? navigationView : desktopWindow);
    }
}
