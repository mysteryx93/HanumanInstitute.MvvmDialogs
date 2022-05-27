using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;
using Win32Button = System.Windows.MessageBoxButton;
using Win32Image = System.Windows.MessageBoxImage;
using Win32Result = System.Windows.MessageBoxResult;
using Win32Options = System.Windows.MessageBoxOptions;
using Win32MessageBox = System.Windows.MessageBox;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

/// <summary>
/// Class wrapping <see cref="System.Windows.MessageBox"/>.
/// </summary>
internal class MessageBox : FrameworkDialogBase<MessageBoxSettings, bool?>
{
    /// <inheritdoc />
    public MessageBox(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, MessageBoxSettings settings, AppDialogSettings appSettings)
        : base(settings, appSettings, pathInfo, api)
    {
    }

    /// <inheritdoc />
    public override Task<bool?> ShowDialogAsync(WindowWrapper owner) =>
        owner.Ref.RunUiAsync(() => ShowDialog(owner));

    /// <inheritdoc />
    public override bool? ShowDialog(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        var button = Api.ShowMessageBox(owner.Ref, apiSettings);
        var result = button switch
        {
            Win32Result.Yes => true,
            Win32Result.OK => true,
            Win32Result.No => false,
            Win32Result.Cancel => null,
            _ => (bool?)null
        };
        return result;
    }

    // Convert platform-agnostic types into Win32 types.

    private MessageBoxApiSettings GetApiSettings() =>
        new()
        {
            MessageBoxText = Settings.Text,
            Caption = Settings.Title,
            Buttons = SyncButton(Settings.Button),
            Icon = SyncIcon(Settings.Icon),
            DefaultButton = SyncDefault(Settings.Button, Settings.DefaultValue),
            Options = SyncOptions()
        };

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
        EvalOption(AppSettings.MessageBoxDefaultDesktopOnly, Win32Options.DefaultDesktopOnly) |
        EvalOption(AppSettings.MessageBoxRightToLeft, Win32Options.RightAlign) |
        EvalOption(AppSettings.MessageBoxRightToLeft, Win32Options.RtlReading) |
        EvalOption(AppSettings.MessageBoxServiceNotification, Win32Options.ServiceNotification);

    private static Win32Options EvalOption(bool cond, Win32Options option) =>
        cond ? option : Win32Options.None;
}
