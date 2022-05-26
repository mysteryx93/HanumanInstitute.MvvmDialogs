using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia.Api;

internal class FileApiSettings
{
    public string? Directory { get; set; }
    public string? InitialFileName { get; set; } = string.Empty;
    public List<FileDialogFilter> Filters { get; set; } = new List<FileDialogFilter>();
    public string? Title { get; set; }

    internal void ApplyTo(FileDialog d)
    {
        d.Directory = Directory;
        d.InitialFileName = InitialFileName;
        d.Filters = Filters;
        d.Title = Title;
    }
}
