using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// Default framework dialog factory that will create instances of standard Windows dialogs.
/// </summary>
public class FluentDialogFactory : DialogFactoryBase
{
    private readonly FluentMessageBoxType _messageBoxType;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="messageBoxType">Specifies how MessageBox dialogs will be handled.</param>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public FluentDialogFactory(FluentMessageBoxType messageBoxType = FluentMessageBoxType.TaskDialog, IDialogFactory? chain = null)
        : base(chain)
    {
        _messageBoxType = messageBoxType;
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings) =>
        settings switch
        {
            ContentDialogSettings s => await ShowContentDialogAsync(owner, s).ConfigureAwait(true),
            TaskDialogSettings s => await ShowTaskDialogAsync(owner, s).ConfigureAwait(true),
            MessageBoxSettings s when _messageBoxType == FluentMessageBoxType.ContentDialog => await ShowMessageBoxContentDialogAsync(owner, s)
                .ConfigureAwait(true),
            MessageBoxSettings s when _messageBoxType == FluentMessageBoxType.TaskDialog => await ShowMessageBoxTaskDialogAsync(owner, s).ConfigureAwait(true),
            _ => await base.ShowDialogAsync(owner, settings).ConfigureAwait(true)
        };

    private async Task<FAContentDialogResult> ShowContentDialogAsync(IView? owner, ContentDialogSettings settings)
    {
        ArgumentNullException.ThrowIfNull(owner);
        var view = new FluentContentView(settings);
        if (view.ViewModel != null)
        {
            GetDialogManager().HandleDialogEvents(view.ViewModel, view);
        }

        await view.ShowDialogAsync(owner).ConfigureAwait(true);
        return view.DialogResult;
    }

    private async Task<object?> ShowTaskDialogAsync(IView? owner, TaskDialogSettings settings)
    {
        ArgumentNullException.ThrowIfNull(owner);
        var view = new FluentTaskView(settings);
        if (view.ViewModel != null)
        {
            GetDialogManager().HandleDialogEvents(view.ViewModel, view);
        }

        await view.ShowDialogAsync(owner).ConfigureAwait(true);
        return view.DialogResult;
    }

    private async Task<bool?> ShowMessageBoxContentDialogAsync(IView? owner, MessageBoxSettings settings)
    {
        var apiSettings = new ContentDialogSettings()
        {
            Title = settings.Title,
            Content = settings.Content,
            DefaultButton = FAContentDialogButton.Primary
        };
        var yes = FAContentDialogResult.Primary;
        var no = FAContentDialogResult.Secondary;
        if (settings.Button == MessageBoxButton.Ok)
        {
            apiSettings.CloseButtonText = "OK";
            apiSettings.DefaultButton = FAContentDialogButton.Close;
            yes = FAContentDialogResult.None;
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

        var result = await ShowContentDialogAsync(owner, apiSettings).ConfigureAwait(true);
        return result == yes ? true : result == no ? false : null;
    }

    private async Task<bool?> ShowMessageBoxTaskDialogAsync(IView? owner, MessageBoxSettings settings)
    {
        var apiSettings = new TaskDialogSettings()
        {
            Title = settings.Title,
            Content = settings.Content,
            Buttons = SyncButton(settings.Button, settings.DefaultValue),

            // Icon = SyncIcon(settings.Icon)
        };

        var result = await ShowTaskDialogAsync(owner, apiSettings).ConfigureAwait(true);
        return result as bool?; // It can be TaskDialogStandardResult.None if we press Escape
    }

    private static FATaskDialogButton[] SyncButton(MessageBoxButton value, bool? defaultValue) =>
        (value) switch
        {
            MessageBoxButton.YesNo =>
            [
                GetButton("Yes", true, defaultValue),
                GetButton("No", false, defaultValue)
            ],
            MessageBoxButton.OkCancel =>
            [
                GetButton("OK", true, defaultValue),
                GetButton("Cancel", null, defaultValue)
            ],
            MessageBoxButton.YesNoCancel =>
            [
                GetButton("Yes", true, defaultValue),
                GetButton("No", false, defaultValue),
                GetButton("Cancel", null, defaultValue)
            ],
            _ =>
            [
                GetButton("OK", true, true)
            ]
        };

    private static FATaskDialogButton GetButton(string text, bool? value, bool? defaultValue) =>
        new(text, value)
        {
            IsDefault = defaultValue == value
        };
}
