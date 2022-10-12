#pragma warning disable CS1591
#pragma warning disable CS8618
namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Class wrapping an instance of Avalonia <see cref="Window"/> within <see cref="IView"/>.
/// </summary>
/// <seealso cref="IView" />
public class ViewNavigationWrapper : IView
{
    private NavigationManager _navigation = default!;

    public void Initialize(INotifyPropertyChanged viewModel, Type viewType)
    {
        ViewModel = viewModel;
        ViewType = viewType;
    }
    
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
        ViewType = view.GetType();
    }
        
    public ViewNavigationWrapper SetNavigation(NavigationManager navigationManager)
    {
        _navigation = navigationManager;
        return this;
    }
    
    public Type ViewType { get; set; }
    
    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public UserControl Ref { get; private set; }

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public object RefObj => Ref;
    
    /// <inheritdoc />
    public IView? Owner { get; set; }
    
    /// <summary>
    /// Fired when the window is loaded.
    /// </summary>
    public event EventHandler? Loaded;
    
    internal void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
    
    /// <summary>
    /// Fired when the window is closing.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing;
    
    internal CancelEventArgs RaiseClosing()
    {
        var args = new CancelEventArgs();
        Closing?.Invoke(this, args);
        return args;
    }
    
    /// <summary>
    /// Fired when the window is closed.
    /// </summary>
    public event EventHandler? Closed;
    
    internal void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);
   
    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)Ref.DataContext!;
        set => Ref.DataContext = value;
    }
    
    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync(IView owner) => _navigation.ShowDialogAsync(ViewModel, ViewType);
    
    /// <inheritdoc />
    public void Show(IView? owner) => _navigation.Show(ViewModel, ViewType);
    
    /// <inheritdoc />
    public void Activate() => _navigation.Show(ViewModel, ViewType);
    
    /// <inheritdoc />
    public void Close() => _navigation.Close(ViewModel, ViewType);
    
    /// <inheritdoc />
    public bool IsEnabled
    {
        get => Ref.IsEnabled;
        set => Ref.IsEnabled = value;
    }
    
    /// <inheritdoc />
    public bool IsVisible => Ref.IsVisible;
    
    /// <inheritdoc />    
    public bool ClosingConfirmed { get; set; }
}
