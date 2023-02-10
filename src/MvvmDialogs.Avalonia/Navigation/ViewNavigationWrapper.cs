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

    /// <summary>
    /// Initializes a new instance of the ViewNavigationWrapper class. 
    /// </summary>
    /// <param name="navigationManager">The <see cref="INavigationManager"/> to set.</param>
    public ViewNavigationWrapper(INavigationManager navigationManager)
    {
        _navigation = navigationManager;
    }

    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, Type viewType)
    {
        ViewModel = viewModel;
        ViewType = viewType;
    }
    
    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
        ViewType = view.GetType();
        Ref = (UserControl)view;
    }
      
    // /// <summary>
    // /// Sets the <see cref="INavigationManager"/> associated with this wrapper. Must be called before use. 
    // /// </summary>
    // /// <param name="navigationManager">The <see cref="INavigationManager"/> to set.</param>
    // /// <returns>Returns this class instance.</returns>
    // public ViewNavigationWrapper SetNavigation(INavigationManager navigationManager)
    // {
    //     _navigation = navigationManager;
    //     return this;
    // }
    
    public Type ViewType { get; set; }

    public IView? Owner { get; set; }

    /// <summary>
    /// Gets the UserControl reference held by this class.
    /// </summary>
    public UserControl? Ref { get; private set; }

    /// <inheritdoc />
    public object RefObj => Ref!;
    
    /// <inheritdoc />
    public event EventHandler? Loaded;
    
    // /// <summary>
    // /// Invokes the <see cref="Loaded" /> event.
    // /// </summary>
    // internal void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
    
    /// <inheritdoc />
    public event EventHandler<CancelEventArgs>? Closing;
    
    // /// <summary>
    // /// Invokes the <see cref="Closing" /> event.
    // /// </summary>
    // internal CancelEventArgs RaiseClosing()
    // {
    //     var args = new CancelEventArgs();
    //     Closing?.Invoke(this, args);
    //     return args;
    // }
    
    /// <inheritdoc />
    public event EventHandler? Closed;
    
    // /// <summary>
    // /// Invokes the <see cref="Closed" /> event.
    // /// </summary>
    // internal void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);
   
    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel { get; private set; }

    /// <inheritdoc />
    public async Task ShowDialogAsync(IView owner)
    {
        var task = _navigation.ShowDialogAsync(ViewModel, ViewType, owner.ViewModel);
        Ref = _navigation.CurrentView!;
        Loaded?.Invoke(this, EventArgs.Empty);
        await task.ConfigureAwait(true);
    }

    /// <inheritdoc />
    public void Show(IView? owner)
    {
        _navigation.Show(ViewModel, ViewType);  
        Ref = _navigation.CurrentView!;
        Loaded?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (!ReferenceEquals(_navigation.CurrentView?.DataContext, ViewModel))
        {
            if (_navigation.Activate(ViewModel, ViewType))
            {
                Ref = _navigation.CurrentView!;
                Loaded?.Invoke(this, EventArgs.Empty);
            }
        } 
    }

    /// <inheritdoc />
    public void Close()
    {
        _navigation.Close(ViewModel, ViewType);
        var args = new CancelEventArgs();
        Closing?.Invoke(this, args);
        if (!args.Cancel)
        {
            Closed?.Invoke(this, EventArgs.Empty);
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
    public bool IsVisible => Ref != null && object.ReferenceEquals(Ref, _navigation.CurrentView);
    
    /// <inheritdoc />    
    public bool ClosingConfirmed { get; set; }
}
