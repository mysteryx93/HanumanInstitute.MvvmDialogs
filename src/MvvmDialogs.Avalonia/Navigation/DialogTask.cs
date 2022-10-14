namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Represents a dialog shown with single-page navigation that is awaiting result.
/// </summary>
internal class DialogTask
{
    /// <summary>
    /// Initializes a new instance of the DialogTask.
    /// </summary>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    public DialogTask(INotifyPropertyChanged viewModel, INotifyPropertyChanged ownerViewModel)
    {
        ViewModel = viewModel;
        OwnerViewModel = ownerViewModel;
    }
    
    /// <summary>
    /// Gets the view model of the new dialog.
    /// </summary>
    public INotifyPropertyChanged ViewModel { get; }
    /// <summary>
    /// Gets the view model that represents the owner of the dialog. 
    /// </summary>
    public INotifyPropertyChanged? OwnerViewModel { get; }
    /// <summary>
    /// Gets a <see cref="TaskCompletionSource{TResult}"/> that will notify when the dialog is closed.
    /// </summary>
    public TaskCompletionSource<bool> Completion { get; } = new();
}
