namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Provides Avalonia-specific application-wide settings.
/// Settings can be overriden for individual calls by specifying the optional appSettings parameter.
/// </summary>
public class AppDialogSettings : AppDialogSettingsBase
{
    /// <summary>
    /// Creates a copy of this class. Useful to customize settings for specific calls.
    /// </summary>
    /// <returns>A copy of this class.</returns>
    public AppDialogSettings Clone() => (AppDialogSettings)this.MemberwiseClone();
}
