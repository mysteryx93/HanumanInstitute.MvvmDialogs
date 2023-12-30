using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Strongly-typed View Locator that does not rely on reflection.
/// </summary>
public class StrongViewLocator : StrongViewLocatorBase, IDataTemplate, IViewLocatorNavigation
{
    /// <summary>
    /// Registers specified views as being associated with specified view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    /// <typeparam name="TView">The view type to associate with the view model.</typeparam>
    public void Register<TViewModel, TView>()
        where TViewModel : INotifyPropertyChanged
        where TView : Control, new() =>
        Register<TViewModel>(new ViewDefinition(typeof(TView), () => new TView()));

    /// <summary>
    /// Registers specified views as being associated with specified view model type.
    /// DesktopWindow or NavigationView will be selected based on runtime needs.  
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    /// <typeparam name="TNavView">The UserControl view associated with the view model for navigation mode.</typeparam>
    /// <typeparam name="TDeskView">The Window view associated with the view model for desktop applications.</typeparam>
    public void Register<TViewModel, TNavView, TDeskView>()
        where TViewModel : INotifyPropertyChanged
        where TNavView : UserControl, new()
        where TDeskView : Window, new() =>
        Register<TViewModel>(UseSinglePageNavigation ?
            new ViewDefinition(typeof(TNavView), () => new TNavView()) :
            new ViewDefinition(typeof(TDeskView), () => new TDeskView()));

    /// <summary>
    /// Gets or sets whether to force single-page navigation. Setting this to true can allow running in single-page mode on desktop.
    /// </summary>
    public bool ForceSinglePageNavigation
    {
        get => _forceSinglePageNavigation;
        set
        {
            if (Registrations.Count > 0)
            {
                throw new InvalidOperationException("ForceSinglePageNavigation must be set before registering views.");
            }
            _forceSinglePageNavigation = value;
        }
    }
    private bool _forceSinglePageNavigation;

    /// <inheritdoc />
    public virtual Control Build(object? data)
    {
        try
        {
            return (Control)Create(data!);
        }
        catch (Exception)
        {
            return new TextBlock
            {
                Text = $"No view registered for {data?.GetType().FullName}"
            };
        }
    }

    /// <inheritdoc />
    public virtual bool Match(object? data) => data is INotifyPropertyChanged;

    /// <summary>
    /// Gets whether the application runs in single-page navigation mode.
    /// </summary>
    public bool UseSinglePageNavigation => Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime || ForceSinglePageNavigation;
}
