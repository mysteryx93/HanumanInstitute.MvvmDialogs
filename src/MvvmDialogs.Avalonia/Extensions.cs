using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets the owner of a <see cref="StyledElement"/> wrapped in a <see cref="WindowWrapper"/>.
    /// </summary>
    /// <param name="frameworkElement">
    /// The <see cref="StyledElement"/> to find the <see cref="WindowWrapper"/> for.
    /// </param>
    /// <returns>The owning <see cref="WindowWrapper"/> if found; otherwise null.</returns>
    internal static WindowWrapper? GetOwner(this StyledElement frameworkElement)
    {
        var owner = frameworkElement as Window ?? frameworkElement.FindLogicalAncestorOfType<Window>();
        return owner.AsWrapper();
    }

    /// <summary>
    /// Creates a WindowWrapper around specified window.
    /// </summary>
    /// <param name="window">The Window to get a wrapper for.</param>
    /// <returns>A WindowWrapper referencing the window.</returns>
    [return: NotNullIfNotNull("window")]
    public static WindowWrapper? AsWrapper(this Window? window) =>
        window != null ? new WindowWrapper(window) : null;

    /// <summary>
    /// Converts an IWindow into a WindowWrapper.
    /// </summary>
    /// <param name="window">The IWindow to convert.</param>
    /// <returns>A WindowWrapper referencing the window.</returns>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("window")]
    public static WindowWrapper? AsWrapper(this IWindow? window) =>
        (WindowWrapper?)window;

    /// <summary>
    /// Runs a synchronous action asynchronously on the UI thread.
    /// </summary>
    /// <param name="window">Any window to get the dispatcher from.</param>
    /// <param name="action">The action to run asynchronously.</param>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <returns>The result of the action.</returns>
    public static Task<T> RunUiAsync<T>(this Window window, Func<T> action)
    {
        if (window == null) throw new ArgumentNullException(nameof(window));
        TaskCompletionSource<T> completion = new();
        Dispatcher.UIThread.Post(new Action(() => completion.SetResult(action())));
        return completion.Task;
    }
}
