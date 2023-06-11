
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public abstract class DialogFactoryBase : IDialogFactory
{
    /// <inheritdoc />
    public IDialogManager? DialogManager { get; set; }

    /// <inheritdoc />
    public IDialogFactory? Chain { get; }

    /// <inheritdoc />
    public IDialogFactory ChainTop { get; private set; }

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
    public virtual async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings)
    {
        if (appSettings is not AppDialogSettings app) throw new ArgumentException($"{nameof(appSettings)} must be of type {nameof(AppDialogSettings)}");
        return Chain != null ? await Chain.ShowDialogAsync(owner, settings, app).ConfigureAwait(true) :
            throw new NotSupportedException($"There is no registered dialog in IDialogFactory for settings of type {typeof(TSettings).Name}.");
    }

    /// <summary>
    /// Returns the <see cref="IDialogManager"/> set on the root factory.
    /// </summary>
    /// <returns>The <see cref="IDialogManager"/>.</returns>
    public IDialogManager GetDialogManager() =>
        ChainTop.DialogManager ?? throw new NullReferenceException("Missing IDialogManager reference in root DialogFactory.");

    // /// <summary>
    // /// Opens a framework dialog with specified owner.
    // /// </summary>
    // /// <param name="owner">Handle to the window that owns the dialog.</param>
    // /// <param name="settings">The settings for the framework dialog.</param>
    // /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    // /// <returns>Return data specific to the dialog.</returns>
    // public virtual async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings, AppDialogSettings appSettings) =>
    //     Chain != null ? await Chain.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true) :
    //         throw new NotSupportedException($"There is no registered dialog in IDialogFactory for settings of type {typeof(TSettings).Name}.");
}
