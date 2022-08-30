
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public abstract class DialogFactoryBase : IDialogFactory
{
    /// <summary>
    /// If the dialog is not handled by this class, calls this other handler next.
    /// </summary>
    protected readonly IDialogFactory? Chain;

    /// <summary>
    /// A reference to the top of the IDialogFactory chain, used to display message boxes.
    /// </summary>
    protected IDialogFactory ChainTop { get; set; }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    protected DialogFactoryBase(IDialogFactory? chain)
    {
        Chain = chain;
        ChainTop = this;

        // Set ChainTop recursively.
        var item = chain;
        while (item is DialogFactoryBase f)
        {
            f.ChainTop = this;
            item = f.Chain;
        }
    }

    /// <inheritdoc />
    public Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings)
    {
        if (owner is not null and not ViewWrapper) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(ViewWrapper)}");
        if (appSettings is not AppDialogSettings app) throw new ArgumentException($"{nameof(appSettings)} must be of type {nameof(AppDialogSettings)}");
        return ShowDialogAsync((ViewWrapper?)owner, settings, app);
    }

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <returns>Return data specific to the dialog.</returns>
    public virtual async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings, AppDialogSettings appSettings) =>
        Chain != null ? await Chain.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true) :
            throw new NotSupportedException($"There is no registered dialog in IDialogFactory for settings of type {typeof(TSettings).Name}.");
}
