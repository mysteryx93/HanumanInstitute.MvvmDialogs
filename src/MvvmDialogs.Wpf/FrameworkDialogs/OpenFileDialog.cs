using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;
using Win32CustomPlace = System.Windows.Forms.FileDialogCustomPlace;
using Win32CustomPlaces = Microsoft.Win32.FileDialogCustomPlaces;
using Win32OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

/// <summary>
/// Class wrapping <see cref="System.Windows.Forms.OpenFileDialog"/>.
/// </summary>
internal class OpenFileDialog : FileDialogBase<OpenFileDialogSettings, string[]>
{
    /// <inheritdoc />
    public OpenFileDialog(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, OpenFileDialogSettings settings, AppDialogSettings appSettings)
        : base(api, pathInfo, settings, appSettings)
    {
    }

    /// <inheritdoc />
    public override Task<string[]> ShowDialogAsync(WindowWrapper owner) =>
        owner.Ref.RunUiAsync(() => ShowDialog(owner));

    /// <inheritdoc />
    public override string[] ShowDialog(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        return Api.ShowOpenFileDialog(owner.Ref, apiSettings);
    }

    private OpenFileApiSettings GetApiSettings()
    {
        var d = new OpenFileApiSettings()
        {
            Multiselect = Settings.AllowMultiple ?? false,
            ReadOnlyChecked = Settings.ReadOnlyChecked,
            ShowReadOnly = Settings.ShowReadOnly
        };
        AddSharedSettings(d);
        return d;
    }
}
