using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

// ReSharper disable CheckNamespace

namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

/// <summary>
/// Default framework dialog factory that will create instances of standard Windows dialogs.
/// </summary>
public class MessageBoxDialogFactory : DialogFactoryBase
{
    private readonly IMessageBoxApi _api;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public MessageBoxDialogFactory(IDialogFactory? chain = null)
        : this(chain, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia messagebox dialogs API.</param>
    internal MessageBoxDialogFactory(IDialogFactory? chain, IMessageBoxApi? api)
        : base(chain)
    {
        _api = api ?? new MessageBoxApi();
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings) =>
        settings switch
        {
            MessageBoxSettings s => await ShowMessageBoxDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            _ => await base.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true)
        };

    private async Task<bool?> ShowMessageBoxDialogAsync(IView? owner, MessageBoxSettings settings, AppDialogSettingsBase appSettings)
    {
        var apiSettings = new MessageBoxApiSettings()
        {
            Title = settings.Title,
            Text = settings.Text,
            Buttons = SyncButton(settings.Button),
            Icon = SyncIcon(settings.Icon),
            EnterDefaultButton = SyncDefaultEnter(settings.Button, settings.DefaultValue),
            EscDefaultButton = SyncDefaultEsc(settings.Button)
        };

        var ownerRef = owner.GetRef();
        if (ownerRef is Window ownerWin)
        {
            var result = await _api.ShowMessageBox(ownerWin, apiSettings);
            return result switch
            {
                ButtonResult.Yes => true,
                ButtonResult.Ok => true,
                ButtonResult.No => false,
                ButtonResult.Cancel => null,
                _ => null
            };
        }
        else
        {
            throw new InvalidCastException("Owner must be of type Window.");
        }
    }

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
