using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.CustomOpenFolderDialog
{
    public static class DialogFactoryExtensions
    {
        public static IDialogFactory AddCustomOpenFolder(this IDialogFactory factory) => new CustomDialogFactory(factory);
    }
}
