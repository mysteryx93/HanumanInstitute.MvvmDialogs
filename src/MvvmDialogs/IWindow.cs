using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface describing a generic window.
/// </summary>
/// <remarks>
/// This interface allows cross-platform support, and allows for custom windows
/// not deriving from the standard types.
/// </remarks>
public interface IWindow
{
    /// <summary>
    /// Occurs when the window is closed.
    /// </summary>
    event EventHandler Closed;

    /// <summary>
    /// Gets or sets the data context for an element when it participates in data binding.
    /// </summary>
    object? DataContext { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IWindow"/> that owns this <see cref="IWindow"/>.
    /// </summary>
    IWindow? Owner { get; set; }

    /// <summary>
    /// Opens a window and returns without waiting for the newly opened window to close.
    /// </summary>
    void Show();

    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <returns>
    /// A <see cref="Nullable{Boolean}"/> value that specifies whether the activity was accepted (true) or canceled (false).</returns>
    Task<bool?> ShowDialogAsync();

    /// <summary>
    /// Tries to activate the Window.
    /// </summary>
    void Activate();

    /// <summary>
    /// Tries to close the Window.
    /// </summary>
    void Close();
}