using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs.Api;
using Win32FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs;

/// <summary>
/// Class wrapping <see cref="System.Windows.Forms.FolderBrowserDialog"/>.
/// </summary>
internal class OpenFolderDialog : FrameworkDialogBase<OpenFolderDialogSettings, string?>
{
    /// <inheritdoc />
    public OpenFolderDialog(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
        : base(settings, appSettings, pathInfo, api)
    {
    }

    /// <inheritdoc />
    public override Task<string?> ShowDialogAsync(WindowWrapper owner) =>
        owner.Ref.RunUiAsync(() => ShowDialog(owner));

    /// <inheritdoc />
    public override string? ShowDialog(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        return Api.ShowOpenFolderDialog(owner.Ref, apiSettings);
    }

    private OpenFolderApiSettings GetApiSettings() =>
        new()
        {
            Description = Settings.Title,
            SelectedPath = Settings.InitialPath,
            HelpRequest = Settings.HelpRequest
        };
}
