using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Class abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in WPF.
/// </summary>
public class DialogService : DialogServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogServiceBase"/> class.
    /// </summary>
    /// <remarks>
    /// By default <see cref="ReflectionDialogFactory"/> is used as dialog factory,
    /// <see cref="DialogTypeLocatorBase"/> is used as dialog type locator
    /// and <see cref="FrameworkDialogFactory"/> is used as framework dialog factory.
    /// </remarks>
    public DialogService()
        : this(settings: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="dialogManager">Class responsible for UI interactions.</param>
    /// <param name="dialogTypeLocator">Locator responsible for finding a dialog type matching a view model. Default value is
    /// an instance of <see cref="DialogTypeLocatorBase"/>.</param>
    /// <param name="settings">Set application-wide settings.</param>
    public DialogService(
        IDialogManager? dialogManager = null,
        IDialogTypeLocator? dialogTypeLocator = null,
        AppDialogSettings? settings = null)
        : base(settings ?? new AppDialogSettings(),
            dialogManager ?? new DialogManagerBase(new FrameworkDialogFactory(), new ReflectionDialogFactory()),
            dialogTypeLocator ?? new NamingConventionDialogTypeLocator())
    {
    }

    private static IEnumerable<Window> Windows =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows ?? Array.Empty<Window>();

    /// <inheritdoc />
    protected override IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel) =>
        Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();

    /// <summary>
    /// Returns the Window owning specified ViewModel.
    /// </summary>
    /// <param name="ownerViewModel">The ViewModel to get the Window for.</param>
    /// <returns>A Window for the ViewModel..</returns>
    protected Window? FindOwnerWindow(INotifyPropertyChanged ownerViewModel) =>
        (ViewRegistration.FindView(ownerViewModel) as WindowWrapper)?.Ref;
}
