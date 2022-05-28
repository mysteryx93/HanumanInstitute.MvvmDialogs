
using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Class abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in WPF.
/// </summary>
public abstract class DialogServiceBase : IDialogService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogServiceBase"/> class.
    /// </summary>
    /// <param name="appSettings">Set application-wide settings.</param>
    /// <param name="dialogManager">Class responsible to manage UI interactions.</param>
    /// <param name="viewLocator">Locator responsible for finding a dialog type matching a view model.</param>
    protected DialogServiceBase(AppDialogSettingsBase appSettings, IDialogManager dialogManager, IViewLocator viewLocator)
    {
        AppSettings = appSettings;
        DialogManager = dialogManager;
        ViewLocator = viewLocator;
    }

    /// <summary>
    /// Set application-wide settings.
    /// </summary>
    public AppDialogSettingsBase AppSettings { get; }

    /// <summary>
    /// Factory responsible for creating dialogs.
    /// </summary>
    public IDialogManager DialogManager { get; }

    /// <summary>
    /// Locator responsible for finding a dialog type matching a view model.
    /// </summary>
    protected IViewLocator ViewLocator { get; }

    /// <inheritdoc />
    public void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel) =>
        ShowInternal(ownerViewModel, viewModel, ViewLocator.Locate(viewModel));

    /// <inheritdoc />
    public void Show<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel) =>
        ShowInternal(ownerViewModel, viewModel, ViewLocator.Locate(viewModel));

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternalAsync(ownerViewModel, viewModel, ViewLocator.Locate(viewModel));

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternalAsync(ownerViewModel, viewModel, ViewLocator.Locate(viewModel));

    /// <summary>
    /// Attempts to bring the window to the foreground and activates it.
    /// </summary>
    /// <param name="viewModel">The view model of the window.</param>
    /// <returns>true if the Window was successfully activated; otherwise, false.</returns>
    public bool Activate(INotifyPropertyChanged viewModel)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var window = DialogManager.FindWindowByViewModel(viewModel);
        DialogManager.Logger?.LogInformation("Activate View: {View}; ViewModel: {ViewModel}", window?.RefObj?.GetType(), viewModel.GetType());
        window?.Activate();
        return window != null;
    }

    /// <summary>
    /// Closes a non-modal dialog that previously was opened using <see cref="DialogServiceBase.Show"/>,
    /// <see cref="DialogServiceBase.Show{T}"/>.
    /// </summary>
    /// <param name="viewModel">The view model of the dialog to close.</param>
    /// <returns>true if the Window was successfully closed; otherwise, false.</returns>
    public bool Close(INotifyPropertyChanged viewModel)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var window = DialogManager.FindWindowByViewModel(viewModel);
        DialogManager.Logger?.LogInformation("Close View: {View}; ViewModel: {ViewModel}", window?.RefObj?.GetType(), viewModel.GetType());
        if (window != null)
        {
            try
            {
                window.Close();
                return true;
            }
            catch (Exception e)
            {
                DialogManager.Logger?.LogWarning(e, "Failed to close dialog");
            }
        }
        return false;
    }

    /// <summary>
    /// Displays a non-modal dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="view">The view to show.</param>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected void ShowInternal(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel, object? view)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        DialogManager.Show(ownerViewModel, viewModel, view);
    }

    /// <summary>
    /// Displays a modal dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="view">The view to show.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected async Task<bool?> ShowDialogInternalAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, object? view)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        await DialogManager.ShowDialogAsync(ownerViewModel, viewModel, view);
        return viewModel.DialogResult;
    }
}
