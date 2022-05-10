using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;

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
