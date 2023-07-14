
namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in WPF.
/// </summary>
public class DialogService : DialogServiceBase, IDialogServiceSync
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
    /// <param name="viewModelFactory">Function used to create view model instances. This function is used only by <see cref="IDialogService.CreateViewModel"/> and is not used internally.</param>
    public DialogService(
        IDialogManager? dialogManager = null,
        Func<Type, object?>? viewModelFactory = null)
        : base(dialogManager ?? new DialogManager(dialogFactory: new DialogFactory()),
            viewModelFactory)
    {
    }

    /// <inheritdoc />
    public bool? ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternal(ownerViewModel, viewModel);

    /// <inheritdoc />
    public bool? ShowDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternal(ownerViewModel, viewModel);

    /// <summary>
    /// Displays a modal dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected bool? ShowDialogInternal(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        DialogManager.AsSync().ShowDialog(ownerViewModel, viewModel);
        return viewModel.DialogResult;
    }
}
