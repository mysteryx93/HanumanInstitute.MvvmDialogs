using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

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

    /// <inheritdoc />
    public bool? ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternal(ownerViewModel, viewModel, DialogManager.FindWindowByViewModel(viewModel));

    /// <inheritdoc />
    public bool? ShowDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternal(ownerViewModel, viewModel, DialogManager.FindWindowByViewModel(viewModel));

    /// <summary>
    /// Displays a modal dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="view">The view to show.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected bool? ShowDialogInternal(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, object? view)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        DialogLogger.Write($"Dialog: {view?.GetType()}; View model: {viewModel.GetType()}; Owner: {ownerViewModel.GetType()}");
        DialogManager.AsSync().ShowDialog(ownerViewModel, viewModel, view);
        DialogLogger.Write($"Dialog: {view?.GetType()}; Result: {viewModel.DialogResult}");
        return viewModel.DialogResult;
    }
}
