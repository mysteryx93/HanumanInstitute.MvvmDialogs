using System.IO;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc cref="IDialogStorageFile"/>
public class DialogStorageFile : DialogStorageItem, IDialogStorageFile
{
    private readonly IStorageFile _item;
    
    /// <summary>
    /// Initializes a new instance of DialogStorageFile as a bridge to specified Avalonia IStorageFile.
    /// </summary>
    /// <param name="item">An Avalonia IStorageFile from which to get the values.</param>
    public DialogStorageFile(IStorageFile item) : base(item)
    {
        _item = item;
    }

    /// <inheritdoc />
    public bool CanOpenRead => _item.CanOpenRead;

    /// <inheritdoc />
    public Task<Stream> OpenReadAsync() => _item.OpenReadAsync();

    /// <inheritdoc />
    public bool CanOpenWrite => _item.CanOpenWrite;

    /// <inheritdoc />
    public Task<Stream> OpenWriteAsync() => _item.OpenWriteAsync();
}
