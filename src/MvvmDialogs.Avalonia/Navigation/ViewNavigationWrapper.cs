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
    /// <param name="closingHandler">A handler for the Closing event. Not that the Closing event is unsupported in this class and we thus support a single listener.</param>
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
        ViewDef = new ViewDefinition(view.GetType(), () => (UserControl)view);
        Ref = (UserControl)view;
    }
    
    private ViewDefinition ViewDef { get; set; }
    
    public IView? Owner { get; set; }

    /// <summary>
    /// Gets the UserControl reference held by this class.
    /// </summary>
    public UserControl? Ref { get; private set; }

    /// <inheritdoc />
    public object RefObj => Ref!;

    /// <summary>
    /// Unused event.
    /// </summary>
    public event EventHandler? Loaded;

    /// <summary>
    /// Unused event.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing;
    
    /// <summary>
    /// Unused event.
    /// </summary>
    public event EventHandler? Closed;
    
    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel { get; private set; }

    private void RaiseLoaded()
    {
        if (ViewModel is IViewLoaded vm)
        {
            vm.OnLoaded();
        }
    }

    private void RaiseClosed()
    {
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
        if (!args.Cancel)
        {
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
}
