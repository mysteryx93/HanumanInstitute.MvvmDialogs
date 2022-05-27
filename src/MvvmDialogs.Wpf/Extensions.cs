using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Provides extension methods for WPF.
/// </summary>
public static class Extensions
{
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
        window.Dispatcher.BeginInvoke(new Action(() => completion.SetResult(action())));
        return completion.Task;
    }

    /// <summary>
    /// Creates a WindowWrapper around specified window.
    /// </summary>
    /// <param name="window">The Window to get a wrapper for.</param>
    /// <returns>A WindowWrapper referencing the window.</returns>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("window")]
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
    /// Gets the owner of a <see cref="FrameworkElement"/> wrapped in a <see cref="WindowWrapper"/>.
    /// </summary>
    /// <param name="frameworkElement">
    /// The <see cref="FrameworkElement"/> to find the <see cref="WindowWrapper"/> for.
    /// </param>
    /// <returns>The owning <see cref="WindowWrapper"/> if found; otherwise null.</returns>
    internal static WindowWrapper GetOwner(this FrameworkElement frameworkElement)
    {
        var owner = frameworkElement as Window ?? Window.GetWindow(frameworkElement);
        return owner.AsWrapper();
    }

    /// <summary>
    /// Returns the Sync interface of an IFrameworkDialog.
    /// </summary>
    internal static IFrameworkDialogSync<T> AsSync<T>(this IFrameworkDialog<T> factory) =>
        factory as IFrameworkDialogSync<T> ?? throw new InvalidCastException("IFrameworkDialog<T> instance doesn't implement IFrameworkDialogSync<T>.");

    /// <summary>
    /// Returns the Sync interface of an IWindow.
    /// </summary>
    internal static IWindowSync AsSync(this IWindow window) =>
        window as IWindowSync ?? throw new InvalidCastException("IWindow instance doesn't implement IWindowSync.");

    /// <summary>
    /// Returns the Sync interface of an IDialogService.
    /// </summary>
    internal static IDialogServiceSync AsSync(this IDialogService service) =>
        service as IDialogServiceSync ?? throw new InvalidCastException("IDialogService instance doesn't implement IDialogServiceSync.");

    /// <summary>
    /// Returns the Sync interface of an IDialogManager.
    /// </summary>
    internal static IDialogManagerSync AsSync(this IDialogManager service) =>
        service as IDialogManagerSync ?? throw new InvalidCastException("IDialogManager instance doesn't implement IDialogManagerSync.");
}
