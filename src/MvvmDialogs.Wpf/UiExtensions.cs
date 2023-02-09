using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Provides extension methods for WPF.
/// </summary>
public static class UiExtensions
{
    /// <summary>
    /// Runs a synchronous action asynchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to run asynchronously.</param>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <returns>The result of the action.</returns>
    public static Task<T> RunUiAsync<T>(Func<T> action)
    {
        TaskCompletionSource<T> completion = new();
        Application.Current.Dispatcher.BeginInvoke(new Action(() => completion.SetResult(action())));
        return completion.Task;
    }

    /// <summary>
    /// Runs a synchronous action asynchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to run asynchronously.</param>
    public static Task RunUiAsync(Action action)
    {
        TaskCompletionSource<bool> completion = new();
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            action();
            completion.SetResult(true);
        }));
        return completion.Task;
    }

    /// <summary>
    /// Creates a WindowWrapper around specified window.
    /// </summary>
    /// <param name="window">The Window to get a wrapper for.</param>
    /// <returns>A WindowWrapper referencing the window.</returns>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("window")]
    public static ViewWrapper? AsWrapper(this Window? window)
    {
        if (window != null)
        {
            var result = new ViewWrapper();
            result.InitializeExisting((INotifyPropertyChanged)window.DataContext!, window);
            return result;
        }
        return null;
    }

    /// <summary>
    /// Converts an IWindow into a WindowWrapper.
    /// </summary>
    /// <param name="window">The IWindow to convert.</param>
    /// <returns>A WindowWrapper referencing the window.</returns>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("window")]
    public static ViewWrapper? AsWrapper(this IView? window) =>
        (ViewWrapper?)window;

    /// <summary>
    /// Returns the <see cref="IView"/> RefObj property as an Avalonia ContentControl.
    /// </summary>
    /// <param name="view">The IView to get the Ref property for.</param>
    /// <returns>The ContentControl held within the IView.</returns>
    public static Window? GetRef(this IView? view)
    {
        if (view is ViewWrapper v)
        {
            return v.Ref;
        }
        return null;
    }

    /// <summary>
    /// Gets the owner of a <see cref="FrameworkElement"/> wrapped in a <see cref="ViewWrapper"/>.
    /// </summary>
    /// <param name="frameworkElement">
    /// The <see cref="FrameworkElement"/> to find the <see cref="ViewWrapper"/> for.
    /// </param>
    /// <returns>The owning <see cref="ViewWrapper"/> if found; otherwise null.</returns>
    internal static ViewWrapper GetOwner(this FrameworkElement frameworkElement)
    {
        var owner = frameworkElement as Window ?? Window.GetWindow(frameworkElement);
        return owner.AsWrapper();
    }

    /// <summary>
    /// Returns the Sync interface of an IFrameworkDialog.
    /// </summary>
    internal static IDialogFactorySync AsSync(this IDialogFactory factory) =>
        factory as IDialogFactorySync ?? throw new InvalidCastException("IDialogFactory instance doesn't implement IDialogFactorySync.");

    /// <summary>
    /// Returns the Sync interface of an IWindow.
    /// </summary>
    internal static IViewSync AsSync(this IView window) =>
        window as IViewSync ?? throw new InvalidCastException("IWindow instance doesn't implement IWindowSync.");

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
