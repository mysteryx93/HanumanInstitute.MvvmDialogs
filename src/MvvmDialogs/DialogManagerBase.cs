using Microsoft.Extensions.Logging;
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface responsible for UI interactions.
/// </summary>
/// <typeparam name="T">The base data type of the dialog window for target framework.</typeparam>
public abstract class DialogManagerBase<T> : IDialogManager
{
    /// <summary>
    /// Locator responsible for finding a dialog type matching a view model.
    /// </summary>
    protected IViewLocator ViewLocator { get; }

    /// <summary>
    /// A factory to resolve framework dialog types.
    /// </summary>
    protected IDialogFactory DialogFactory { get; }

    /// <summary>
    /// A ILogger to capture MvvmDialogs logs.
    /// </summary>
    public ILogger<IDialogManager>? Logger { get; }

    /// <summary>
    /// Initializes a new instance of the DisplayManager class.
    /// </summary>
    /// <param name="viewLocator">Locator responsible for finding a dialog type matching a view model.</param>
    /// <param name="dialogFactory">A factory to resolve framework dialog types.</param>
    /// <param name="logger">A ILogger to capture MvvmDialogs logs.</param>
    protected DialogManagerBase(IViewLocator viewLocator, IDialogFactory dialogFactory, ILogger<DialogManagerBase<T>>? logger)
    {
        ViewLocator = viewLocator;
        DialogFactory = dialogFactory;
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel)
    {
        Dispatch(() =>
        {
            var view = ViewLocator.Locate(viewModel);
            Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", view?.GetType(), viewModel.GetType(), ownerViewModel.GetType());

            var dialog = CreateDialog(ownerViewModel, viewModel, view);
            dialog.Show();
        });
    }

    /// <inheritdoc />
    public virtual async Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
    {
        await await DispatchAsync(async () =>
        {
            var view = ViewLocator.Locate(viewModel);
            Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", view?.GetType(), viewModel.GetType(), ownerViewModel.GetType());

            var dialog = CreateDialog(ownerViewModel, viewModel, view);
            await dialog.ShowDialogAsync();

            Logger?.LogInformation("View: {View}; Result: {Result}", view?.GetType(), viewModel.DialogResult);
        });
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
    /// Dispatches an action to the UI thread.
    /// </summary>
    /// <param name="action">The action to execute on the UI thread.</param>
    protected abstract void Dispatch(Action action);

    /// <summary>
    /// Dispatches an asynchronous action to the UI thread.
    /// </summary>
    /// <param name="action">The action to execute on the UI thread.</param>
    /// <typeparam name="D">The return type of the dispatched function.</typeparam>
    protected abstract Task<D> DispatchAsync<D>(Func<D> action);

    /// <summary>
    /// Handles window events. By default, ICloseable and IActivable are handled.
    /// </summary>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="dialog">The dialog being shown.</param>
    protected virtual void HandleDialogEvents(INotifyPropertyChanged viewModel, IWindow dialog)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (viewModel is ICloseable c)
        {
            c.RequestClose += (_, _) => Dispatch(dialog.Close);
        }
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (viewModel is IActivable activable)
        {
            activable.RequestActivate += (_, _) => Dispatch(dialog.Activate);
        }
    }

    /// <inheritdoc />
    public virtual async Task<object?> ShowFrameworkDialogAsync<TSettings>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings, Func<object?, string>? resultToString = null)
        where TSettings : DialogSettingsBase
    {
        Logger?.LogInformation("Dialog: {Dialog}; Title: {Title}", settings.GetType().Name, settings.Title);

        var result = await await DispatchAsync(() =>
        {
            var owner = FindWindowByViewModel(ownerViewModel) ?? throw new ArgumentException($"No view found with specified ownerViewModel of type {ownerViewModel.GetType()}.");
            return DialogFactory.ShowDialogAsync(owner, settings, appSettings);
        }).ConfigureAwait(false);

        Logger?.LogInformation("Dialog: {Dialog}; Result: {Result}", settings.GetType().Name, resultToString != null ? resultToString(result) : result?.ToString());
        return result;
    }


    /// <inheritdoc />
    public abstract IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel);
}
