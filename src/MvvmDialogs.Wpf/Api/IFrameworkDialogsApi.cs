
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Wpf.Api;

/// <summary>
/// Wrapper around Win32 dialogs API that can be replaced by a mock for testing.
/// </summary>
internal interface IFrameworkDialogsApi
{
    MessageBoxResult ShowMessageBox(Window? owner, MessageBoxApiSettings settings);
    IReadOnlyList<IDialogStorageFile> ShowOpenFileDialog(Window? owner, OpenFileApiSettings settings);
    IDialogStorageFile? ShowSaveFileDialog(Window? owner, SaveFileApiSettings settings);
    IReadOnlyList<IDialogStorageFolder> ShowOpenFolderDialog(Window? owner, OpenFolderApiSettings settings);
}
