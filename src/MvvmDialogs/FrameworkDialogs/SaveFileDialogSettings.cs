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
    public string? DefaultExtension { get; set; }

    /// <summary>
    /// Gets or sets whether the save dialog warns before replacing an existing file. Null leaves the platform default.
    /// </summary>
    public bool? ShowOverwritePrompt { get; set; }
}
