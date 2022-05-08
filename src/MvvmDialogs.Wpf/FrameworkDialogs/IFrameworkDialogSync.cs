
namespace HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
/// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
public interface IFrameworkDialogSync<TResult>
{
    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>Return data specific to the dialog.</returns>
    TResult ShowDialog(IWindow owner);
}
