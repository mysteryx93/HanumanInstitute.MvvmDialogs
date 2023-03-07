
// ReSharper disable CheckNamespace

using HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Provides extensions to IDialogFactory.
/// </summary>
public static class DialogFactoryExtensions
{
    /// <summary>
    /// Registers DialogHost handlers in the dialog factory chain.
    /// </summary>
    /// <param name="factory">The dialog factory to add handlers for.</param>
    /// <returns>A new dialog factory that will fallback to the previous one.</returns>
    public static IDialogFactory AddDialogHost(this IDialogFactory factory) =>
        new DialogHostDialogFactory(factory);
}
