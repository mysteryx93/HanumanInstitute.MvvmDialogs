using HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;
// ReSharper disable CheckNamespace

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Provides extensions to IDialogFactory.
/// </summary>
public static class DialogFactoryExtensions
{
    /// <summary>
    /// Registers MessageBox handlers in the dialog factory chain.
    /// </summary>
    /// <param name="factory">The dialog factory to add handlers for.</param>
    /// <returns>A new dialog factory that will fallback to the previous one.</returns>
    public static IDialogFactory AddMessageBoxAuraUI(this IDialogFactory factory) =>
        new MessageBoxDialogFactory(factory);
}
