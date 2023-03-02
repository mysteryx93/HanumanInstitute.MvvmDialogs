using System.Windows.Forms;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class wrapping an instance of WPF <see cref="Window"/> within <see cref="IView"/>.
/// </summary>
/// <seealso cref="IView" />
public class ViewWrapper : IView, IViewSync
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewWrapper"/> class.
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="viewDef"></param>
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
    public Window Ref { get; private set; } = default!;

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
                var handler = new RoutedEventHandler(value.Invoke);
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
    private readonly Dictionary<EventHandler, RoutedEventHandler> _loadedHandlers = new();

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Closing
    {
        add
        {
            if (value != null)
            {
                var handler = new CancelEventHandler(value.Invoke);
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
    private readonly Dictionary<EventHandler<CancelEventArgs>, CancelEventHandler> _closingHandlers = new();

    /// <summary>
    /// Occurs when the window is about to close.
    /// </summary>
    public event EventHandler? Closed
    {
        add => Ref.Closed += value;
        remove => Ref.Closed -= value;
    }

    ///// <summary>
    ///// Initializes a new instance of the <see cref="ViewWrapper"/> class.
    ///// </summary>
    ///// <param name="window">The window.</param>
    //public ViewWrapper(Window window) =>
    //    this.Ref = window ?? throw new ArgumentNullException(nameof(window));

    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)Ref.DataContext;
        set => Ref.DataContext = value;
    }

    /// <inheritdoc />
    public IView? Owner
    {
        get => Ref.Owner.AsWrapper();
        set =>
            Ref.Owner = value switch
            {
                null => null,
                ViewWrapper w => w.Ref,
                _ => throw new ArgumentException($"Owner must be of type {typeof(ViewWrapper).FullName}")
            };
    }

    /// <inheritdoc />
    public Task ShowDialogAsync(IView? owner) => UiExtensions.RunUiAsync(() => ShowDialog(owner));

    /// <inheritdoc />
    public void ShowDialog(IView? owner)
    {
        Ref.Owner = owner?.RefObj as Window;
        Ref.ShowDialog();
    }

    /// <inheritdoc />
    public void Show(IView? owner)
    {
        Ref.Owner = owner?.RefObj as Window;
        Ref.Show();
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
}
