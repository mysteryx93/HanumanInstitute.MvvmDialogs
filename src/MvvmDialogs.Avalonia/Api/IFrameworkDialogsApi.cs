
namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <summary>
/// Wrapper around Win32 dialogs API that can be replaced by a mock for testing.
/// </summary>
internal interface IFrameworkDialogsApi
{
    Task<string[]?> ShowOpenFileDialogAsync(Window? owner, OpenFileApiSettings settings);
    Task<string?> ShowSaveFileDialogAsync(Window? owner, SaveFileApiSettings settings);
    Task<string?> ShowOpenFolderDialogAsync(Window? owner, OpenFolderApiSettings settings);
}
