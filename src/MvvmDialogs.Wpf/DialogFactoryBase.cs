
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public abstract class DialogFactoryBase : IDialogFactory, IDialogFactorySync
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
    public Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings)
    {
        if (owner is not null and not ViewWrapper) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(ViewWrapper)}");

        return ShowDialogAsync((ViewWrapper?)owner, settings);
    }

    /// <inheritdoc />
    public object? ShowDialog<TSettings>(IView? owner, TSettings settings)
    {
        if (owner is not null and not ViewWrapper) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(ViewWrapper)}");

        return ShowDialog((ViewWrapper?)owner, settings);
    }

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <returns>Return data specific to the dialog.</returns>
    public virtual Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings) =>
        Chain != null ? Chain.ShowDialogAsync(owner, settings) :
            throw new NotSupportedException($"There is no registered dialog for settings of type {typeof(TSettings).Name}.");

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <returns>Return data specific to the dialog.</returns>
    public virtual object? ShowDialog<TSettings>(ViewWrapper? owner, TSettings settings) =>
        Chain != null ? Chain.AsSync().ShowDialog(owner, settings) :
            throw new NotSupportedException($"There is no registered dialog for settings of type {typeof(TSettings).Name}.");
}
