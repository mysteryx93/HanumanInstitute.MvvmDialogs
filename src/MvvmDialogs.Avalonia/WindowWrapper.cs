namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Class wrapping an instance of Avalonia <see cref="Window"/> within <see cref="IWindow"/>.
/// </summary>
/// <seealso cref="IWindow" />
public class WindowWrapper : IWindow
{
    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public Window Ref { get; private set; }

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public object RefObj => Ref;

    /// <inheritdoc />
    public IWindow? Owner { get; set; }

    /// <summary>
    /// Fired when the window is closed.
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
    public WindowWrapper(Window window)
    {
        Ref = window ?? throw new ArgumentNullException(nameof(window));
        Owner = window.Owner is Window w ? w.AsWrapper() : null;
    }

    /// <inheritdoc />
    public object? DataContext
    {
        get => Ref.DataContext;
        set => Ref.DataContext = value;
    }

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync()
    {
        if (Owner is not WindowWrapper w) throw new InvalidOperationException($"{nameof(Owner)} must be set before calling {nameof(ShowDialogAsync)}");

        return Ref.ShowDialog<bool?>(w.Ref);
    }

    /// <inheritdoc />
    public void Show() => Ref.Show();

    /// <inheritdoc />
    public void Activate() => Ref.Activate();

    /// <inheritdoc />
    public void Close() => Ref.Close();
}
