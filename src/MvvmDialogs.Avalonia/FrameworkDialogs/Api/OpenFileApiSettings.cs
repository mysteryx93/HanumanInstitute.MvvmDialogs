using AvaloniaOpenFileDialog = Avalonia.Controls.OpenFileDialog;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia.Api;

internal class OpenFileApiSettings : FileApiSettings
{
    public bool AllowMultiple { get; set; }

    internal void ApplyTo(AvaloniaOpenFileDialog d)
    {
        base.ApplyTo(d);
        d.AllowMultiple = AllowMultiple;
    }
}