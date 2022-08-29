using System.Windows.Forms;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class wrapping an instance of WPF <see cref="Window"/> within <see cref="IWindow"/>.
/// </summary>
/// <seealso cref="IWindow" />
public class WindowWrapper : IWindow, IWindowSync
{
    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public Window Ref { get; private set; }

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public object RefObj => Ref;

    /// <summary>
    /// Returns a IWin32Window class that can be used for API calls.
    /// </summary>
    public IWin32Window Win32Window => new Win32Window(Ref);

    /// <summary>
    /// Occurs when the window is loading.
    /// </summary>
    public event EventHandler? Loaded
    {
        add
        {
            if (value != null)
            {
                var handler = new RoutedEventHandler((s, e) => value.Invoke(s, e));
                _loadedHandlers.Add(value, handler);
                Ref.Loaded += handler;
            }
        }
        remove
        {
            if (value != null)
            {
                Ref.Loaded += _loadedHandlers[value];
                _loadedHandlers.Remove(value);
            }
        }
    }
    private Dictionary<EventHandler, RoutedEventHandler> _loadedHandlers = new();

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing
    {
        add
        {
            if (value != null)
            {
                var handler = new CancelEventHandler((s, e) => value.Invoke(s, e));
                _closingHandlers.Add(value, handler);
                Ref.Closing += handler;
            }
        }
        remove
        {
            if (value != null)
            {
                Ref.Closing += _closingHandlers[value];
                _closingHandlers.Remove(value);
            }
        }
    }
    private Dictionary<EventHandler<CancelEventArgs>, CancelEventHandler> _closingHandlers = new();

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler? Closed
    {
        add => Ref.Closed += value;
        remove => Ref.Closed -= value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowWrapper"/> class.
    /// </summary>
    /// <param name="window">The window.</param>
    public WindowWrapper(Window window) =>
        this.Ref = window ?? throw new ArgumentNullException(nameof(window));

    /// <inheritdoc />
    public object? DataContext
    {
        get => Ref.DataContext;
        set => Ref.DataContext = value;
    }

    /// <inheritdoc />
    public IWindow? Owner
    {
        get => Ref.Owner.AsWrapper();
        set =>
            Ref.Owner = value switch
            {
                null => null,
                WindowWrapper w => w.Ref,
                _ => throw new ArgumentException($"Owner must be of type {typeof(WindowWrapper).FullName}")
            };
    }

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync() => Ref.RunUiAsync(ShowDialog);

    /// <inheritdoc />
    public bool? ShowDialog() => Ref.ShowDialog();

    /// <inheritdoc />
    public void Show() => Ref.Show();

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
    public bool ClosingConfirmed { get; set; }
}
