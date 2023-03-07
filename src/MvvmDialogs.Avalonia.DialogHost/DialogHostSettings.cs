using System.ComponentModel;
using Avalonia.Media;
using DialogHostAvalonia;
using DialogHostAvalonia.Positioners;

namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

/// <summary>
/// Settings for showing DialogHost overlays.
/// </summary>
public class DialogHostSettings : DialogSettingsBase
{
    /// <summary>
    /// The view model of the view to show. The view will be resolved through Avalonia's ViewLocator.
    /// </summary>
    public INotifyPropertyChanged? ContentViewModel { get; set; }
    /// <summary>
    /// A handler that will be called when the view is closing, allowing to cancel the close.
    /// </summary>
    public DialogClosingEventHandler? ClosingHandler { get; set; }
    /// <summary>
    /// Whether to close the view when clicking elsewhere in the parent container.
    /// </summary>
    public bool CloseOnClickAway { get; set; }
    /// <summary>
    /// The close value to set when closing by clicking away.
    /// </summary>
    public object? CloseOnClickAwayParameter { get; set; }
    /// <summary>
    /// A class allowing to customize the positioning of the dialog.
    /// </summary>
    public IDialogPopupPositioner? PopupPositioner { get; set; }
    /// <summary>
    /// The background of the overlay.
    /// </summary>
    public IBrush? OverlayBackground { get; set; }
    /// <summary>
    /// The margin of the dialog view.
    /// </summary>
    public Thickness? DialogMargin { get; set; }
    /// <summary>
    /// Whether to disable the popup animation.
    /// </summary>
    public bool DisableOpeningAnimation { get; set; } = true;
}
