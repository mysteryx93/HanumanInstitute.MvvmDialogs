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

        Assert.Equal(new DirectoryInfo(directory).FullName, path);
    }

    [Fact]
    public void TryGetLocalPath_VolumeRoot_ReturnsFullPath()
    {
        var root = Path.GetPathRoot(Environment.CurrentDirectory)!;
        var folder = CreateFolder(root);

        var path = new AvaloniaDialogStorageFolder(folder).TryGetLocalPath();

        Assert.Equal(new DirectoryInfo(root).FullName, path);
    }

    [Fact]
    public void TryGetLocalPath_RelativeUri_ReturnsNull()
    {
        var folder = new Mock<IStorageFolder>();
        folder.Setup(item => item.Path).Returns(new Uri("picked", UriKind.Relative));

        var path = new AvaloniaDialogStorageFolder(folder.Object).TryGetLocalPath();

        Assert.Null(path);
    }

    [Fact]
    public void LocalPath_RelativeUri_Throws()
    {
        var folder = new Mock<IStorageFolder>();
        folder.Setup(item => item.Path).Returns(new Uri("picked", UriKind.Relative));
        var storage = new AvaloniaDialogStorageFolder(folder.Object);

        var act = () => storage.LocalPath;

        Assert.Throws<InvalidOperationException>(act);
    }

    // The folder picker item is internal. This builds the same type the dialog returns.
    private static IStorageFolder CreateFolder(string path)
    {
        var assembly = typeof(AvaloniaObject).Assembly;
        var type = assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFolder")!;
        var constructor = type.GetConstructor([typeof(DirectoryInfo)])!;
        return (IStorageFolder)constructor.Invoke([new DirectoryInfo(path)]);
    }
}
