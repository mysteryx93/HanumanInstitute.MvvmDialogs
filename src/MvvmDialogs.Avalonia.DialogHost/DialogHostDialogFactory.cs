
namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

/// <summary>
/// Default framework dialog factory that will create instances of standard framework dialogs.
/// </summary>
public class DialogHostDialogFactory : DialogFactoryBase
{
    private readonly IDialogHostApi _api;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogHostDialogFactory(IDialogFactory? chain = null)
        : this(chain, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia messagebox dialogs API.</param>
    internal DialogHostDialogFactory(IDialogFactory? chain, IDialogHostApi? api)
        : base(chain)
    {
        _api = api ?? new DialogHostApi();
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings) =>
        settings switch
        {
            // MessageBoxSettings s => await ShowMessageBoxDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            DialogHostSettings s => await ShowDialogHostAsync(owner, s, appSettings), 
            _ => await base.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true)
        };

    private Task<object?> ShowDialogHostAsync(IView? owner, DialogHostSettings settings, AppDialogSettingsBase appSettings) => 
        _api.ShowDialogHostAsync(owner.GetRef(), settings);
    
    // private Task<bool?> ShowMessageBoxAsync(IView? owner, MessageBoxSettings settings, AppDialogSettingsBase appSettings)
    // {
    //     return Task.FromResult<bool?>(null);
    // }
}
