namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, handles the View Loaded event.
/// </summary>
public interface IViewLoaded
{
    /// <summary>
    /// Called when the view is loaded.
    /// </summary>
    void OnLoaded();
}
