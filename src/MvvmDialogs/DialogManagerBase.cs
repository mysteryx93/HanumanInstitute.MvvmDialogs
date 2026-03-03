using System.Threading;
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

    /// <inheritdoc />
    public ILogger<IDialogManager>? Logger { get; }

    /// <inheritdoc />
    public bool AllowConcurrentDialogs { get; set; }

    /// <summary>
    /// Returns whether the code is running in design mode.
    /// </summary>
    protected abstract bool IsDesignMode { get; }

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
        DialogFactory.DialogManager = this;
        Logger = logger;
    }

    /// <summary>
    /// Gets or sets whether to raise ViewModel view events when dialog events occur.
    /// Must be set to False for view navigation because it would re-trigger the same event in loop.
    /// </summary>
    protected bool ForwardViewEvents { get; set; } = true;

    /// <inheritdoc />
    public virtual void Show(INotifyPropertyChanged? ownerViewModel, INotifyPropertyChanged viewModel)
    {
        if (IsDesignMode) { return; }
        Dispatch(
            () =>
            {
                var viewDef = ViewLocator.Locate(viewModel);
                Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", viewDef.ViewType, viewModel.GetType(), ownerViewModel?.GetType());

                var dialog = CreateDialog(viewModel, viewDef);
                dialog.Show(FindViewByViewModelOrThrow(ownerViewModel));
            });
    }

    /// <inheritdoc />
    public virtual async Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
    {
        if (IsDesignMode) { return; }
        await await DispatchAsync(
            async () =>
            {
                var viewDef = ViewLocator.Locate(viewModel);
                Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", viewDef.ViewType, viewModel.GetType(), ownerViewModel.GetType());

                var dialog = CreateDialog(viewModel, viewDef);
                await dialog.ShowDialogAsync(FindViewByViewModelOrThrow(ownerViewModel)!);

                Logger?.LogInformation("View: {View}; Result: {Result}", viewDef.ViewType, viewModel.DialogResult);
            });
    }

    /// <summary>
    /// Returns the IView with a DataContext equal to specified ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel to search for.</param>
    /// <returns>A IView, or null.</returns>
    /// <exception cref="InvalidOperationException">Cannot find View for viewModel type.</exception>
    protected IView? FindViewByViewModelOrThrow(INotifyPropertyChanged? viewModel)
    {
        if (viewModel == null) { return null; }
        var owner = FindViewByViewModel(viewModel);
        if (owner == null)
        {
            throw new InvalidOperationException($"Cannot find View for viewModel of type {viewModel.GetType()}");
        }
        return owner;
    }

    /// <summary>
    /// Creates a new IWindow from the configured IDialogFactory.
    /// </summary>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    /// <returns>The new IWindow.</returns>
    /// <exception cref="TypeLoadException">Could not load view for view model.</exception>
    protected IView CreateDialog(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        IView? dialog;
        if (viewDef.TypeDerivesFrom<IView>())
        {
            dialog = (IView)viewDef.Create();
            dialog.Initialize(viewModel, viewDef);
        }
        else if (viewDef.TypeDerivesFrom<T>())
        {
            dialog = CreateWrapper(viewModel, viewDef);
        }
        else
        {
            throw new TypeLoadException($"Only dialogs of type {typeof(T)} or {typeof(IView)} are supported.");
        }

        HandleDialogEvents(viewModel, dialog);
        return dialog;
    }

    /// <summary>
    /// Creates a wrapper around a View.
    /// </summary>
    /// <param name="viewModel">The view model of the View.</param>
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    protected abstract IView CreateWrapper(INotifyPropertyChanged viewModel, ViewDefinition viewDef);

    /// <summary>
    /// Creates a wrapper around a View.
    /// </summary>
    /// <param name="view">The view to create a wrapper for.</param>
    protected abstract IView AsWrapper(T view);

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
    public virtual void HandleDialogEvents(INotifyPropertyChanged viewModel, IView dialog)
    {
        if (viewModel is ICloseable closable)
        {
            closable.RequestClose += (_, _) => Dispatch(dialog.Close);
        }
        if (viewModel is IActivable activable)
        {
            activable.RequestActivate += (_, _) => Dispatch(dialog.Activate);
        }
        if (viewModel is IViewClosing)
        {
            dialog.Closing += (_, e) => View_Closing(dialog, e);
        }
        if (ForwardViewEvents)
        {
            if (viewModel is IViewLoaded loaded)
            {
                dialog.Loaded += (_, _) => loaded.OnLoaded();
            }
            if (viewModel is IViewClosed closed)
            {
                dialog.Closed += (_, _) => closed.OnClosed();
            }
        }
    }

    private bool _isViewClosing;

    public async void View_Closing(IView dialog, CancelEventArgs e)
    {
        if (dialog.ViewModel is not IViewClosing closing || dialog.ClosingConfirmed)
        {
            dialog.ClosingConfirmed = true;
            return;
        }
        // Prevent re-closing while prompting. 
        if (_isViewClosing)
        {
            e.Cancel = true;
            return;
        }

        _isViewClosing = true;
        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            closing.OnClosing(e);
            if (e.Cancel)
            {
                // caller returns and window stays open
                await Task.Yield();

                await closing.OnClosingAsync(e).ConfigureAwait(true);
                if (!e.Cancel)
                {
                    dialog.ClosingConfirmed = true;
                    dialog.Close();
                }
            }
            else
            {
                dialog.ClosingConfirmed = true;
            }
        }
        finally
        {
            _isViewClosing = false;
        }
    }

    private readonly SemaphoreSlim _semaphoreShow = new(1);

    /// <inheritdoc />
    public virtual async Task<object?> ShowFrameworkDialogAsync<TSettings>(
        INotifyPropertyChanged? ownerViewModel,
        TSettings settings,
        Func<object?, string>? resultToString = null)
        where TSettings : DialogSettingsBase
    {
        if (IsDesignMode) { return null; }

        if (!AllowConcurrentDialogs)
        {
            await _semaphoreShow.WaitAsync();
        }

        try
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

                    var result = await DialogFactory.ShowDialogAsync(owner, settings).ConfigureAwait(true);
                    if (isDummyOwner)
                    {
                        owner!.Close();
                    }
                    return result;
                }).ConfigureAwait(true);

            Logger?.LogInformation("Dialog: {Dialog}; Result: {Result}", settings.GetType().Name, resultToString != null ? resultToString(result) : result?.ToString());
            return result;
        }
        finally
        {
            if (!AllowConcurrentDialogs)
            {
                _semaphoreShow.Release();
            }
        }
    }

    /// <inheritdoc />
    public abstract IView? FindViewByViewModel(INotifyPropertyChanged viewModel);

    /// <inheritdoc />
    public abstract IView? GetMainWindow();

    /// <inheritdoc />
    public abstract IView? GetDummyWindow();
}
