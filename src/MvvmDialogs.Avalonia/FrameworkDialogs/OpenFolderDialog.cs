using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.Avalonia.FrameworkDialogs.Api;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using AvaloniaOpenFolderDialog = Avalonia.Controls.OpenFolderDialog;

namespace HanumanInstitute.MvvmDialogs.Avalonia.FrameworkDialogs;

/// <summary>
/// Class wrapping <see cref="OpenFolderDialog"/>.
/// </summary>
internal class OpenFolderDialog : FrameworkDialogBase<OpenFolderDialogSettings, string?>
{
    /// <inheritdoc />
    public OpenFolderDialog(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
        : base(api, pathInfo, settings, appSettings)
    {
    }

    /// <inheritdoc />
    public override async Task<string?> ShowDialogAsync(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        var result = await Api.ShowOpenFolderDialog(owner.Ref, apiSettings).ConfigureAwait(false);
        return result;
    }

    private OpenFolderApiSettings GetApiSettings() =>
        new()
        {
            Title = Settings.Title,
            Directory = Settings.InitialPath

            // d.ShowNewFolderButton = Settings.ShowNewFolderButton;
        };
}