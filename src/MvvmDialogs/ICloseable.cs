using System;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// When implemented by a ViewModel, allows to close the View from the ViewModel by raising the RequestClose event.
/// </summary>
public interface ICloseable
{
    /// <summary>
    /// When raised from the ViewModel, closes the associated view.
    /// </summary>
     event EventHandler? RequestClose;
}
