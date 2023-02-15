using System.IO;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.PathInfo;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <inheritdoc cref="IDialogStorageFile"/>
public class DialogStorageFile : DialogStorageItem, IDialogStorageFile
{
    private readonly IFileInfo _info;
    
    /// <summary>
    /// Initializes a new instance of DialogStorageFile to expose a path.
    /// </summary>
    /// <param name="info">Information about the path to expose.</param>
    public DialogStorageFile(IFileInfo info) : base(info)
    {
        _info = (IFileInfo)InfoBase;
    }
    
    /// <inheritdoc />
    public Task<Stream> OpenReadAsync() => Task.Run<Stream>(() => File.OpenRead(_info.FullName));

    /// <inheritdoc />
    public Task<Stream> OpenWriteAsync() => Task.Run<Stream>(() => File.OpenWrite(_info.FullName));
}
