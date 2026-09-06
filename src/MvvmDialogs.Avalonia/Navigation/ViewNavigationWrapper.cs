#pragma warning disable CS1591
#pragma warning disable CS8618
namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Class wrapping an instance of Avalonia <see cref="Window"/> within <see cref="IView"/>.
/// </summary>
/// <seealso cref="IView" />
public class ViewNavigationWrapper : IView
{
    private readonly INavigationManager _navigation;
    private readonly ViewClosingHandler? _closingHandler;

    /// <summary>
    /// Initializes a new instance of the ViewNavigationWrapper class. 
    /// </summary>
    /// <param name="navigationManager">The <see cref="INavigationManager"/> to set.</param>
    /// <param name="closingHandler">A handler for the Closing event. Note that the Closing event is unsupported in this class and we thus support a single listener.</param>
    public ViewNavigationWrapper(INavigationManager navigationManager, ViewClosingHandler? closingHandler)
    {
        _navigation = navigationManager;
        _closingHandler = closingHandler;
    }

    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        ViewModel = viewModel;
        ViewDef = viewDef;
    }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
        Ref = (UserControl)view;
        var viewType = view.GetType();
        ViewDef = new ViewDefinition(viewType, () => Ref ?? Activator.CreateInstance(viewType)!);
    }

    private ViewDefinition ViewDef { get; set; }

    public IView? Owner { get; set; }

    /// <summary>
    /// Gets the UserControl reference held by this class. Stored as a weak reference so the view can be collected.
    /// </summary>
    public UserControl? Ref
    {
        get => _ref != null && _ref.TryGetTarget(out var view) ? view : null;
        private set => _ref = value is null ? null : new WeakReference<UserControl>(value);
    }
    private WeakReference<UserControl>? _ref;

    /// <inheritdoc />
    public object RefObj => Ref!;

    /// <inheritdoc />
    public event EventHandler? Loaded;

    /// <summary>
    /// Unused. Closing is dispatched through the constructor <see cref="ViewClosingHandler"/> so this wrapper supports a single listener.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing;

    /// <inheritdoc />
    public event EventHandler? Closed;

    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel { get; private set; }

    private void RaiseLoaded()
    {
        Loaded?.Invoke(this, EventArgs.Empty);
        if (ViewModel is IViewLoaded vm)
        {
            vm.OnLoaded();
        }
    }

    private void RaiseClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
        if (ViewModel is IViewClosed vm)
        {
            vm.OnClosed();
        }
    }

    /// <inheritdoc />
    public async Task ShowDialogAsync(IView owner)
    {
        var task = _navigation.ShowDialogAsync(ViewModel, ViewDef, owner.ViewModel);
        Ref = _navigation.CurrentView!;
        RaiseLoaded();
        await task.ConfigureAwait(true);
    }

    /// <inheritdoc />
    public void Show(IView? owner)
    {
        _navigation.Show(ViewModel, ViewDef);
        Ref = _navigation.CurrentView!;
        RaiseLoaded();
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (!ReferenceEquals(_navigation.CurrentView?.DataContext, ViewModel))
        {
            if (_navigation.Activate(ViewModel))
            {
                Ref = _navigation.CurrentView!;
                RaiseLoaded();
            }
        }
    }

    /// <inheritdoc />
    public void Close()
    {
        var args = new CancelEventArgs();
        if (!ClosingConfirmed)
        {
            _closingHandler?.Invoke(this, args);
        }
        if (!args.Cancel && ClosingConfirmed)
        {
            _isClosed = true;
            _navigation.Close(ViewModel);
            RaiseClosed();
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

    private bool _isClosed;
}
