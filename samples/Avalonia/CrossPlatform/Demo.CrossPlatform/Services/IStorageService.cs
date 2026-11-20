using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.CrossPlatform.Services;

public interface IStorageService
{
    bool CanPickFiles { get; }

    bool CanSave { get; }

    Task<IDialogStorageFolder?> GetDownloadsFolderAsync();

    Task<string?> SaveBookmarkAsync(IDialogStorageFile file);

    Task<IDialogStorageFile?> OpenFileBookmarkAsync(string bookmark);

    Task ReleaseBookmarkAsync(IDialogStorageFile file);
}
