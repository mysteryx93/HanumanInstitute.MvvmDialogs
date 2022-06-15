
namespace HanumanInstitute.MvvmDialogs.Wpf.Api;

internal class OpenFileApiSettings : FileApiSettings
{
    public bool Multiselect { get; set; }
    public bool ReadOnlyChecked { get; set; }
    public bool ShowReadOnly { get; set; }

    internal void ApplyTo(System.Windows.Forms.OpenFileDialog d)
    {
        base.ApplyTo(d);
        d.Multiselect = Multiselect;
        d.ReadOnlyChecked = ReadOnlyChecked;
        d.ShowReadOnly = ShowReadOnly;
    }
}
