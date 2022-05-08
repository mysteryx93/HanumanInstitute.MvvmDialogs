using HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.CustomOpenFileDialog;

public class CustomFrameworkDialogFactory : FrameworkDialogFactory
{
    public override IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
    {
        var s2 = (AppDialogSettings)appSettings;
        return settings switch
        {
            OpenFileDialogSettings s => (IFrameworkDialog<TResult>)new CustomOpenFileDialog(s, s2),
            _ => base.Create<TSettings, TResult>(settings, appSettings)
        };
    }
}
