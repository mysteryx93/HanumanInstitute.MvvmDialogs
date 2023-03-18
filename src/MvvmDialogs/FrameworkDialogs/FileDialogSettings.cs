using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for FileDialog.
/// </summary>
public abstract class FileDialogSettings : DialogSettingsBase
{
    /// <summary>
    /// Gets or sets a value indicating whether a file dialog returns either the location of
    /// the file referenced by a shortcut or the location of the shortcut file (.lnk).
    /// </summary>
    public bool DereferenceLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets a collection of filters which determine the types of files displayed in an
    /// OpenFileDialog or SaveFileDialog.
    /// </summary>
    /// <remarks>
    /// The '.' in extensions is optional. Extensions will automatically be added
    /// to the descriptions unless it contains '('.
    /// If you do not wish to display extensions, end the name with '()' and it will be trimmed away.
    /// </remarks>
    public List<FileFilter> Filters { get; set; } = new();

    /// <summary>
    /// Gets or sets the initial directory that is displayed by a file dialog.
    /// </summary>
    public string InitialDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the initial file that is displayed by a file dialog.
    /// </summary>
    public string InitialFile { get; set; } = string.Empty;

    /// <summary>
    /// Callback to invoke when the user clicks the help button. Setting this will display a help button.
    /// </summary>
    public EventHandler? HelpRequest { get; set; }
}
