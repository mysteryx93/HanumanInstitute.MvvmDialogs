using System.IO;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc cref="IDialogStorageFile"/>
public class AvaloniaDialogStorageFile : AvaloniaDialogStorageItem, IDialogStorageFile
{
    private readonly IStorageFile _item;

    /// <summary>
    /// Initializes a new instance of DialogStorageFile as a bridge to specified Avalonia IStorageFile.
    /// </summary>
    /// <param name="item">An Avalonia IStorageFile from which to get the values.</param>
    public AvaloniaDialogStorageFile(IStorageFile item) : base(item)
    {
        _item = item;
    }

    /// <inheritdoc />
    public Task<Stream> OpenReadAsync() => _item.OpenReadAsync();

    /// <inheritdoc />
    public Task<Stream> OpenWriteAsync() => _item.OpenWriteAsync();
}
