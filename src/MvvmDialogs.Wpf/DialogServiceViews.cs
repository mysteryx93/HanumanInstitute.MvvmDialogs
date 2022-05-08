using System.ComponentModel;
using System.Windows;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class containing means to register a FrameworkElement as a view for a view
/// model when using the MVVM pattern. The view will then be used by the
/// <see cref="DialogService"/> when opening dialogs.
/// </summary>
public static class DialogServiceViews
{
    /// <summary>
    /// Attached property describing whether a <see cref="FrameworkElement"/> is acting as a
    /// view for a view model when using the MVVM pattern.
    /// </summary>
    public static readonly DependencyProperty IsRegisteredProperty =
        DependencyProperty.RegisterAttached(
            "IsRegistered",
            typeof(bool),
            typeof(DialogServiceViews),
            new PropertyMetadata(IsRegisteredChanged));

    /// <summary>
    /// Gets value describing whether <see cref="DependencyObject"/> is acting as a view for a
    /// view model when using the MVVM pattern
    /// </summary>
    public static bool GetIsRegistered(DependencyObject target) => (bool)target.GetValue(IsRegisteredProperty);

    /// <summary>
    /// Sets value describing whether <see cref="DependencyObject"/> is acting as a view for a
    /// view model when using the MVVM pattern
    /// </summary>
    public static void SetIsRegistered(DependencyObject target, bool value) => target.SetValue(IsRegisteredProperty, value);

    /// <summary>
    /// Is responsible for handling <see cref="IsRegisteredProperty"/> changes, i.e.
    /// whether <see cref="FrameworkElement"/> is acting as a view for a view model when using
    /// the MVVM pattern.
    /// </summary>
    private static void IsRegisteredChanged(
        DependencyObject target,
        DependencyPropertyChangedEventArgs e)
    {
        // The Visual Studio Designer or Blend will run this code when setting the attached
        // property in XAML, however we wish to abort the execution since the behavior adds
        // nothing to a designer.
        if (DesignerProperties.GetIsInDesignMode(target))
            return;

        if (target is FrameworkElement view)
        {
            if ((bool)e.NewValue)
            {
                ViewRegistration.Register(new ViewWrapper(view));
            }
            else
            {
                ViewRegistration.Unregister(new ViewWrapper(view));
            }
        }
    }
}
