namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Closing event.
/// </summary>
public interface IViewClosing
{
    /// <summary>
    /// Called when the view is closing. If e.Cancel is set to true, <see cref="ViewClosingAsync"/> will be called.
    /// </summary>
    void ViewClosing(CancelEventArgs e);
    
    /// <summary>
    /// Called when e.Cancel is set to true in <see cref="ViewClosing"/>. If e.Cancel is set back to false, the window will be closed.
    /// </summary>
    Task ViewClosingAsync(CancelEventArgs e);
}
