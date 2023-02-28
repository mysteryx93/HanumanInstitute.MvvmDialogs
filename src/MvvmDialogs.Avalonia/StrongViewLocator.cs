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
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    public StrongViewLocator Register<TViewModel>(ViewDefinition viewDef)
        where TViewModel : INotifyPropertyChanged
    {
        RegisterBase<TViewModel>(viewDef);
        return this;
    }

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
