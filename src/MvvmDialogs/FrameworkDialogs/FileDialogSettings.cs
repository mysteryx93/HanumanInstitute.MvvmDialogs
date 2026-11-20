namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for FileDialog.
/// </summary>
public abstract class FileDialogSettings : PickerDialogSettings
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
    public IList<FileFilter> Filters { get; set; } = new List<FileFilter>();

    /// <summary>
    /// Gets or sets the initial file that is displayed by a file dialog.
    /// </summary>
    public string SuggestedFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file type selected when the dialog opens. Use one of the instances in <see cref="Filters"/>.
    /// </summary>
    public FileFilter? SuggestedFileType { get; set; }

    /// <summary>
    /// Gets the file type the user selected. The open and save methods set this when the platform reports one, and clear it otherwise.
    /// </summary>
    public FileFilter? SelectedFileType { get; set; }

    /// <summary>
    /// Callback to invoke when the user clicks the help button. Setting this will display a help button.
    /// </summary>
    public EventHandler? HelpRequest { get; set; }
}
