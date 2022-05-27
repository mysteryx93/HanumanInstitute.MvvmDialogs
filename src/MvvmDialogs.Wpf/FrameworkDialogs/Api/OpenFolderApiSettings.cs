
namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;

internal class OpenFolderApiSettings
{
    public string Description { get; set; } = string.Empty;
    public string? SelectedPath { get; set; } = string.Empty;
    public EventHandler? HelpRequest { get; set; }

    internal void ApplyTo(System.Windows.Forms.FolderBrowserDialog d)
    {
        d.Description = Description;
        d.SelectedPath = SelectedPath;
        if (HelpRequest != null)
        {
            d.HelpRequest += HelpRequest;
        }
    }
}
