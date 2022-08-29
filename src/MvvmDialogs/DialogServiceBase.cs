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
    /// <param name="viewModelFactory">Function used to create view model instances. This function is used only by <see cref="IDialogService.CreateViewModel"/> and is not used internally.</param>
    protected DialogServiceBase(AppDialogSettingsBase appSettings, IDialogManager dialogManager,
        Func<Type, object?>? viewModelFactory)
    {
        AppSettings = appSettings;
        DialogManager = dialogManager;
        ViewModelFactory = viewModelFactory;
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
    /// Function used to create view model instances. This function is used only by <see cref="IDialogService.CreateViewModel"/> and is not used internally.
    /// </summary>
    protected Func<Type, object?>? ViewModelFactory { get; }

    /// <inheritdoc />
    public void Show(INotifyPropertyChanged? ownerViewModel, INotifyPropertyChanged viewModel) =>
        ShowInternal(ownerViewModel, viewModel);

    /// <inheritdoc />
    public void Show<T>(INotifyPropertyChanged? ownerViewModel, INotifyPropertyChanged viewModel) =>
        ShowInternal(ownerViewModel, viewModel);

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternalAsync(ownerViewModel, viewModel);

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternalAsync(ownerViewModel, viewModel);

    /// <summary>
    /// Attempts to bring the window to the foreground and activates it.
    /// This method is not thread-safe and must be run on the UI thread.
    /// </summary>
    /// <param name="viewModel">The view model of the window.</param>
    /// <returns>true if the Window was successfully activated; otherwise, false.</returns>
    public bool Activate(INotifyPropertyChanged viewModel)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var window = DialogManager.FindWindowByViewModel(viewModel);
        DialogManager.Logger?.LogInformation("Activate View: {View}; ViewModel: {ViewModel}", window?.RefObj?.GetType(),
            viewModel.GetType());
        window?.Activate();
        return window != null;
    }

    /// <summary>
    /// Closes a non-modal dialog that previously was opened using <see cref="DialogServiceBase.Show"/>.
    /// This method is not thread-safe and must be run on the UI thread.
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
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected void ShowInternal(INotifyPropertyChanged? ownerViewModel, INotifyPropertyChanged viewModel)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        DialogManager.Show(ownerViewModel, viewModel);
    }

    /// <summary>
    /// Displays a modal dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected async Task<bool?> ShowDialogInternalAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        await DialogManager.ShowDialogAsync(ownerViewModel, viewModel);
        return viewModel.DialogResult;
    }

    /// <inheritdoc />
    public object CreateViewModel(Type type) =>
        ViewModelFactory?.Invoke(type) ??
        throw new NullReferenceException(
            $"ViewModelFactory was not set in the DialogService constructor or the function returned null for type '{type}'.");

    /// <inheritdoc />
    public T CreateViewModel<T>() =>
        (T?)ViewModelFactory?.Invoke(typeof(T)) ??
        throw new NullReferenceException(
            $"ViewModelFactory was not set in the DialogService constructor or the function returned null for type '{typeof(T)}'.");
}
