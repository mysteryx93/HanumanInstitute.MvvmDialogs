namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

/// <summary>
/// Default framework dialog factory that will create instances of standard framework dialogs.
/// </summary>
public class DialogHostDialogFactory : DialogFactoryBase
{
    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogHostDialogFactory(IDialogFactory? chain = null)
        : base(chain)
    {
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings) =>
        settings switch
        {
            // MessageBoxSettings s => await ShowMessageBoxDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            DialogHostSettings s => await ShowDialogHostAsync(owner, s, appSettings), 
            _ => await base.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true)
        };

    private async Task<object?> ShowDialogHostAsync(IView? owner, DialogHostSettings settings, AppDialogSettingsBase appSettings)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var view = new DialogHostView(settings);
        if (view.ViewModel != null)
        {
            GetDialogManager().HandleDialogEvents(view.ViewModel, view);    
        }
        
        await view.ShowDialogAsync(owner).ConfigureAwait(true);
        return view.DialogResult;
    }
    
    // private Task<bool?> ShowMessageBoxAsync(IView? owner, MessageBoxSettings settings, AppDialogSettingsBase appSettings)
    // {
    //     return Task.FromResult<bool?>(null);
    // }
}
