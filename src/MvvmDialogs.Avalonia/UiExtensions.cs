// using System.Diagnostics.CodeAnalysis;
// using Avalonia.LogicalTree;

using System.Diagnostics.CodeAnalysis;

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
    /// <param name="navigationManager">The <see cref="INavigationManager"/> to set.</param>
    /// <param name="closingHandler">An optional handler for close requests made through the wrapper.</param>
    /// <returns>A wrapper for a user control with an <see cref="INotifyPropertyChanged"/> data context, or null.</returns>
    public static ViewNavigationWrapper? AsWrapper(this UserControl? view, INavigationManager navigationManager, ViewClosingHandler? closingHandler)
    {
        if (view?.DataContext is not INotifyPropertyChanged viewModel) { return null; }
        if (navigationManager is NavigationManager manager)
        {
            var existing = manager.FindOpenView(viewModel);
            if (existing != null && ReferenceEquals(existing.Ref, view) &&
                (closingHandler == null || Equals(closingHandler, existing.ClosingHandler)))
            {
                return existing;
            }
        }
        var wrapper = new ViewNavigationWrapper(navigationManager, closingHandler);
        wrapper.InitializeExisting(viewModel, view);
        return wrapper;
    }

    /// <summary>
    /// Returns the view's <see cref="IView.RefObj"/> when it is an Avalonia control.
    /// </summary>
    /// <param name="view">The view whose owner control is required.</param>
    /// <returns>The control stored in RefObj, or null.</returns>
    public static ContentControl? GetRef(this IView? view) => view?.RefObj as ContentControl;

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
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                completion.SetResult(action());
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        });
        return completion.Task;
    }
}
