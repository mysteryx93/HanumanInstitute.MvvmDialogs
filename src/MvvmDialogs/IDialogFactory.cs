namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public interface IDialogFactory
{
    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <typeparam name="TSettings">The type of settings to use for this dialog.</typeparam>
    /// <returns>Return data specific to the dialog.</returns>
    Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings);

    /// <summary>
    /// Gets or sets a reference to the <see cref="IDialogManager"/>. Will only be set to the root factory in the chain.
    /// </summary>
    IDialogManager? DialogManager { get; set; }

    /// <summary>
    /// If the dialog is not handled by this class, calls this other handler next.
    /// </summary>
    IDialogFactory? Chain { get; }

    /// <summary>
    /// A reference to the top of the IDialogFactory chain, used to display message boxes.
    /// </summary>
    IDialogFactory ChainTop { get; }
}
