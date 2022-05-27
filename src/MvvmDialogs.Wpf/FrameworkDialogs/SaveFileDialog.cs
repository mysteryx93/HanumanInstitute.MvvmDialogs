using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;
using Win32SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

/// <summary>
/// Class wrapping <see cref="System.Windows.Forms.SaveFileDialog"/>.
/// </summary>
internal class SaveFileDialog : FileDialogBase<SaveFileDialogSettings, string?>
{
    /// <inheritdoc />
    public SaveFileDialog(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, SaveFileDialogSettings settings, AppDialogSettings appSettings)
        : base(api, pathInfo, settings, appSettings)
    {
    }

    /// <inheritdoc />
    public override Task<string?> ShowDialogAsync(WindowWrapper owner) =>
        owner.Ref.RunUiAsync(() => ShowDialog(owner));

    /// <inheritdoc />
    public override string? ShowDialog(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        return Api.ShowSaveFileDialog(owner.Ref, apiSettings);
    }

    private SaveFileApiSettings GetApiSettings()
    {
        var d = new SaveFileApiSettings()
        {
            CheckFileExists = Settings.CheckFileExists,
            CreatePrompt = Settings.CreatePrompt,
            OverwritePrompt = Settings.OverwritePrompt
        };
        AddSharedSettings(d);
        return d;
    }
}
