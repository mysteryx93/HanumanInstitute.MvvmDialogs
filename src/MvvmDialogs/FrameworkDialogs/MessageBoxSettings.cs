
namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for a MessageBox.
/// </summary>
public class MessageBoxSettings : DialogSettingsBase
{
    /// <summary>
    /// Gets or sets the <see cref="MessageBoxButton"/> value that specifies which button or
    /// buttons to display. Default value is <see cref="MessageBoxButton.Ok"/>.
    /// </summary>
    public MessageBoxButton Button { get; set; } = MessageBoxButton.Ok;

    /// <summary>
    /// Gets or sets the value of the button selected by default. True=OK/Yes, False=No, Null=Cancel.
    /// </summary>
    public bool? DefaultValue { get; set; } = true;

    /// <summary>
    /// Gets or sets the <see cref="MessageBoxImage"/> value that specifies the icon to
    /// display. Default value is <see cref="MessageBoxImage.None"/>.
    /// </summary>
    public MessageBoxImage Icon { get; set; } = MessageBoxImage.None;

    /// <summary>
    /// Gets or sets the <see cref="string"/> that specifies the text to display.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
