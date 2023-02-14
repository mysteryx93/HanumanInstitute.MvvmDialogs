namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Closing event.
/// </summary>
public interface IViewClosing
{
    /// <summary>
    /// Occurs when the view is closing. If e.Cancel is set to true, <see cref="OnViewClosingAsync"/> will be called.
    /// </summary>
    event EventHandler<CancelEventArgs> ViewClosing;

    /// <summary>
    /// Raises the Closing event.
    /// </summary>
    void RaiseViewClosing(CancelEventArgs e);

    /// <summary>
    /// Called when e.Cancel is set to true in <see cref="ViewClosing"/>. If e.Cancel is set back to false, the window will be closed.
    /// </summary>
    Task OnViewClosingAsync(CancelEventArgs e);
}
