
namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Provides WPF-specific application-wide settings.
/// Settings can be overriden for individual calls by specifying the optional appSettings parameter.
/// </summary>
public class AppDialogSettings : AppDialogSettingsBase
{
    /// <summary>
    /// Gets or sets whether message boxes are displayed right-to-left (RightAlign+RtlReading).
    /// </summary>
    public bool MessageBoxRightToLeft { get; set; }

    /// <summary>
    /// Gets or sets whether to display on the default desktop of the interactive window station. Specifies that the message box is displayed from a .NET Windows Service application in order to notify the user of an event.
    /// </summary>
    public bool MessageBoxDefaultDesktopOnly { get; set; }

    /// <summary>
    /// Gets or sets whether to display on the currently active desktop even if a user is not logged on to the computer. Specifies that the message box is displayed from a .NET Windows Service application in order to notify the user of an event.
    /// </summary>
    public bool MessageBoxServiceNotification { get; set; }

    /// <summary>
    /// Creates a copy of this class. Useful to customize settings for specific calls.
    /// </summary>
    /// <returns>A copy of this class.</returns>
    public AppDialogSettings Clone() => (AppDialogSettings)this.MemberwiseClone();
}
