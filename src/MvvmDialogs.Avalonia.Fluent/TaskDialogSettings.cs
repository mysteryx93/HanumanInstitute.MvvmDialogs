
using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// Represents the task dialog settings.
/// </summary>
public class TaskDialogSettings : DialogSettingsBase
{
    /// <summary>
    /// Initializes a new instance of the TaskDialogSettings class.
    /// </summary>
    public TaskDialogSettings() {}

    /// <summary>
    /// Initializes a new instance of the TaskDialogSettings class with specified content.
    /// </summary>
    /// <param name="content">The content of the dialog. Can be a ViewModel, Control, or any content.</param>
    public TaskDialogSettings(object? content)
    {
        Content = content;
    }
    
    /// <summary>
    /// Gets or sets the dialog header text
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the dialog sub header text
    /// </summary>
    public string SubHeader { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the content of the dialog.
    /// </summary>
    public object? Content { get; set; }
    /// <summary>
    /// Gets or sets the dialog Icon
    /// </summary>
    public IconSource? IconSource { get; set; }
    /// <summary>
    /// Gets the list of buttons that display at the bottom of the TaskDialog
    /// </summary>
    public IList<TaskDialogButton> Buttons { get; set; } = new List<TaskDialogButton>();
    /// <summary>
    /// Gets the list of Commands displayed in the TaskDialog
    /// </summary>
    public IList<TaskDialogCommand> Commands { get; set; } = new List<TaskDialogCommand>();
    /// <summary>
    /// Gets or sets the visibility of the Footer area
    /// </summary>
    public TaskDialogFooterVisibility FooterVisibility { get; set; }
    /// <summary>
    /// Gets or sets whether the footer is visible
    /// </summary>
    public bool IsFooterExpanded { get; set; }
    /// <summary>
    /// Gets or sets the footer content
    /// </summary>
    public object? Footer { get; set; }
    /// <summary>
    /// Gets or sets whether this TaskDialog shows a progress bar
    /// </summary>
    public bool ShowProgressBar { get; set; }
}
