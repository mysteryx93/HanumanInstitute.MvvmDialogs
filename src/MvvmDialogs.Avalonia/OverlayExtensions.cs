using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Presents inline dialogs for Avalonia windows and navigation pages.
/// </summary>
public static class OverlayExtensions
{
    /// <summary>
    /// Gets the control that can show an overlay. A covered navigation page
    /// confirming a close uses the foreground control in its navigation host.
    /// </summary>
    public static Control? GetOverlayOwner(this IView? view) =>
        view is ViewNavigationWrapper page ? page.GetOverlayOwner() : view?.RefObj as Control;

    /// <summary>
    /// Shows a dialog that uses its own overlay on the owner's top level.
    /// Back requests call <paramref name="close"/>. Owner removal calls <paramref name="ownerClosed"/>
    /// when supplied, so a dialog can bypass normal close confirmation.
    /// </summary>
    /// <exception cref="InvalidOperationException">The owner is detached from its top level.</exception>
    public static async Task<TResult> RunOverlayAsync<TResult>(
        this IView view,
        Func<TopLevel, Task<TResult>> show,
        Action close,
        Action? ownerClosed = null)
    {
        var owner = view.GetOverlayOwner();
        var top = await GetTopLevelAsync(owner) ??
                  throw new InvalidOperationException("The dialog owner is detached from its top level.");
        return await TrackOverlayAsync(owner!, top, () => show(top), close, ownerClosed);
    }

    /// <summary>
    /// Shows an inline dialog on its own content control over the owner. Back and owner removal
    /// dismiss the dialog. Returns <paramref name="unavailable"/> if the owner has no overlay layer.
    /// </summary>
    public static async Task<TResult?> WithOverlaySurfaceAsync<TResult>(
        this IView? view,
        Func<ContentControl, Task<TResult>> show,
        Action close,
        TResult? unavailable = default,
        Action? ownerClosed = null)
    {
        var owner = view.GetOverlayOwner();
        var top = await GetTopLevelAsync(owner);
        if (top == null) { return unavailable; }

        // The window's content presenter is inside its visual layer manager.
        var target = owner is Window window ? window.Presenter : owner;
        var layer = target == null ? null : AdornerLayer.GetAdornerLayer(target);
        if (layer == null) { return unavailable; }

        var surface = new ContentControl
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        AdornerLayer.SetAdornedElement(surface, target);
        try
        {
            layer.Children.Add(surface);
            return await TrackOverlayAsync(owner!, top, () => show(surface), close, ownerClosed);
        }
        finally
        {
            layer.Children.Remove(surface);
            AdornerLayer.SetAdornedElement(surface, null);
        }
    }

    // A newly presented navigation page may still be waiting for layout.
    private static async Task<TopLevel?> GetTopLevelAsync(Control? owner)
    {
        var top = TopLevel.GetTopLevel(owner);
        if (top == null && owner != null)
        {
            await Dispatcher.UIThread.InvokeAsync(static () => { }, DispatcherPriority.Loaded);
            top = TopLevel.GetTopLevel(owner);
        }
        return top;
    }

    // Keep Back and owner dismissal registered for the lifetime of the native show operation.
    private static async Task<TResult> TrackOverlayAsync<TResult>(
        Control owner,
        TopLevel top,
        Func<Task<TResult>> show,
        Action close,
        Action? ownerClosed)
    {
        Task<TResult>? pending = null;
        var dismissOnRemoval = ownerClosed ?? close;
        owner.DetachedFromVisualTree += OwnerGone;
        top.Closed += OwnerGone;
        try
        {
            return await CancellableActions.RunAsync(() => pending = show(), CloseOnBack, owner);
        }
        finally
        {
            owner.DetachedFromVisualTree -= OwnerGone;
            top.Closed -= OwnerGone;
        }

        void CloseOnBack()
        {
            if (pending?.IsCompleted != true) { close(); }
        }

        // Detachment and window closure can both report the same removal.
        void OwnerGone(object? sender, EventArgs e)
        {
            var dismiss = dismissOnRemoval;
            if (dismiss == null) { return; }
            dismissOnRemoval = null;
            Dispatcher.UIThread.Post(() =>
            {
                if (pending != null && !pending.IsCompleted) { dismiss(); }
            });
        }
    }
}
