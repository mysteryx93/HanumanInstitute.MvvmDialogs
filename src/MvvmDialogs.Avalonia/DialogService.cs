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
    /// and <see cref="DialogFactory"/> is used as framework dialog factory.
    /// </remarks>
    public DialogService()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="dialogManager">Class responsible for UI interactions.</param>
    /// <param name="settings">Set application-wide settings.</param>
    /// <param name="viewModelFactory">Function used to create view model instances. This function is used only by <see cref="IDialogService.CreateViewModel"/> and is not used internally.</param>
    public DialogService(
        IDialogManager? dialogManager = null,
        AppDialogSettings? settings = null,
        Func<Type, object?>? viewModelFactory = null)
        : base(settings ?? new AppDialogSettings(),
            dialogManager ?? new DialogManager(dialogFactory: new DialogFactory()),
            viewModelFactory)
    {
    }
}
