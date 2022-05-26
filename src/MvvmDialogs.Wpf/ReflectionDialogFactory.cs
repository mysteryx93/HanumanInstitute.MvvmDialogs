using System.Windows;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <inheritdoc />
public class ReflectionDialogFactory : ReflectionDialogFactoryBase<Window>
{
    /// <inheritdoc />
    protected override IWindow CreateWrapper(Window window) => window.AsWrapper();
}