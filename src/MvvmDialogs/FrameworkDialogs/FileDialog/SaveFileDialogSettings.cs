namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for SaveFileDialog.
/// </summary>
public class SaveFileDialogSettings : FileDialogSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the dialog box prompts the user for permission
    /// to create a file if the user specifies a file that does not exist.
    /// </summary>
    public bool CreatePrompt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog box displays a warning if the user
    /// specifies the name of a file that already exists.
    /// </summary>
    public bool OverwritePrompt { get; set; }
}