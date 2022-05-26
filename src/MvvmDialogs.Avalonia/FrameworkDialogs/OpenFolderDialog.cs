using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;

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
            Directory = Settings.InitialDirectory

            // d.ShowNewFolderButton = Settings.ShowNewFolderButton;
        };
}
