namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// A view model representing a modal dialog opened using <see cref="DialogServiceBase"/>.
/// </summary>
public interface IModalDialogViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the dialog result value, which is the value that is returned from the
    /// <see cref="DialogServiceBase.ShowDialogAsync"/> and <see cref="DialogServiceBase.ShowDialogAsync{T}"/>
    /// methods.
    /// </summary>
    bool? DialogResult { get; }
}
