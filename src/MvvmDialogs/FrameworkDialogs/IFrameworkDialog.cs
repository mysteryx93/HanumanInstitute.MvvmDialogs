namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
/// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
public interface IFrameworkDialog<TResult>
{
    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>Return data specific to the dialog.</returns>
    Task<TResult> ShowDialogAsync(IWindow owner);
}
