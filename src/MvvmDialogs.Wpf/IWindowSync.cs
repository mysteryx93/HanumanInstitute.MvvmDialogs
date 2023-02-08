
namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Provides a sync ShowDialog compatibility method to IWindow in WPF.
/// </summary>
public interface IViewSync
{
    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    void ShowDialog(IView owner);
}
