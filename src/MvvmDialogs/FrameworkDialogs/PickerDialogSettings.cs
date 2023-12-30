using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Settings for file/folder picker dialogs.
/// </summary>
public abstract class PickerDialogSettings : DialogSettingsBase
{
    /// <summary>
    /// Gets or sets the initial directory that is displayed by a file dialog.
    /// </summary>
    public IDialogStorageFolder? SuggestedStartLocation { get; set; }
}
