namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Base class for platform-specific application settings.
/// </summary>
public abstract class AppDialogSettingsBase
{
    /// <summary>
    /// Gets or sets whether multiple dialogs can be shown at the same time.
    /// If false (default), it will wait for the previous dialog to close before showing the next one.
    /// </summary>
    public bool AllowConcurrentDialogs { get; set; } 
}
