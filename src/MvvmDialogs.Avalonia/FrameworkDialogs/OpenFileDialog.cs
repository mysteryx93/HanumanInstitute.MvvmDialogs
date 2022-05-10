using System;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.Avalonia;
using AvaloniaOpenFileDialog = Avalonia.Controls.OpenFileDialog;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;

/// <summary>
/// Class wrapping <see cref="AvaloniaOpenFileDialog"/>.
/// </summary>
internal class OpenFileDialog : FileDialogBase<OpenFileDialogSettings, string[]>
{
    /// <inheritdoc />
    public OpenFileDialog(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, OpenFileDialogSettings settings, AppDialogSettings appSettings)
        : base(api, pathInfo, settings, appSettings)
    {
    }

    /// <inheritdoc />
    public override async Task<string[]> ShowDialogAsync(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        var result = await Api.ShowOpenFileDialog(owner.Ref, apiSettings).ConfigureAwait(false);
        return result ?? Array.Empty<string>();
    }

    private OpenFileApiSettings GetApiSettings()
    {
        var d = new OpenFileApiSettings()
        {
            AllowMultiple = Settings.AllowMultiple ?? false
            // d.ShowReadOnly = Settings.ShowReadOnly;
            // d.ReadOnlyChecked = Settings.ReadOnlyChecked;
        };
        AddSharedSettings(d);
        return d;
    }
}
