namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface describing a generic window.
/// </summary>
/// <remarks>
/// This interface allows cross-platform support, and allows for custom windows
/// not deriving from the standard types.
/// </remarks>
public interface IView
{
    /// <summary>
    /// Initializes the <see cref="IView"/> from specified view model and view type.
    /// </summary>
    /// <param name="viewModel">The view model to display.</param>
    /// <param name="viewType">The data type of the view.</param>
    void Initialize(INotifyPropertyChanged viewModel, Type viewType);

    /// <summary>
    /// Initializes the <see cref="IView"/> from specified view model and existing view.
    /// </summary>
    /// <param name="viewModel">The view model to display.</param>
    /// <param name="view">The view to display.</param>
    void InitializeExisting(INotifyPropertyChanged viewModel, object view);

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public object RefObj { get; }

    /// <summary>
    /// Occurs when the window is loaded.
    /// </summary>
    event EventHandler Loaded;

    /// <summary>
    /// Occurs when the window is closing.
    /// </summary>
    event EventHandler<CancelEventArgs> Closing;

    /// <summary>
    /// Occurs when the window is closed.
    /// </summary>
    event EventHandler Closed;

    /// <summary>
    /// Gets or sets the data context for an element when it participates in data binding.
    /// </summary>
    INotifyPropertyChanged ViewModel { set; }

    /// <summary>
    /// Opens a window and returns without waiting for the newly opened window to close.
    /// </summary>
    void Show(IView? owner);

    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <returns>
    /// A <see cref="Nullable{Boolean}"/> value that specifies whether the activity was accepted (true) or canceled (false).</returns>
    Task<bool?> ShowDialogAsync(IView owner);

    /// <summary>
    /// Tries to activate the Window.
    /// </summary>
    void Activate();

    /// <summary>
    /// Tries to close the Window.
    /// </summary>
    void Close();

    /// <summary>
    /// Gets or sets whether the window is enabled.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets whether the window is visible.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Gets or sets whether closing has been confirmed, in which case Closing event should be ignored. 
    /// </summary>
    bool ClosingConfirmed { get; set; }
}
