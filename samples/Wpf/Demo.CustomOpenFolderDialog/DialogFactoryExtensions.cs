
using HanumanInstitute.MvvmDialogs;

namespace Demo.CustomOpenFolderDialog
{
    public static class DialogFactoryExtensions
    {
        public static IDialogFactory AddCustomOpenFolder(this IDialogFactory factory) => new CustomDialogFactory(factory);
    }
}
