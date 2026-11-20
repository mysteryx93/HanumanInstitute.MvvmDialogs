// ReSharper disable VirtualMemberCallInConstructor

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Class wrapping an instance of Avalonia <see cref="Window"/> within <see cref="IView"/>.
/// </summary>
/// <seealso cref="IView" />
public class ViewWrapper : IView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewWrapper"/> class.
    /// </summary>
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        Ref = (Window)viewDef.Create();
        ViewModel = viewModel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewWrapper"/> class.
    /// </summary>
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        Ref = (Window)view;
        ViewModel = viewModel;
    }

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public Window Ref { get; private set; } = null!;

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public object RefObj => Ref;

    /// <summary>
    /// Fired when the window is loaded.
    /// </summary>
    public event EventHandler? Loaded
    {
        add => Ref.Opened += value;
        remove => Ref.Opened -= value;
    }

    /// <summary>
    /// Fired when the window is closing.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing
    {
        add
        {
            if (value == null) { return; }
            if (_closing == null) { Ref.Closing += WindowClosing; }
            _closing += value;
        }
        remove
        {
            _closing -= value;
            if (_closing == null) { Ref.Closing -= WindowClosing; }
        }
    }

    private EventHandler<CancelEventArgs>? _closing;

    // Keep a retry approved until every wrapper subscriber has seen the native close attempt.
    private void WindowClosing(object? sender, WindowClosingEventArgs e)
    {
        try { _closing?.Invoke(sender, e); }
        finally { ClosingConfirmed = false; }
    }

    /// <summary>
    /// Fired when the window is closed.
    /// </summary>
    public event EventHandler? Closed
    {
        add
        {
            if (value == null) { return; }
            if (_closed == null) { Ref.Closed += WindowClosed; }
            _closed += value;
        }
        remove
        {
            _closed -= value;
            if (_closed == null) { Ref.Closed -= WindowClosed; }
        }
    }

    private EventHandler? _closed;

    // A callback failure must leave Avalonia free to finish closing and release the window.
    private void WindowClosed(object? sender, EventArgs e)
    {
        if (_closed == null) { return; }

        foreach (EventHandler handler in _closed.GetInvocationList())
        {
            try { handler(sender, e); }
            catch (Exception error) { Dispatcher.UIThread.Post(ExceptionDispatchInfo.Capture(error).Throw); }
        }
    }

    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)Ref.DataContext!;
        set => Ref.DataContext = value;
    }

    /// <inheritdoc />
    public Task ShowDialogAsync(IView owner)
    {
        var window = (Window)owner.RefObj;
        SetMainWindowIfEmpty(window);
        return Ref.ShowDialog<bool?>(window);
    }

    /// <inheritdoc />
    public void Show(IView? owner)
    {
        var own = owner?.RefObj as Window;
        SetMainWindowIfEmpty(Ref);
        if (own != null)
        {
            Ref.Show(own);
        }
        else
        {
            Ref.Show();
        }
    }

    /// <inheritdoc />
    public void Activate() => Ref.Activate();

    /// <inheritdoc />
    public void Close() => Ref.Close();

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

    private void SetMainWindowIfEmpty(Window? window)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow == null)
        {
            desktop.MainWindow = window;
        }
    }
}
