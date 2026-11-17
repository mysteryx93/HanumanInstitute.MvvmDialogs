using HanumanInstitute.MvvmDialogs.Wpf.Api;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using System.Text;
using Win32Button = System.Windows.MessageBoxButton;
using Win32Image = System.Windows.MessageBoxImage;
using Win32Result = System.Windows.MessageBoxResult;
using Win32Options = System.Windows.MessageBoxOptions;
using MessageBoxButton = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton;
using MessageBoxImage = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxImage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Handles OpenFileDialog, SaveFileDialog and OpenFolderDialog for WPF.
/// </summary>
public class DialogFactory : DialogFactoryBase
{
    private readonly IFrameworkDialogsApi _api;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogFactory(IDialogFactory? chain = null)
        : this(chain, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing WPF framework dialogs.</param>
    internal DialogFactory(IDialogFactory? chain, IFrameworkDialogsApi? api)
        : base(chain)
    {
        _api = api ?? new FrameworkDialogsApi();
    }

    /// <summary>
    /// Gets or sets whether message boxes are displayed right-to-left (RightAlign+RtlReading).
    /// </summary>
    public bool MessageBoxRightToLeft { get; set; }

    /// <summary>
    /// Gets or sets whether to display on the default desktop of the interactive window station. Specifies that the message box is displayed from a .NET Windows Service application in order to notify the user of an event.
    /// </summary>
    public bool MessageBoxDefaultDesktopOnly { get; set; }

    /// <summary>
    /// Gets or sets whether to display on the currently active desktop even if a user is not logged on to the computer. Specifies that the message box is displayed from a .NET Windows Service application in order to notify the user of an event.
    /// </summary>
    public bool MessageBoxServiceNotification { get; set; }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await UiExtensions.RunUiAsync(() => ShowOpenFolderDialog(owner, s)).ConfigureAwait(true),
            OpenFileDialogSettings s => await UiExtensions.RunUiAsync(() => ShowOpenFileDialog(owner, s)).ConfigureAwait(true),
            SaveFileDialogSettings s => await UiExtensions.RunUiAsync(() => ShowSaveFileDialog(owner, s)).ConfigureAwait(true),
            MessageBoxSettings s => await UiExtensions.RunUiAsync(() => ShowMessageBox(owner, s)).ConfigureAwait(true),
            _ => base.ShowDialogAsync(owner, settings)
        };

    /// <inheritdoc />
    public override object? ShowDialog<TSettings>(ViewWrapper? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => ShowOpenFolderDialog(owner, s),
            OpenFileDialogSettings s => ShowOpenFileDialog(owner, s),
            SaveFileDialogSettings s => ShowSaveFileDialog(owner, s),
            MessageBoxSettings s => ShowMessageBox(owner, s),
            _ => base.ShowDialog(owner, settings)
        };

    private IReadOnlyList<IDialogStorageFolder> ShowOpenFolderDialog(ViewWrapper? owner, OpenFolderDialogSettings settings)
    {
        var apiSettings = new OpenFolderApiSettings()
        {
            Description = settings.Title,
            SelectedPath = settings.SuggestedStartLocation?.TryGetLocalPath(),
            HelpRequest = settings.HelpRequest
        };

        return _api.ShowOpenFolderDialog(owner?.Ref, apiSettings);
    }

    private IReadOnlyList<IDialogStorageFile> ShowOpenFileDialog(ViewWrapper? owner, OpenFileDialogSettings settings)
    {
        var apiSettings = new OpenFileApiSettings()
        {
            CheckFileExists = true,
            Multiselect = settings.AllowMultiple ?? false,
            ReadOnlyChecked = settings.ReadOnlyChecked,
            ShowReadOnly = settings.ShowReadOnly
        };
        AddSharedSettings(apiSettings, settings);

        var result = _api.ShowOpenFileDialog(owner?.Ref, apiSettings) ?? Array.Empty<IDialogStorageFile>();
        return WithSelectedFilter(settings, result, apiSettings.FilterIndex);
    }

    private IDialogStorageFile? ShowSaveFileDialog(ViewWrapper? owner, SaveFileDialogSettings settings)
    {
        var apiSettings = new SaveFileApiSettings()
        {
            DefaultExt = settings.DefaultExtension ?? string.Empty
        };
        if (settings.ShowOverwritePrompt.HasValue)
        {
            apiSettings.OverwritePrompt = settings.ShowOverwritePrompt.Value;
        }
        AddSharedSettings(apiSettings, settings);

        var result = _api.ShowSaveFileDialog(owner?.Ref, apiSettings);
        return WithSelectedFilter(settings, result, apiSettings.FilterIndex);
    }

    private void AddSharedSettings(FileApiSettings d, FileDialogSettings s)
    {
        d.InitialDirectory = s.SuggestedStartLocation?.TryGetLocalPath() ?? string.Empty;
        d.FileName = s.SuggestedFileName;
        d.DereferenceLinks = s.DereferenceLinks;
        d.Filter = SyncFilters(s.Filters);
        d.FilterIndex = FilterIndex(s);
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
    private static string SyncFilters(IList<FileFilter> filters)
    {
        var result = new StringBuilder();
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

    // Win32 filter indexes are 1-based. Zero leaves the dialog's own default.
    private static int FilterIndex(FileDialogSettings settings)
    {
        if (settings.SuggestedFileType == null)
        {
            return 0;
        }

        var index = settings.SuggestedFileType.IndexIn(settings.Filters);
        return index < 0 ? 0 : index + 1;
    }

    private static FileFilter? FilterAt(IList<FileFilter> filters, int filterIndex)
    {
        var index = filterIndex - 1;
        if (index < 0 || index >= filters.Count)
        {
            return null;
        }

        return filters[index];
    }

    // Records the chosen filter when the dialog returns a file.
    private static IDialogStorageFile? WithSelectedFilter(FileDialogSettings settings, IDialogStorageFile? file, int filterIndex)
    {
        settings.SelectedFileType = file == null ? null : FilterAt(settings.Filters, filterIndex);
        return file;
    }

    // Records the chosen filter when the dialog returns at least one file.
    private static IReadOnlyList<IDialogStorageFile> WithSelectedFilter(FileDialogSettings settings, IReadOnlyList<IDialogStorageFile> files, int filterIndex)
    {
        settings.SelectedFileType = files.Count == 0 ? null : FilterAt(settings.Filters, filterIndex);
        return files;
    }

    private bool? ShowMessageBox(ViewWrapper? owner, MessageBoxSettings settings)
    {
        var apiSettings = new MessageBoxApiSettings()
        {
            MessageBoxText = settings.Content,
            Caption = settings.Title,
            Buttons = SyncButton(settings.Button),
            Icon = SyncIcon(settings.Icon),
            DefaultButton = SyncDefault(settings.Button, settings.DefaultValue),
            Options = SyncOptions()
        };

        var button = _api.ShowMessageBox(owner?.Ref, apiSettings);
        return button switch
        {
            Win32Result.Yes => true,
            Win32Result.OK => true,
            Win32Result.No => false,
            Win32Result.Cancel => null,
            _ => null
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

    private Win32Options SyncOptions() =>
        EvalOption(MessageBoxDefaultDesktopOnly, Win32Options.DefaultDesktopOnly) |
        EvalOption(MessageBoxRightToLeft, Win32Options.RightAlign) |
        EvalOption(MessageBoxRightToLeft, Win32Options.RtlReading) |
        EvalOption(MessageBoxServiceNotification, Win32Options.ServiceNotification);

    private static Win32Options EvalOption(bool cond, Win32Options option) =>
        cond ? option : Win32Options.None;
}
