using Microsoft.Extensions.Logging;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuspiciousTypeConversion.Global

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
    public virtual void Show(INotifyPropertyChanged? ownerViewModel, INotifyPropertyChanged viewModel)
    {
        Dispatch(
            () =>
            {
                var viewType = ViewLocator.Locate(viewModel);
                Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", viewType, viewModel.GetType(), ownerViewModel?.GetType());

                var dialog = CreateDialog(viewModel, viewType);
                dialog.Show(GetOwnerView(ownerViewModel));
            });
    }

    /// <inheritdoc />
    public virtual async Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
    {
        await await DispatchAsync(
            async () =>
            {
                var viewType = ViewLocator.Locate(viewModel);
                Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", viewType, viewModel.GetType(), ownerViewModel.GetType());

                var dialog = CreateDialog(viewModel, viewType);
                await dialog.ShowDialogAsync(GetOwnerView(ownerViewModel)!);

                Logger?.LogInformation("View: {View}; Result: {Result}", viewType, viewModel.DialogResult);
            });
    }

    private IView? GetOwnerView(INotifyPropertyChanged? ownerViewModel)
    {
        if (ownerViewModel == null) { return null; }
        var owner = FindViewByViewModel(ownerViewModel);
        if (owner == null)
        {
            throw new InvalidOperationException($"Cannot find View for ownerViewModel of type {ownerViewModel.GetType()}");
        }
        return owner;
    }

    /// <summary>
    /// Creates a new IWindow from the configured IDialogFactory.
    /// </summary>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="viewType">The type of view to show.</param>
    /// <returns>The new IWindow.</returns>
    /// <exception cref="TypeLoadException">Could not load view for view model.</exception>
    protected IView CreateDialog(INotifyPropertyChanged viewModel, Type viewType)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        IView? dialog;
        if (typeof(IView).IsAssignableFrom(viewType))
        {
            dialog = (IView)Activator.CreateInstance(viewType)!;
        }
        else if (typeof(T).IsAssignableFrom(viewType))
        {
            dialog = CreateWrapper(viewModel, viewType);
        }
        else
        {
            throw new TypeLoadException($"Only dialogs of type {typeof(T)} or {typeof(IView)} are supported.");
        }

        dialog.ViewModel = viewModel;
        HandleDialogEvents(viewModel, dialog);
        return dialog;
    }

    /// <summary>
    /// Creates a wrapper around a View.
    /// </summary>
    /// <param name="viewModel">The view model of the View.</param>
    /// <param name="viewType">The data type of the View.</param>
    protected abstract IView CreateWrapper(INotifyPropertyChanged viewModel, Type viewType);

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
    protected virtual void HandleDialogEvents(INotifyPropertyChanged viewModel, IView dialog)
    {
        if (viewModel is ICloseable closable)
        {
            closable.RequestClose += (_, _) => Dispatch(dialog.Close);
        }
        if (viewModel is IActivable activable)
        {
            activable.RequestActivate += (_, _) => Dispatch(dialog.Activate);
        }
        if (viewModel is IViewLoaded loaded)
        {
            dialog.Loaded += (_, _) => loaded.OnLoaded();
        }
        if (viewModel is IViewClosing closing)
        {
            dialog.Closing += (_, e) => Window_Closing(dialog, e, closing);
        }
        if (viewModel is IViewClosed closed)
        {
            dialog.Closed += (_, _) => closed.OnClosed();
        }
    }

    private async void Window_Closing(IView dialog, CancelEventArgs e, IViewClosing closing)
    {
        if (dialog.ClosingConfirmed) { return; }

        // ReSharper disable once MethodHasAsyncOverload
        closing.OnClosing(e);
        if (e.Cancel)
        {
            dialog.IsEnabled = false;

            // caller returns and window stays open
            await Task.Yield();

            await closing.OnClosingAsync(e).ConfigureAwait(true);
            if (!e.Cancel)
            {
                dialog.ClosingConfirmed = true;
                dialog.Close();
            }

            // doesn't matter if it's closed
            dialog.IsEnabled = true;
        }
    }

    /// <inheritdoc />
    public virtual async Task<object?> ShowFrameworkDialogAsync<TSettings>(
        INotifyPropertyChanged? ownerViewModel,
        TSettings settings,
        AppDialogSettingsBase appSettings,
        Func<object?, string>? resultToString = null)
        where TSettings : DialogSettingsBase
    {
        Logger?.LogInformation("Dialog: {Dialog}; Title: {Title}", settings.GetType().Name, settings.Title);

        var result = await await DispatchAsync(
            async () =>
            {
                IView? owner;
                var isDummyOwner = false;
                if (ownerViewModel != null)
                {
                    owner = FindViewByViewModel(ownerViewModel) ??
                                throw new ArgumentException($"No view found with specified ownerViewModel of type {ownerViewModel.GetType()}.");
                }
                else
                {
                    // If no owner is specified, get MainWindow if available, otherwise create a dummy parent window.
                    owner = GetMainWindow();
                    if (owner == null || !owner.IsVisible)
                    {
                        owner = GetDummyWindow();
                        isDummyOwner = true;
                    }
                }
                var result = await DialogFactory.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true);
                if (isDummyOwner)
                {
                    owner!.Close();
                }
                return result;
            }).ConfigureAwait(true);

        Logger?.LogInformation("Dialog: {Dialog}; Result: {Result}", settings.GetType().Name, resultToString != null ? resultToString(result) : result?.ToString());
        return result;
    }

    /// <inheritdoc />
    public abstract IView? FindViewByViewModel(INotifyPropertyChanged viewModel);

    /// <inheritdoc />
    public abstract IView? GetMainWindow();

    /// <inheritdoc />
    public abstract IView? GetDummyWindow();
}
