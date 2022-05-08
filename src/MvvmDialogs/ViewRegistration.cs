using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Class containing means to register a FrameworkElement as a view for a view
/// model when using the MVVM pattern. The view will then be used by the
/// <see cref="DialogServiceBase"/> when opening dialogs.
/// </summary>
public static class ViewRegistration
{
    /// <summary>
    /// The registered views.
    /// </summary>
    private static readonly List<ViewBase> InternalViews = new List<ViewBase>();

    /// <summary>
    /// Gets the registered views.
    /// </summary>
    public static IEnumerable<ViewBase> Views =>
        InternalViews
            .Where(view => view.IsAlive)
            .ToArray();

    /// <summary>
    /// Registers specified view.
    /// </summary>
    /// <param name="view">The view to register.</param>
    public static void Register(ViewBase view)
    {
        if (view == null) throw new ArgumentNullException(nameof(view));

        // Get owner window
        var owner = view.GetOwner();
        if (owner == null)
        {
            // Perform a late register when the view hasn't been loaded yet.
            // This will happen if e.g. the view is contained in a Frame.
            view.Loaded += LateRegister;
            return;
        }

        PruneInternalViews();

        // Register for owner window closing to cleanup views connected to this window, but
        // only register for the event once, thus the un-registration of any prior
        // registrations.
        owner.Closed -= OwnerClosed;
        owner.Closed += OwnerClosed;

        DialogLogger.Write($"Register view {view.Id}");
        InternalViews.Add(view);
        DialogLogger.Write($"Registered view {view.Id} ({InternalViews.Count} registered)");
    }

    /// <summary>
    /// Unregisters specified view.
    /// </summary>
    /// <param name="view">The view to unregister.</param>
    public static void Unregister(ViewBase view)
    {
        if (view == null) throw new ArgumentNullException(nameof(view));

        PruneInternalViews();

        DialogLogger.Write($"Unregister view {view.Id}");
        InternalViews.RemoveAll(v => ReferenceEquals(v.SourceObj, view.SourceObj));
        DialogLogger.Write($"Unregistered view {view.Id} ({InternalViews.Count} registered)");
    }

    /// <summary>
    /// Clears the registered views.
    /// </summary>
    public static void Clear()
    {
        DialogLogger.Write("Clearing views");
        InternalViews.Clear();
        DialogLogger.Write("Cleared views");
    }

    /// <summary>
    /// Find the view corresponding to specified view model.
    /// </summary>
    /// <exception cref="ViewNotRegisteredException">View model is not present as data context on any registered view.</exception>
    public static IWindow FindView(INotifyPropertyChanged viewModel)
    {
        var view = Views.SingleOrDefault(
            registeredView =>
                registeredView.IsLoaded &&
                ReferenceEquals(registeredView.DataContext, viewModel));

        var owner = view?.GetOwner();

        if (view == null || owner == null)
        {
            var message =
                $"View model of type '{viewModel.GetType()}' is not present as data context on any registered view. Please register the view by setting DialogServiceViews.IsRegistered=\"True\" in your XAML.";

            throw new ViewNotRegisteredException(message);
        }

        return owner;
    }

    /// <summary>
    /// Callback for late view register. It wasn't possible to do a instant register since the
    /// view wasn't at that point part of the logical nor visual tree.
    /// </summary>
    private static void LateRegister(object? sender, EventArgs e)
    {
        if (sender is ViewBase view)
        {
            // Unregister loaded event
            view.Loaded -= LateRegister;

            Register(view);
        }
    }

    /// <summary>
    /// Handles owner window closed. All views acting within the closed window should be
    /// unregistered.
    /// </summary>
    private static void OwnerClosed(object? sender, EventArgs e)
    {
        if (sender is IWindow owner)
        {
            // Find views acting within closed window
            var windowViews = Views
                .Where(view => ReferenceEquals(view.GetOwner(), owner))
                .ToArray();

            // Unregister Views in window
            foreach (var windowView in windowViews)
            {
                DialogLogger.Write($"Window containing view {windowView.Id} closed");
                Unregister(windowView);
            }
        }
    }

    private static void PruneInternalViews()
    {
        DialogLogger.Write($"Before pruning ({InternalViews.Count} registered)");
        InternalViews.RemoveAll(reference => !reference.IsAlive);
        DialogLogger.Write($"After pruning ({InternalViews.Count} registered)");
    }
}
