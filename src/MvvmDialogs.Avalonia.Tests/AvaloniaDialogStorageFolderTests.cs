using System.IO;
using System.Reflection;
using Avalonia.Platform.Storage;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class AvaloniaDialogStorageFolderTests
{
    [Fact]
    public void LocalPath_VolumeRoot_ReturnsFullPath()
    {
        var root = Path.GetPathRoot(Environment.CurrentDirectory)!;
        var folder = CreateFolder(root);

        var path = new AvaloniaDialogStorageFolder(folder).LocalPath;

        Assert.Equal(new DirectoryInfo(root).FullName, path);
    }

    [Fact]
    public void LocalPath_NestedFolder_ReturnsFullPath()
    {
        var directory = Environment.CurrentDirectory;
        var folder = CreateFolder(directory);

        var path = new AvaloniaDialogStorageFolder(folder).LocalPath;

        Assert.Equal(new DirectoryInfo(directory).FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, path);
    }

    // The folder picker item is internal. This builds the same type the dialog returns.
    private static IStorageFolder CreateFolder(string path)
    {
        var assembly = typeof(global::Avalonia.AvaloniaObject).Assembly;
        var type = assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFolder")!;
        var constructor = type.GetConstructor([typeof(DirectoryInfo)])!;
        return (IStorageFolder)constructor.Invoke([new DirectoryInfo(path)]);
    }
}
