
namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in WPF.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Factory responsible for creating framework dialogs.
    /// </summary>
    IDialogManager DialogManager { get; }

    /// <summary>
    /// Set application-wide settings.
    /// </summary>
    AppDialogSettingsBase AppSettings { get; }

    /// <summary>
    /// Displays a non-modal dialog of a type that is determined by the dialog type locator.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel);

    /// <summary>
    /// Displays a non-modal dialog of specified type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <typeparam name="T">The type of the dialog to show.</typeparam>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    void Show<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel); //where T : TWindow;

    /// <summary>
    /// Displays a modal dialog of a type that is determined by the dialog type locator.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    Task<bool?> ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel);

    /// <summary>
    /// Displays a modal dialog of specified type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <typeparam name="T">The type of the dialog to show.</typeparam>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    Task<bool?> ShowDialogAsync<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel); // where T : TWindow;

    /// <summary>
    /// Attempts to bring the window to the foreground and activates it.
    /// </summary>
    /// <param name="viewModel">The view model of the window.</param>
    /// <returns>true if the window was successfully activated; otherwise, false.</returns>
    bool Activate(INotifyPropertyChanged viewModel);

    /// <summary>
    /// Closes a non-modal dialog that previously was opened using <see cref="Show"/>,
    /// </summary>
    /// <param name="viewModel">The view model of the dialog to close.</param>
    /// <returns>true if the window was successfully closed; otherwise, false.</returns>
    bool Close(INotifyPropertyChanged viewModel);

    /// <summary>
    /// Creates a view model of specified type, using the ViewModelFactory passed in the constructor.
    /// </summary>
    /// <param name="type">The type of the view model to create.</param>
    /// <returns>The newly created view model.</returns>
    object CreateViewModel(Type type);

    /// <summary>
    /// Creates a view model of specified type, using the ViewModelFactory passed in the constructor.
    /// </summary>
    /// <typeparam name="T">The type of the view model to create.</typeparam>
    /// <returns>The newly created view model.</returns>
    T CreateViewModel<T>();
}
