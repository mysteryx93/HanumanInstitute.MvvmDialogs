using System;
using HanumanInstitute.MvvmDialogs.Private;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Holds a weak reference to a View instance.
/// </summary>
public abstract class ViewBase
{
    private readonly WeakReference viewReference;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewBase"/> class and hold a weak reference to specified object.
    /// </summary>
    /// <param name="view">The object to hold a weak reference for.</param>
    protected ViewBase(object view)
    {
        if (view == null) throw new ArgumentNullException(nameof(view));

        viewReference = new WeakReference(view);
    }

    /// <summary>
    /// Occurs when the object is fully loaded.
    /// </summary>
    public virtual event EventHandler? Loaded;

    /// <summary>
    /// Returns an auto-generated unique ID for the view.
    /// </summary>
    public int Id { get; } = ViewIdGenerator.Generate();

    /// <summary>
    /// Returns whether the weak reference is still available.
    /// </summary>
    public virtual bool IsAlive => viewReference.IsAlive;

    /// <summary>
    /// Returns the original view reference if it is still alive.
    /// </summary>
    /// <exception cref="InvalidOperationException">View has been garbage collected.</exception>
    public virtual object SourceObj
    {
        get
        {
            if (!IsAlive) throw new InvalidOperationException("View has been garbage collected.");
            if (viewReference.Target == null) throw new InvalidOperationException("View has been set to null.");

            return viewReference.Target;
        }
    }

    /// <summary>
    /// Returns the owner of the element, within a <see cref="IWindow"/> wrapper.
    /// </summary>
    /// <returns>A <see cref="IWindow"/> wrapper around the owner, or null.</returns>
    public abstract IWindow? GetOwner();

    /// <summary>
    /// Returns whether referenced element is loaded.
    /// </summary>
    public abstract bool IsLoaded { get; }

    /// <summary>
    /// Returns the DataContext of referenced element.
    /// </summary>
    public abstract object? DataContext { get; }

    /// <summary>Determines whether specified view references the same object as the current view.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if specified view references the same object; otherwise, false.</returns>
    public override bool Equals(object? obj) =>
        obj is ViewBase other && SourceObj.Equals(other.SourceObj);

    /// <summary>Returns the hash code of referenced object.</summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode() => SourceObj.GetHashCode();

    /// <summary>
    /// Raises the Loaded event.
    /// </summary>
    protected void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
}
