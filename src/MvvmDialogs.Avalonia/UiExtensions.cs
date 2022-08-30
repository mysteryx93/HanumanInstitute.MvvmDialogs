using System.Diagnostics.CodeAnalysis;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Extension methods.
/// </summary>
public static class UiExtensions
{
    /// <summary>
    /// Gets the owner of a <see cref="StyledElement"/> wrapped in a <see cref="ViewWrapper"/>.
    /// </summary>
    /// <param name="frameworkElement">
    /// The <see cref="StyledElement"/> to find the <see cref="ViewWrapper"/> for.
    /// </param>
    /// <returns>The owning <see cref="ViewWrapper"/> if found; otherwise null.</returns>
    internal static ViewWrapper? GetOwner(this StyledElement frameworkElement)
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
    public static ViewWrapper? AsWrapper(this Window? window) =>
        window != null ? new ViewWrapper(window) : null;

    /// <summary>
    /// Converts an IWindow into a WindowWrapper.
    /// </summary>
    /// <param name="window">The IWindow to convert.</param>
    /// <returns>A WindowWrapper referencing the window.</returns>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("window")]
    public static ViewWrapper? AsWrapper(this IView? window) =>
        (ViewWrapper?)window;

    /// <summary>
    /// Runs a synchronous action asynchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to run asynchronously.</param>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <returns>The result of the action.</returns>
    public static Task<T> RunUiAsync<T>(Func<T> action)
    {
        TaskCompletionSource<T> completion = new();
        Dispatcher.UIThread.Post(new Action(() => completion.SetResult(action())));
        return completion.Task;
    }
}
