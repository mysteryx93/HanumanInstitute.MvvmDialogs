namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Closed event.
/// </summary>
public interface IViewClosed
{
    /// <summary>
    /// Called when the view is closed.
    /// </summary>
    void OnClosed();
}
