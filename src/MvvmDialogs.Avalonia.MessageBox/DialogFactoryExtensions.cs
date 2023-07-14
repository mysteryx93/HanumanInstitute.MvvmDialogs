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
    /// <param name="mode">The message box type to show.</param>
    /// <returns>A new dialog factory that will fallback to the previous one.</returns>
    public static IDialogFactory AddMessageBox(this IDialogFactory factory, MessageBoxMode mode = MessageBoxMode.Window) =>
        new MessageBoxDialogFactory(factory)
        {
            Mode = mode
        };
}
