using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.FolderBrowser;

namespace Demo.CustomFolderBrowserDialog;

public class CustomFrameworkDialogFactory : DefaultFrameworkDialogFactory
{
    public override IFrameworkDialog CreateFolderBrowserDialog(FolderBrowserDialogSettings settings)
    {
        return new CustomFolderBrowserDialog(settings);
    }
}
