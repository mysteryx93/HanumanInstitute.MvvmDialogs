
namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// Represents the content dialog settings.
/// </summary>
public class ContentDialogSettings : DialogSettingsBase
{
    /// <summary>
    /// Gets or sets the content of the dialog.
    /// </summary>
    public object? Content { get; set; }
    /// <summary>
    /// Gets or sets the text to display on the close button.
    /// </summary>
    public string CloseButtonText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the text to display on the primary button.
    /// </summary>
    public string PrimaryButtonText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the text to be displayed on the secondary button.
    /// </summary>
    public string SecondaryButtonText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value that indicates which button on the dialog is the default action.
    /// </summary>
    public ContentDialogButton DefaultButton { get; set; } = ContentDialogButton.None;
    /// <summary>
    /// Gets or sets whether the dialog's primary button is enabled.
    /// </summary>
    public bool IsPrimaryButtonEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets whether the dialog's secondary button is enabled.
    /// </summary>
    public bool IsSecondaryButtonEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets whether the Dialog should show full screen
    /// On WinUI3, at least desktop, this just show the dialog at 
    /// the maximum size of a ContentDialog.
    /// </summary>
    public bool FullSizeDesired { get; set; }
}
