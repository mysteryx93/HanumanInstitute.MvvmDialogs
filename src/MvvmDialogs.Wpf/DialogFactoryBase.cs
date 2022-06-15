
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public abstract class DialogFactoryBase : IDialogFactory, IDialogFactorySync
{
    /// <summary>
    /// If the dialog is not handled by this class, calls this other handler next.
    /// </summary>
    protected readonly IDialogFactory? Chain;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    protected DialogFactoryBase(IDialogFactory? chain)
    {
        Chain = chain;
    }

    /// <inheritdoc />
    public Task<object?> ShowDialogAsync<TSettings>(IWindow owner, TSettings settings, AppDialogSettingsBase appSettings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (owner is not WindowWrapper window) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(WindowWrapper)}");
        if (appSettings is not AppDialogSettings app) throw new ArgumentException($"{nameof(appSettings)} must be of type {nameof(AppDialogSettings)}");

        return ShowDialogAsync(window, settings, app);
    }

    /// <inheritdoc />
    public object? ShowDialog<TSettings>(IWindow owner, TSettings settings, AppDialogSettingsBase appSettings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (owner is not WindowWrapper window) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(WindowWrapper)}");
        if (appSettings is not AppDialogSettings app) throw new ArgumentException($"{nameof(appSettings)} must be of type {nameof(AppDialogSettings)}");

        return ShowDialogAsync(window, settings, app);
    }

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <returns>Return data specific to the dialog.</returns>
    public virtual Task<object?> ShowDialogAsync<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        Chain != null ? Chain.ShowDialogAsync(owner, settings, appSettings) :
            throw new NotSupportedException($"There is no registered dialog for settings of type {typeof(TSettings).Name}.");

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <returns>Return data specific to the dialog.</returns>
    public virtual object? ShowDialog<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        Chain != null ? Chain.AsSync().ShowDialog(owner, settings, appSettings) :
            throw new NotSupportedException($"There is no registered dialog for settings of type {typeof(TSettings).Name}.");
}
