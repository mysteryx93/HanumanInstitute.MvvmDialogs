#pragma warning disable CS1591
#pragma warning disable CS8618
namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Class wrapping an instance of Avalonia <see cref="UserControl"/> within <see cref="IView"/>.
/// </summary>
/// <seealso cref="IView" />
public class ViewNavigationWrapper : IView
{
    private readonly INavigationManager _navigation;
    internal ViewClosingHandler? ClosingHandler { get; }

    /// <summary>
    /// Initializes a new instance of the ViewNavigationWrapper class. 
    /// </summary>
    /// <param name="navigationManager">The <see cref="INavigationManager"/> to set.</param>
    /// <param name="closingHandler">A handler for the Closing event. Note that the Closing event is unsupported in this class and we thus support a single listener.</param>
    public ViewNavigationWrapper(INavigationManager navigationManager, ViewClosingHandler? closingHandler)
    {
        _navigation = navigationManager;
        ClosingHandler = closingHandler;
    }

    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        Ref = null;
        ViewModel = viewModel;
        ViewDef = viewDef;
    }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
        Ref = (UserControl)view;
        var viewType = view.GetType();
        var supplied = _ref!;
        ViewDef = new(viewType, () => supplied.TryGetTarget(out var target) ? target : Activator.CreateInstance(viewType)!);
    }

    private ViewDefinition ViewDef { get; set; }

    // Gets the type of the live control or the type recorded in its view definition.
    internal Type ViewType => Ref?.GetType() ?? ViewDef.ViewType;

    internal TaskCompletionSource<bool>? Completion { get; set; }
    internal Task<bool>? PendingClose { get; set; }
    internal bool IsConfirming { get; private set; }

    internal UserControl Realize(UserControl? reusable = null)
    {
        var view = Ref;
        if (view == null)
        {
            view = reusable != null && reusable.GetType() == ViewDef.ViewType ? reusable : (UserControl)ViewDef.Create();
            Ref = view;
        }
        view.DataContext = ViewModel;
        return view;
    }

    internal Control? GetOverlayOwner() => IsConfirming ? _navigation.CurrentView : Ref;

    public IView? Owner { get; set; }

    /// <summary>
    /// Gets the UserControl reference held by this class. Stored as a weak reference so the view can be collected.
    /// </summary>
    public UserControl? Ref
    {
        get => _ref != null && _ref.TryGetTarget(out var view) ? view : null;
        internal set => _ref = value is null ? null : new WeakReference<UserControl>(value);
    }
    private WeakReference<UserControl>? _ref;

    /// <inheritdoc />
    public object RefObj => Ref ??
                            (_navigation is NavigationManager manager && manager.IsOpen(this) ? Realize() : null)!;

    /// <inheritdoc />
    public event EventHandler? Loaded;

    /// <summary>
    /// Unused. Closing is dispatched through the constructor <see cref="ViewClosingHandler"/> so this wrapper supports a single listener.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing { add { } remove { } }

    /// <inheritdoc />
    public event EventHandler? Closed;

    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel { get; private set; }

    internal void NotifyLoaded()
    {
        Loaded?.Invoke(this, EventArgs.Empty);
        if (ViewModel is IViewLoaded vm)
        {
            vm.OnLoaded();
        }
    }

    internal void NotifyClosed(bool notifyModel = true)
    {
        Closed?.Invoke(this, EventArgs.Empty);
        if (notifyModel && ViewModel is IViewClosed vm)
        {
            vm.OnClosed();
        }
    }

    /// <inheritdoc />
    public async Task ShowDialogAsync(IView owner)
    {
        if (_navigation is NavigationManager manager)
        {
            await manager.Present(this, owner).ConfigureAwait(true);
            return;
        }
        var task = _navigation.ShowDialogAsync(ViewModel, ViewDef, owner.ViewModel!);
        Ref = _navigation.CurrentView!;
        NotifyLoaded();
        await task.ConfigureAwait(true);
    }

    /// <inheritdoc />
    public void Show(IView? owner)
    {
        if (_navigation is NavigationManager manager)
        {
            manager.Present(this, null);
            return;
        }
        _navigation.Show(ViewModel, ViewDef);
        Ref = _navigation.CurrentView!;
        NotifyLoaded();
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (!ReferenceEquals(_navigation.CurrentView?.DataContext, ViewModel))
        {
            if (_navigation.Activate(ViewModel) && _navigation is not NavigationManager)
            {
                Ref = _navigation.CurrentView!;
                NotifyLoaded();
            }
        }
    }

    /// <inheritdoc />
    public void Close()
    {
        var close = CloseAsync();
        if (close.IsCompleted) { close.GetAwaiter().GetResult(); }
        else { AwaitClose(close); }

        static async void AwaitClose(Task pending) => await pending.ConfigureAwait(true);
    }

    private async Task CloseAsync()
    {
        if (_navigation is NavigationManager manager)
        {
            await manager.RequestCloseAsync(this).ConfigureAwait(true);
        }
        else if (await ConfirmAsync().ConfigureAwait(true))
        {
            _navigation.Close(ViewModel);
            NotifyClosed();
        }
    }

    internal async Task<bool> ConfirmAsync()
    {
        var args = new CancelEventArgs();
        IsConfirming = true;
        try
        {
            if (ClosingHandler?.Target is DialogManager manager && ClosingHandler == manager.View_Closing)
            {
                await manager.ConfirmNavigationCloseAsync(this, args).ConfigureAwait(true);
            }
            else if (ClosingHandler != null && !ClosingConfirmed) { ClosingHandler(this, args); }
            else { ClosingConfirmed = true; }
            return !args.Cancel && ClosingConfirmed;
        }
        finally
        {
            IsConfirming = false;
            ClosingConfirmed = false;
        }
    }

    /// <inheritdoc />
    public bool IsEnabled
    {
        get => Ref?.IsEnabled ?? true;
        set
        {
            if (Ref != null)
            {
                Ref.IsEnabled = value;
            }
        }
    }

    /// <inheritdoc />
    public bool IsVisible => Ref != null && ReferenceEquals(Ref, _navigation.CurrentView);

    /// <inheritdoc />    
    public bool ClosingConfirmed { get; set; }
}
