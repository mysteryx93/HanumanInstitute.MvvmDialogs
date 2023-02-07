
namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// Contains the API to display FluentAvalonia dialogs.
/// </summary>
internal interface IFluentApi
{
    Task<ContentDialogResult> ShowContentDialog(ContentControl? owner, ContentDialogSettings settings);
    Task<object> ShowTaskDialog(ContentControl? owner, TaskDialogSettings settings);
}
