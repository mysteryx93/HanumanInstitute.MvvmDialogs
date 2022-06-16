using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
// ReSharper disable CheckNamespace

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Provides extensions to IDialogFactory.
/// </summary>
public static class DialogFactoryExtensions
{
    /// <summary>
    /// Registers Fluent handlers in the dialog factory chain.
    /// </summary>
    /// <param name="factory">The dialog factory to add handlers for.</param>
    /// <param name="messageBoxType">Specifies how MessageBox dialogs will be handled.</param>
    /// <returns>A new dialog factory that will fallback to the previous one.</returns>
    public static IDialogFactory AddFluent(this IDialogFactory factory, FluentMessageBoxType messageBoxType = FluentMessageBoxType.TaskDialog) =>
        new FluentDialogFactory(messageBoxType, factory);
}
