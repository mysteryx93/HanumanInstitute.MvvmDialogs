namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Closed event.
/// </summary>
public interface IViewClosed
{
    /// <summary>
    /// Occurs when the view is closed.
    /// </summary>
    event EventHandler ViewClosed;

    /// <summary>
    /// Raises the Closed event.
    /// </summary>
    void RaiseViewClosed();
}
