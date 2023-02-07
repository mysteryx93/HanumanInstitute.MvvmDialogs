
using System.Collections.Generic;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <summary>
/// Wrapper around Win32 dialogs API that can be replaced by a mock for testing.
/// </summary>
internal interface IFrameworkDialogsApi
{
    Task<IReadOnlyList<IDialogStorageFile>> ShowOpenFileDialogAsync(ContentControl? owner, FilePickerOpenOptions options);
    Task<IDialogStorageFile?> ShowSaveFileDialogAsync(ContentControl? owner, FilePickerSaveOptions options);
    Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(ContentControl? owner, FolderPickerOpenOptions options);
}
