
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.PathInfo;

namespace Demo.Avalonia.CustomOpenFolderDialog
{
    public static class DialogFactoryExtensions
    {
        public static IDialogFactory AddCustomOpenFolder(this IDialogFactory factory) => new CustomDialogFactory(new PathInfoFactory(), factory);
    }
}
