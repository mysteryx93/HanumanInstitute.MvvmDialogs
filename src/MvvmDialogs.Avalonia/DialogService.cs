using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;

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
    /// By default, <see cref="ViewLocatorBase"/> is used as dialog type locator
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
    /// <param name="viewLocator">Locator responsible for finding a dialog type matching a view model.
    /// If null, <see cref="ViewLocatorBase"/> will be used.</param>
    /// <param name="settings">Set application-wide settings.</param>
    public DialogService(
        IDialogManager? dialogManager = null,
        IViewLocator? viewLocator = null,
        AppDialogSettings? settings = null)
        : base(settings ?? new AppDialogSettings(),
            dialogManager ?? new DialogManager(new FrameworkDialogFactory()),
            viewLocator ?? new ViewLocatorBase())
    {
    }
}
