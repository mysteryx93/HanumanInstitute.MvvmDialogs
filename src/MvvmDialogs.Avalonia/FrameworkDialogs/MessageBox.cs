using System.Threading.Tasks;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;

/// <summary>
/// Class wrapping <see cref="MessageBoxManager"/>.
/// </summary>
internal class MessageBox : FrameworkDialogBase<MessageBoxSettings, bool?>
{
    /// <inheritdoc />
    public MessageBox(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, MessageBoxSettings settings, AppDialogSettings appSettings)
        : base(api, pathInfo, settings, appSettings)
    {
    }

    /// <inheritdoc />
    public override async Task<bool?> ShowDialogAsync(WindowWrapper owner)
    {
        var apiSettings = GetApiSettings();
        var result = await Api.ShowMessageBox(owner.Ref, apiSettings).ConfigureAwait(false);

        return result switch
        {
            ButtonResult.Yes => true,
            ButtonResult.Ok => true,
            ButtonResult.No => false,
            ButtonResult.Cancel => null,
            _ => null
        };
    }

    private MessageBoxApiSettings GetApiSettings() =>
        new()
        {
            Title = Settings.Title,
            Text = Settings.Text,
            Buttons = SyncButton(Settings.Button),
            Icon = SyncIcon(Settings.Icon),
            EnterDefaultButton = SyncDefaultEnter(Settings.Button, Settings.DefaultValue),
            EscDefaultButton = SyncDefaultEsc(Settings.Button)
        };

    // Convert platform-agnostic types into Win32 types.

    private static ButtonEnum SyncButton(MessageBoxButton value) =>
        (value) switch
        {
            MessageBoxButton.Ok => ButtonEnum.Ok,
            MessageBoxButton.YesNo => ButtonEnum.YesNo,
            MessageBoxButton.OkCancel => ButtonEnum.OkCancel,
            MessageBoxButton.YesNoCancel => ButtonEnum.YesNoCancel,
            _ => ButtonEnum.Ok
        };

    private static Icon SyncIcon(MessageBoxImage value) =>
        (value) switch
        {
            MessageBoxImage.None => Icon.None,
            MessageBoxImage.Error => Icon.Error,
            MessageBoxImage.Exclamation => Icon.Warning,
            MessageBoxImage.Information => Icon.Info,
            MessageBoxImage.Stop => Icon.Stop,
            MessageBoxImage.Warning => Icon.Warning,
            _ => Icon.None
        };

    private static ClickEnum SyncDefaultEnter(MessageBoxButton buttons, bool? value) =>
        buttons switch
        {
            MessageBoxButton.Ok => ClickEnum.Ok,
            MessageBoxButton.OkCancel => value == true ? ClickEnum.Ok : ClickEnum.Cancel,
            MessageBoxButton.YesNo => value == true ? ClickEnum.Yes : ClickEnum.No,
            MessageBoxButton.YesNoCancel => value switch
            {
                true => ClickEnum.Yes,
                false => ClickEnum.No,
                null => ClickEnum.Cancel
            },
            _ => ClickEnum.Default
        };

    private static ClickEnum SyncDefaultEsc(MessageBoxButton buttons) =>
        buttons switch
        {
            MessageBoxButton.Ok => ClickEnum.Ok,
            MessageBoxButton.OkCancel => ClickEnum.Cancel,
            MessageBoxButton.YesNo => ClickEnum.No,
            MessageBoxButton.YesNoCancel => ClickEnum.Cancel,
            _ => ClickEnum.Default
        };
}
