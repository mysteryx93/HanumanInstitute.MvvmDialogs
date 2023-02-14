namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Loaded event.
/// </summary>
public interface IViewLoaded
{
    /// <summary>
    /// Occurs when the view is loaded.
    /// </summary>
    event EventHandler ViewLoaded;

    /// <summary>
    /// Raises the Loaded event.
    /// </summary>
    void RaiseViewLoaded();
}
