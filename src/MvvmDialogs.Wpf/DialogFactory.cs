using HanumanInstitute.MvvmDialogs.Wpf.Api;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using System.Text;
using Win32Button = System.Windows.MessageBoxButton;
using Win32Image = System.Windows.MessageBoxImage;
using Win32Result = System.Windows.MessageBoxResult;
using Win32Options = System.Windows.MessageBoxOptions;
using Win32MessageBox = System.Windows.MessageBox;
using MessageBoxButton = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton;
using MessageBoxImage = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxImage;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Handles OpenFileDialog, SaveFileDialog and OpenFolderDialog for Avalonia.
/// </summary>
public class DialogFactory : DialogFactoryBase
{
    private readonly IFrameworkDialogsApi _api;
    private readonly IPathInfoFactory _pathInfo;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogFactory(IDialogFactory? chain = null)
        : this(chain, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia framework dialogs.</param>
    /// <param name="pathInfo">Provides information about files and directories.</param>
    internal DialogFactory(IDialogFactory? chain, IFrameworkDialogsApi? api, IPathInfoFactory? pathInfo)
        : base(chain)
    {
        _api = api ?? new FrameworkDialogsApi();
        _pathInfo = pathInfo ?? new PathInfoFactory();
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await owner.Ref.RunUiAsync(() => ShowOpenFolderDialog(owner, s, appSettings)).ConfigureAwait(true),
            OpenFileDialogSettings s => await owner.Ref.RunUiAsync(() => ShowOpenFileDialog(owner, s, appSettings)).ConfigureAwait(true),
            SaveFileDialogSettings s => await owner.Ref.RunUiAsync(() => ShowSaveFileDialog(owner, s, appSettings)).ConfigureAwait(true),
            MessageBoxSettings s => await owner.Ref.RunUiAsync(() => ShowMessageBox(owner, s, appSettings)).ConfigureAwait(true),
            _ => base.ShowDialogAsync(owner, settings, appSettings)
        };

    /// <inheritdoc />
    public override object? ShowDialog<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => ShowOpenFolderDialog(owner, s, appSettings),
            OpenFileDialogSettings s => ShowOpenFileDialog(owner, s, appSettings),
            SaveFileDialogSettings s => ShowSaveFileDialog(owner, s, appSettings),
            MessageBoxSettings s => ShowMessageBox(owner, s, appSettings),
            _ => base.ShowDialog(owner, settings, appSettings)
        };

    private string? ShowOpenFolderDialog(WindowWrapper owner, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new OpenFolderApiSettings()
        {
            Description = settings.Title,
            SelectedPath = settings.InitialDirectory,
            HelpRequest = settings.HelpRequest
        };

        return _api.ShowOpenFolderDialog(owner.Ref, apiSettings);
    }

    private string[] ShowOpenFileDialog(WindowWrapper owner, OpenFileDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new OpenFileApiSettings()
        {
            CheckFileExists = true,
            Multiselect = settings.AllowMultiple ?? false,
            ReadOnlyChecked = settings.ReadOnlyChecked,
            ShowReadOnly = settings.ShowReadOnly
        };
        AddSharedSettings(apiSettings, settings);

        return _api.ShowOpenFileDialog(owner.Ref, apiSettings) ?? Array.Empty<string>();
    }

    private string? ShowSaveFileDialog(WindowWrapper owner, SaveFileDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new SaveFileApiSettings()
        {
            DefaultExt = settings.DefaultExtension
        };
        AddSharedSettings(apiSettings, settings);

        return _api.ShowSaveFileDialog(owner.Ref, apiSettings);
    }

    private void AddSharedSettings(FileApiSettings d, FileDialogSettings s)
    {
        d.InitialDirectory = s.InitialDirectory;
        d.FileName = s.InitialFile;
        d.DereferenceLinks = s.DereferenceLinks;
        d.Filter = SyncFilters(s.Filters);
        d.Title = s.Title;
        d.ShowHelp = s.HelpRequest != null;
        d.HelpRequest = s.HelpRequest;
    }

    /// <summary>
    /// Encodes the list of filters in the Win32 API format:
    /// "Image Files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
    /// </summary>
    /// <param name="filters">The list of filters to encode.</param>
    /// <returns>A string representation of the list compatible with Win32 API.</returns>
    private static string SyncFilters(List<FileFilter> filters)
    {
        StringBuilder result = new StringBuilder();
        foreach (var item in filters)
        {
            // Add separator.
            if (result.Length > 0)
            {
                result.Append('|');
            }

            // Get all extensions as a string.
            var extDesc = item.ExtensionsToString();
            // Get name including extensions.
            var name = item.NameToString(extDesc);
            // Add name+extensions for display.
            result.Append(name);
            // Add extensions again for the API.
            result.Append("|");
            result.Append(extDesc);
        }
        return result.ToString();
    }

    private bool? ShowMessageBox(WindowWrapper owner, MessageBoxSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new MessageBoxApiSettings()
        {
            MessageBoxText = settings.Text,
            Caption = settings.Title,
            Buttons = SyncButton(settings.Button),
            Icon = SyncIcon(settings.Icon),
            DefaultButton = SyncDefault(settings.Button, settings.DefaultValue),
            Options = SyncOptions(appSettings)
        };

        var button = _api.ShowMessageBox(owner.Ref, apiSettings);
        return button switch
        {
            Win32Result.Yes => true,
            Win32Result.OK => true,
            Win32Result.No => false,
            Win32Result.Cancel => null,
            _ => (bool?)null
        };

    }

    private static Win32Button SyncButton(MessageBoxButton value) =>
        (value) switch
        {
            MessageBoxButton.Ok => Win32Button.OK,
            MessageBoxButton.YesNo => Win32Button.YesNo,
            MessageBoxButton.OkCancel => Win32Button.OKCancel,
            MessageBoxButton.YesNoCancel => Win32Button.YesNoCancel,
            _ => Win32Button.OK
        };

    private static Win32Image SyncIcon(MessageBoxImage value) =>
        (value) switch
        {
            MessageBoxImage.None => Win32Image.None,
            MessageBoxImage.Error => Win32Image.Error,
            MessageBoxImage.Exclamation => Win32Image.Exclamation,
            MessageBoxImage.Information => Win32Image.Information,
            MessageBoxImage.Stop => Win32Image.Stop,
            MessageBoxImage.Warning => Win32Image.Warning,
            _ => Win32Image.None
        };

    private static Win32Result SyncDefault(MessageBoxButton buttons, bool? value) =>
        buttons switch
        {
            MessageBoxButton.Ok => Win32Result.OK,
            MessageBoxButton.YesNo => value == true ? Win32Result.Yes : Win32Result.No,
            MessageBoxButton.OkCancel => value == true ? Win32Result.OK : Win32Result.Cancel,
            MessageBoxButton.YesNoCancel => value switch
            {
                true => Win32Result.Yes,
                false => Win32Result.No,
                _ => Win32Result.Cancel
            },
            _ => Win32Result.None
        };

    private Win32Options SyncOptions(AppDialogSettings appSettings) =>
        EvalOption(appSettings.MessageBoxDefaultDesktopOnly, Win32Options.DefaultDesktopOnly) |
        EvalOption(appSettings.MessageBoxRightToLeft, Win32Options.RightAlign) |
        EvalOption(appSettings.MessageBoxRightToLeft, Win32Options.RtlReading) |
        EvalOption(appSettings.MessageBoxServiceNotification, Win32Options.ServiceNotification);

    private static Win32Options EvalOption(bool cond, Win32Options option) =>
        cond ? option : Win32Options.None;
}
