using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.CustomMessageBox;

public class CustomFrameworkDialogFactory : FrameworkDialogFactory
{
    public override IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
    {
        var s2 = (AppDialogSettings)appSettings;
        return settings switch
        {
            TaskMessageBoxSettings s => (IFrameworkDialog<TResult>)new CustomMessageBox(s, s2),
            _ => base.Create<TSettings, TResult>(settings, appSettings)
        };
    }
}
