namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for SaveFileDialog.
/// </summary>
public class SaveFileDialogSettings : FileDialogSettings
{
    /// <summary>
    /// Gets or sets the default extension to be used (including the period ".")
    /// if not set by the user or by a filter
    /// </summary>
    public string DefaultExtension { get; set; } = string.Empty;
}
