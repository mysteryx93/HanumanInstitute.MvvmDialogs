
namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for FolderBrowserDialog.
/// </summary>
public class OpenFolderDialogSettings : DialogSettingsBase
{
    /// <summary>
    /// DO NOT USE. Use OpenFolderDialog / OpenFoldersDialog instead.
    /// </summary>
    public bool? AllowMultiple { get; set; }

    /// <summary>
    /// Gets or sets the path initially selected.
    /// </summary>
    public string? InitialDirectory { get; set; }

    /// <summary>
    /// Callback to invoke when the user clicks the help button. Setting this will display a help button.
    /// </summary>
    public EventHandler? HelpRequest { get; set; }
}
