using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// DialogManager for Avalonia.
/// </summary>
public class DialogManager : DialogManagerBase
{
    /// <inheritdoc />
    public DialogManager(IFrameworkDialogFactory? frameworkDialogFactory = null, IDialogFactory? dialogFactory = null) :
        base(frameworkDialogFactory ?? new FrameworkDialogFactory(), dialogFactory ?? new ReflectionDialogFactory())
    {
    }
}
