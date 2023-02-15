namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Closing event.
/// </summary>
public interface IViewClosing
{
    /// <summary>
    /// Called when the view is closing. If e.Cancel is set to true, <see cref="OnClosingAsync"/> will be called.
    /// </summary>
    void OnClosing(CancelEventArgs e);

    /// <summary>
    /// Called when e.Cancel is set to true in <see cref="OnClosing"/>. If e.Cancel is set back to false, the window will be closed.
    /// </summary>
    Task OnClosingAsync(CancelEventArgs e);
}
