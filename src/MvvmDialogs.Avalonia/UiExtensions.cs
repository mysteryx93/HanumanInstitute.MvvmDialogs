// using System.Diagnostics.CodeAnalysis;
// using Avalonia.LogicalTree;

using System.Diagnostics.CodeAnalysis;
using Avalonia.Threading;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Extension methods.
/// </summary>
public static class UiExtensions
{
    // /// <summary>
    // /// Gets the owner of a <see cref="StyledElement"/> wrapped in a <see cref="ViewWrapper"/>.
    // /// </summary>
    // /// <param name="frameworkElement">
    // /// The <see cref="StyledElement"/> to find the <see cref="ViewWrapper"/> for.
    // /// </param>
    // /// <returns>The owning <see cref="ViewWrapper"/> if found; otherwise null.</returns>
    // internal static ViewWrapper? GetOwner(this StyledElement frameworkElement)
    // {
    //     var owner = frameworkElement as Window ?? frameworkElement.FindLogicalAncestorOfType<Window>();
    //     return owner.AsWrapper();
    // }

    /// <summary>
    /// Creates a ViewWrapper around specified window.
    /// </summary>
    /// <param name="window">The Window to get a wrapper for.</param>
    /// <returns>A ViewWrapper referencing the window.</returns>
    [return: NotNullIfNotNull("window")]
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
    /// Creates a ViewNavigationWrapper around specified user control.
    /// </summary>
    /// <param name="view">The UserControl to get a wrapper for.</param>
    /// <param name="navigationManager"></param>
    /// <returns>A ViewNavigationWrapper referencing the user control.</returns>
    [return: NotNullIfNotNull("view")]
    public static ViewNavigationWrapper? AsWrapper(this UserControl? view, INavigationManager navigationManager)
    {
        if (view != null)
        {
            var result = new ViewNavigationWrapper();
            result.InitializeExisting((INotifyPropertyChanged)view.DataContext!, view);
            result.SetNavigation(navigationManager);
            return result;
        }
        return null;
    }

    /// <summary>
    /// Returns the <see cref="IView"/> RefObj property as an Avalonia ContentControl.
    /// </summary>
    /// <param name="view">The IView to get the Ref property for.</param>
    /// <returns>The ContentControl held within the IView.</returns>
    public static ContentControl? GetRef(this IView? view)
    {
        if (view is ViewWrapper v)
        {
            return v.Ref;
        }
        else if (view is ViewNavigationWrapper nav)
        {
            return nav.Ref;
        }
        return null;
    }

    // /// <summary>
    // /// Converts an IView into a ViewWrapper.
    // /// </summary>
    // /// <param name="window">The IWindow to convert.</param>
    // /// <returns>A ViewWrapper referencing the window.</returns>
    // [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("window")]
    // public static ViewWrapper? AsWrapper(this IView? window) =>
    //     (ViewWrapper?)window;

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
