using System;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Provides a sync ShowDialog compatibility method to IWindow in WPF.
/// </summary>
public interface IWindowSync
{
    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <returns>
    /// A <see cref="Nullable{T}"/> value that specifies whether the activity was accepted (true) or canceled (false).</returns>
    bool? ShowDialog();
}
