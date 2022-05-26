using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.CustomSaveFileDialog;

public class CustomFrameworkDialogFactory : FrameworkDialogFactory
{
    public override IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
    {
        var s2 = (AppDialogSettings)appSettings;
        return settings switch
        {
            SaveFileDialogSettings s => (IFrameworkDialog<TResult>)new CustomSaveFileDialog(s),
            _ => base.Create<TSettings, TResult>(settings, appSettings)
        };
    }
}
