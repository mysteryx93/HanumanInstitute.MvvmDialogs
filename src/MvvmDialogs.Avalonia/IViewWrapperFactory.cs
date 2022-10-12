namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Factory to create instances of <see cref="ViewWrapper"/> and <see cref="ViewNavigationWrapper"/>.
/// </summary>
public interface IViewWrapperFactory
{
    /// <summary>
    /// Creates a new <see cref="ViewWrapper"/> instance.
    /// </summary>
    /// <param name="viewModel">The view model that will be displayed by the view.</param>
    /// <param name="viewType">The data type of the view.</param>
    /// <returns>The new <see cref="ViewWrapper"/>.</returns>
    ViewWrapper Create(INotifyPropertyChanged viewModel, Type viewType);

    /// <summary>
    /// Creates a <see cref="ViewWrapper"/> instance around specified <see cref="Window"/>.
    /// </summary>
    /// <param name="view">The <see cref="Window"/> to get a <see cref="ViewWrapper"/> for.</param>
    /// <returns>The new <see cref="ViewWrapper"/>.</returns>
    public ViewWrapper Create(Window view);
    
    /// <summary>
    /// Creates a new <see cref="ViewNavigationWrapper"/> instance.
    /// </summary>
    /// <param name="viewModel">The view model that will be displayed by the view.</param>
    /// <param name="viewType">The data type of the view.</param>
    /// <returns>The new <see cref="ViewNavigationWrapper"/>.</returns>
    ViewNavigationWrapper CreateNavigation(INotifyPropertyChanged viewModel, Type viewType);

    /// <summary>
    /// Creates a <see cref="ViewNavigationWrapper"/> instance around specified <see cref="UserControl"/>.
    /// </summary>
    /// <param name="view">The <see cref="UserControl"/> to get a <see cref="ViewWrapper"/> for.</param>
    /// <returns>The new <see cref="ViewNavigationWrapper"/>.</returns>
    ViewNavigationWrapper CreateNavigation(UserControl view);
}
