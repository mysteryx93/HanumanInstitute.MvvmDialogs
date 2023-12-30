using System.Linq;
using System.Windows.Forms;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Wpf.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
    public FrameworkDialogsApi()
    {
    }

    public MessageBoxResult ShowMessageBox(Window? owner, MessageBoxApiSettings settings) =>
        owner != null ?
            System.Windows.MessageBox.Show(
                owner,
                settings.MessageBoxText,
                settings.Caption,
                settings.Buttons,
                settings.Icon,
                settings.DefaultButton) :
            System.Windows.MessageBox.Show(
                settings.MessageBoxText,
                settings.Caption,
                settings.Buttons,
                settings.Icon,
                settings.DefaultButton);

    public IReadOnlyList<IDialogStorageFile> ShowOpenFileDialog(Window? owner, OpenFileApiSettings settings)
    {
        var dialog = new System.Windows.Forms.OpenFileDialog();
        settings.ApplyTo(dialog);
        var result = dialog.ShowDialog(owner);
        return result == DialogResult.OK ?
            dialog.FileNames.Select(x => new DesktopDialogStorageFile(x)).ToArray() :
            Array.Empty<IDialogStorageFile>();
    }

    public IDialogStorageFile? ShowSaveFileDialog(Window? owner, SaveFileApiSettings settings)
    {
        var dialog = new System.Windows.Forms.SaveFileDialog();
        settings.ApplyTo(dialog);
        var result = dialog.ShowDialog(owner);
        return result == DialogResult.OK ?
            new DesktopDialogStorageFile(dialog.FileName) :
            null;
    }

    public IReadOnlyList<IDialogStorageFolder> ShowOpenFolderDialog(Window? owner, OpenFolderApiSettings settings)
    {
        var dialog = new FolderBrowserDialog();
        settings.ApplyTo(dialog);
        var result = dialog.ShowDialog(owner);
        return result == DialogResult.OK ?
            new IDialogStorageFolder[] { new DesktopDialogStorageFolder(dialog.SelectedPath) } :
            Array.Empty<IDialogStorageFolder>();
    }
}
