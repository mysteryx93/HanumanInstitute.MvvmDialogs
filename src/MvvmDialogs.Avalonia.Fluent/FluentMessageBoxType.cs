// ReSharper disable CheckNamespace
namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Represents how to display message boxes. 
/// </summary>
public enum FluentMessageBoxType
{
    /// <summary>
    /// MessageBox dialogs will not be handled by FluentAvalonia.
    /// </summary>
    None,
    /// <summary>
    /// MessageBox dialogs will be shown in content dialogs.
    /// </summary>
    ContentDialog,
    /// <summary>
    /// MessageBox dialogs will be shown in task dialogs.
    /// </summary>
    TaskDialog
}
