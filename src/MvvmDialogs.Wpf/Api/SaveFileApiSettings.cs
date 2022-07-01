
namespace HanumanInstitute.MvvmDialogs.Wpf.Api;

internal class SaveFileApiSettings : FileApiSettings
{
    public bool CreatePrompt { get; set; }
    public bool OverwritePrompt { get; set; } = true;

    internal void ApplyTo(System.Windows.Forms.SaveFileDialog d)
    {
        base.ApplyTo(d);
        d.CreatePrompt = CreatePrompt;
        d.OverwritePrompt = OverwritePrompt;
    }
}
