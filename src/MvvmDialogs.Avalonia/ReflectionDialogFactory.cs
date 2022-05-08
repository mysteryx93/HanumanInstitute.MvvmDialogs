using Avalonia.Controls;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc />
public class ReflectionDialogFactory : ReflectionDialogFactoryBase<Window>
{
    /// <inheritdoc />
    protected override IWindow CreateWrapper(Window window) => window.AsWrapper();
}
