namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, allows to activate the View from the ViewModel by raising the RequestActivate event.
/// </summary>
public interface IActivable
{
    /// <summary>
    /// When raised from the ViewModel, activates the associated view.
    /// </summary>
     event EventHandler? RequestActivate;
}
