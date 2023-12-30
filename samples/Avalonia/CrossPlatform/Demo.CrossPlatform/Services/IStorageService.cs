using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.CrossPlatform.Services;

public interface IStorageService
{
    Task<IDialogStorageFolder?> GetDownloadsFolderAsync();
}
