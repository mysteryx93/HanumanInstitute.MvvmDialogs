
namespace HanumanInstitute.MvvmDialogs.Wpf.Api;

/// <summary>
/// Wrapper around Win32 dialogs API that can be replaced by a mock for testing.
/// </summary>
internal interface IFrameworkDialogsApi
{
    MessageBoxResult ShowMessageBox(Window owner, MessageBoxApiSettings settings);
    string[] ShowOpenFileDialog(Window owner, OpenFileApiSettings settings);
    string? ShowSaveFileDialog(Window owner, SaveFileApiSettings settings);
    string? ShowOpenFolderDialog(Window owner, OpenFolderApiSettings settings);
}
