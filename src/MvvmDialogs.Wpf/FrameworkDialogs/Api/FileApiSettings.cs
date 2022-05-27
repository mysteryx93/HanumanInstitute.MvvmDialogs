using System.Windows.Forms;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;

internal class FileApiSettings
{
    public string DefaultExt { get; set; } = string.Empty;
    public bool AddExtension { get; set; } = true;
    public bool CheckFileExists { get; set; } = false;
    public bool CheckPathExists { get; set; } = true;
    public FileDialogCustomPlacesCollection CustomPlaces { get; } = new FileDialogCustomPlacesCollection();
    public string InitialDirectory { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string[] FileNames { get; set; } = Array.Empty<string>();
    public bool DereferenceLinks { get; set; } = true;
    public string Filter { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool ShowHelp { get; set; } = false;
    public EventHandler? HelpRequest { get; set; }

    internal void ApplyTo(FileDialog d)
    {
        d.DefaultExt = DefaultExt;
        d.AddExtension = AddExtension;
        d.CheckFileExists = CheckFileExists;
        d.CheckPathExists = CheckPathExists;
        foreach (var item in CustomPlaces)
        {
            d.CustomPlaces.Add(item);
        }
        d.InitialDirectory = InitialDirectory;
        d.FileName = FileName;
        d.DereferenceLinks = DereferenceLinks;
        d.Filter = Filter;
        d.Title = Title;
        d.ShowHelp = ShowHelp;
        if (HelpRequest != null)
        {
            d.HelpRequest += HelpRequest;
        }
    }
}
