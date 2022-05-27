using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface responsible for UI interactions.
/// </summary>
/// <typeparam name="T">The base data type of the dialog window for target framework.</typeparam>
public abstract class DialogManagerBase<T> : IDialogManager
{
    /// <summary>
    /// A factory to resolve framework dialog types.
    /// </summary>
    protected IFrameworkDialogFactory FrameworkDialogFactory { get; }

    /// <summary>
    /// Initializes a new instance of the DisplayManager class.
    /// </summary>
    /// <param name="frameworkDialogFactory">A factory to resolve framework dialog types.</param>
    protected DialogManagerBase(IFrameworkDialogFactory frameworkDialogFactory) => FrameworkDialogFactory = frameworkDialogFactory;

    /// <inheritdoc />
    public virtual void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel, object? view)
    {
        var dialog = CreateDialog(ownerViewModel, viewModel, view);
        dialog.Show();
    }

    /// <inheritdoc />
    public virtual async Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, object? view)
    {
        var dialog = CreateDialog(ownerViewModel, viewModel, view);
        await dialog.ShowDialogAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Creates a new IWindow from the configured IDialogFactory.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="view">The view to show.</param>
    /// <returns>The new IWindow.</returns>
    /// <exception cref="TypeLoadException">Could not load view for view model.</exception>
    protected IWindow CreateDialog(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel, object? view)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        var dialog = view switch
        {
            IWindow w => w,
            T t => CreateWrapper(t),
            null => throw new TypeLoadException($"Could not load view for view model of type {viewModel.GetType().FullName}."),
            _ => throw new TypeLoadException($"Only dialogs of type {typeof(T)} or {typeof(IWindow)} are supported.")
        };

        dialog.Owner = FindWindowByViewModel(ownerViewModel);
        dialog.DataContext = viewModel;
        HandleDialogEvents(viewModel, dialog);
        return dialog;
    }

    /// <summary>
    /// Creates a wrapper around a native window.
    /// </summary>
    /// <param name="window">The window to create a wrapper for.</param>
    protected abstract IWindow CreateWrapper(T window);

    /// <summary>
    /// Handles window events. By default, ICloseable and IActivable are handled.
    /// </summary>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="dialog">The dialog being shown.</param>
    protected virtual void HandleDialogEvents(INotifyPropertyChanged viewModel, IWindow dialog)
    {
        if (viewModel is ICloseable c)
        {
            c.RequestClose += (_, _) => dialog.Close();
        }
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (viewModel is IActivable activable)
        {
            activable.RequestActivate += (_, _) => dialog.Activate();
        }
    }

    /// <inheritdoc />
    public virtual Task<TResult> ShowFrameworkDialogAsync<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings)
        where TSettings : DialogSettingsBase
    {
        var dialog = FrameworkDialogFactory.Create<TSettings, TResult>(settings, appSettings);
        var owner = FindWindowByViewModel(ownerViewModel) ?? throw new ArgumentException($"No view found with specified ownerViewModel of type {ownerViewModel.GetType()}.");
        return dialog.ShowDialogAsync(owner);
    }


    /// <inheritdoc />
    public abstract IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel);
}
