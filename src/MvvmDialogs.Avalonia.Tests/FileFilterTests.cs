using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class FileFilterTests
{
    [Fact]
    public void IndexIn_DuplicateName_ReturnsExactInstance()
    {
        var first = new FileFilter("Text", "txt");
        var second = new FileFilter("Text", "md");
        IList<FileFilter> filters = [first, second];

        var index = second.IndexIn(filters);

        Assert.Equal(1, index);
    }

    [Fact]
    public void IndexIn_SharedNameWithoutInstance_ReturnsMinusOne()
    {
        var first = new FileFilter("Text", "txt");
        var second = new FileFilter("Text", "md");
        var requested = new FileFilter("Text", "log");
        IList<FileFilter> filters = [first, second];

        var index = requested.IndexIn(filters);

        Assert.Equal(-1, index);
    }

    [Fact]
    public void IndexIn_UniqueName_ReturnsThatFilter()
    {
        var text = new FileFilter("Text", "txt");
        var image = new FileFilter("Image", "png");
        var requested = new FileFilter("Image", "png");
        IList<FileFilter> filters = [text, image];

        var index = requested.IndexIn(filters);

        Assert.Equal(1, index);
    }
}
