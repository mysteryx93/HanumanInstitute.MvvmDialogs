using System;

namespace HanumanInstitute.MvvmDialogs.DialogTypeLocators;

/// <summary>
/// Interface responsible for creating dialogs.
/// </summary>
public interface IDialogFactory
{
    /// <summary>
    /// Creates a <see cref="IWindow"/> for specified type.
    /// </summary>
    IWindow Create(Type dialogType);
}