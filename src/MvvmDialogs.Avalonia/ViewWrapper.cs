using Avalonia;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Holds a weak reference to a FrameworkElement.
/// </summary>
public class ViewWrapper : ViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewWrapper"/> class and hold a weak reference to specified <see cref="StyledElement"/>.
    /// </summary>
    /// <param name="view">The object to hold a weak reference for.</param>
    public ViewWrapper(StyledElement view) : base(view)
    {
        view.Initialized += (_, _) => RaiseLoaded();
    }

    /// <summary>
    /// Returns the referenced <see cref="StyledElement"/> if it is still alive.
    /// </summary>
    public StyledElement Source => (StyledElement)base.SourceObj;

    /// <summary>
    /// Returns the DataContext of referenced element.
    /// </summary>
    public override object? DataContext => Source.DataContext;

    /// <summary>
    /// Returns whether referenced element is loaded.
    /// </summary>
    public override bool IsLoaded => Source.IsInitialized;

    /// <summary>
    /// Returns the owner of the element, within a <see cref="IWindow"/> wrapper.
    /// </summary>
    /// <returns>A <see cref="IWindow"/> wrapper around the owner, or null.</returns>
    public override IWindow? GetOwner() => Source.GetOwner();
}
