
namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;

internal class SaveFileApiSettings : FileApiSettings
{
    public bool CreatePrompt { get; set; }
    public bool OverwritePrompt { get; set; }

    internal void ApplyTo(System.Windows.Forms.SaveFileDialog d)
    {
        base.ApplyTo(d);
        d.CreatePrompt = CreatePrompt;
        d.OverwritePrompt = OverwritePrompt;
    }
}
