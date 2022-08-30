using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// Default framework dialog factory that will create instances of standard Windows dialogs.
/// </summary>
public class FluentDialogFactory : DialogFactoryBase
{
    private readonly IFluentApi _api;
    private readonly FluentMessageBoxType _messageBoxType;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="messageBoxType">Specifies how MessageBox dialogs will be handled.</param>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public FluentDialogFactory(FluentMessageBoxType messageBoxType = FluentMessageBoxType.TaskDialog, IDialogFactory? chain = null)
        : this(messageBoxType, chain, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="messageBoxType">Specifies how MessageBox dialogs will be handled.</param>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia messagebox dialogs API.</param>
    internal FluentDialogFactory(FluentMessageBoxType messageBoxType, IDialogFactory? chain, IFluentApi? api)
        : base(chain)
    {
        _api = api ?? new FluentApi();
        _messageBoxType = messageBoxType;
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            ContentDialogSettings s => await _api.ShowContentDialog(owner?.Ref, s).ConfigureAwait(true),
            TaskDialogSettings s => await _api.ShowTaskDialog(owner?.Ref, s).ConfigureAwait(true),
            MessageBoxSettings s when _messageBoxType == FluentMessageBoxType.ContentDialog => await ShowMessageBoxContentDialogAsync(owner?.Ref, s, appSettings)
                .ConfigureAwait(true),
            MessageBoxSettings s when _messageBoxType == FluentMessageBoxType.TaskDialog => await ShowMessageBoxTaskDialogAsync(owner?.Ref, s, appSettings).ConfigureAwait(true),
            _ => await base.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true)
        };

    private async Task<bool?> ShowMessageBoxContentDialogAsync(Window? owner, MessageBoxSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new ContentDialogSettings()
        {
            Title = settings.Title,
            Content = settings.Text,
            DefaultButton = ContentDialogButton.Primary
        };
        var yes = ContentDialogResult.Primary;
        var no = ContentDialogResult.Secondary;
        if (settings.Button == MessageBoxButton.Ok)
        {
            apiSettings.CloseButtonText = "OK";
            apiSettings.DefaultButton = ContentDialogButton.Close;
            yes = ContentDialogResult.None;
        }
        else if (settings.Button == MessageBoxButton.OkCancel)
        {
            apiSettings.PrimaryButtonText = "OK";
            apiSettings.CloseButtonText = "Cancel";
        }
        else if (settings.Button == MessageBoxButton.YesNo)
        {
            apiSettings.PrimaryButtonText = "Yes";
            apiSettings.SecondaryButtonText = "No";
        }
        else if (settings.Button == MessageBoxButton.YesNoCancel)
        {
            apiSettings.PrimaryButtonText = "Yes";
            apiSettings.SecondaryButtonText = "No";
            apiSettings.CloseButtonText = "Cancel";
        }

        var result = await _api.ShowContentDialog(owner, apiSettings).ConfigureAwait(true);
        return result == yes ? true : result == no ? false : null;
    }

    private async Task<bool?> ShowMessageBoxTaskDialogAsync(Window? owner, MessageBoxSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new TaskDialogSettings()
        {
            Title = settings.Title,
            Content = settings.Text,
            Buttons = SyncButton(settings.Button, settings.DefaultValue),

            // Icon = SyncIcon(settings.Icon)
        };

        var result = await _api.ShowTaskDialog(owner, apiSettings).ConfigureAwait(true);
        return (bool?)result;
        //return result switch
        //{
        //    TaskDialogStandardResult.None => null,
        //    TaskDialogStandardResult.Yes => true,
        //    TaskDialogStandardResult.No => false,
        //    TaskDialogStandardResult.Cancel => false,
        //    TaskDialogStandardResult.OK => true,
        //    TaskDialogStandardResult.Close => null,
        //    TaskDialogStandardResult.Retry => true,
        //    _ => null
        //};
    }

    private static TaskDialogButton[] SyncButton(MessageBoxButton value, bool? defaultValue) =>
        (value) switch
        {
            MessageBoxButton.YesNo => new[]
            {
                GetButton("Yes", true, defaultValue),
                GetButton("No", false, defaultValue)
            },
            MessageBoxButton.OkCancel => new[]
            {
                GetButton("OK", true, defaultValue),
                GetButton("Cancel", null, defaultValue)
            },
            MessageBoxButton.YesNoCancel => new[]
            {
                GetButton("Yes", true, defaultValue),
                GetButton("No", false, defaultValue),
                GetButton("Cancel", null, defaultValue)
            },
            _ => new[]
            {
                GetButton("OK", true, true)
            }
        };

    private static TaskDialogButton GetButton(string text, bool? value, bool? defaultValue) =>
        new TaskDialogButton(text, value)
        {
            IsDefault = defaultValue == value
        };
}
