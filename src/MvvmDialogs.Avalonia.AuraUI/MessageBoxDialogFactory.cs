using HanumanInstitute.MvvmDialogs.Avalonia.AuraUI;
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
    public override async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            MessageBoxSettings s => await ShowMessageBoxDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            _ => await base.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true)
        };

    private async Task<bool?> ShowMessageBoxDialogAsync(ViewWrapper? owner, MessageBoxSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new MessageBoxApiSettings()
        {
            Title = settings.Title,
            Text = settings.Text
        };
        if (settings.Button == MessageBoxButton.Ok)
        {
            await _api.ShowMessageAsync(owner?.Ref, apiSettings).ConfigureAwait(true);
            return true;
        }
        else
        {
            apiSettings.OkButtonContent = GetButtonText(settings.Button, true);
            apiSettings.CancelButtonContent = GetButtonText(settings.Button, false);
            return await _api.ShowMessageBoxAsync(owner?.Ref, apiSettings).ConfigureAwait(true);
        }
    }

    private static string GetButtonText(MessageBoxButton buttons, bool value) => buttons switch
    {
        MessageBoxButton.OkCancel when value => "Ok",
        MessageBoxButton.OkCancel when !value => "Cancel",
        MessageBoxButton.YesNo when value => "Yes",
        MessageBoxButton.YesNo when !value => "No",
        MessageBoxButton.YesNoCancel when value => "Yes",
        MessageBoxButton.YesNoCancel when !value => "No",
        _ => string.Empty
    };
}
