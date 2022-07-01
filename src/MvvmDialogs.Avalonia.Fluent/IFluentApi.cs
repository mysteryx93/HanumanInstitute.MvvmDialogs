
namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// Contains the API to display FluentAvalonia dialogs.
/// </summary>
internal interface IFluentApi
{
    Task<ContentDialogResult> ShowContentDialog(Window owner, ContentDialogSettings settings);
    Task<object> ShowTaskDialog(Window owner, TaskDialogSettings settings);
}
