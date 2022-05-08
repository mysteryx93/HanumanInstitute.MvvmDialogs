using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace HanumanInstitute.MvvmDialogs.Avalonia.FrameworkDialogs.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
    public Task<ButtonResult> ShowMessageBox(Window owner, MessageBoxApiSettings settings) =>
        MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams() {
            ContentTitle = settings.Title,
            ContentMessage = settings.Text,
            ButtonDefinitions = settings.Buttons,
            Icon = settings.Icon,
            WindowStartupLocation = settings.StartupLocation,
            EnterDefaultButton = settings.EnterDefaultButton,
            EscDefaultButton = settings.EscDefaultButton
        }).ShowDialog(owner);

    public Task<string[]?> ShowOpenFileDialog(Window owner, OpenFileApiSettings settings)
    {
        var dialog = new global::Avalonia.Controls.OpenFileDialog();
        settings.ApplyTo(dialog);
        return dialog.ShowAsync(owner);
    }

    public Task<string?> ShowSaveFileDialog(Window owner, SaveFileApiSettings settings)
    {
        var dialog = new global::Avalonia.Controls.SaveFileDialog();
        settings.ApplyTo(dialog);
        return dialog.ShowAsync(owner);
    }

    public Task<string?> ShowOpenFolderDialog(Window owner, OpenFolderApiSettings settings)
    {
        var dialog = new global::Avalonia.Controls.OpenFolderDialog();
        settings.ApplyTo(dialog);
        return dialog.ShowAsync(owner);
    }
}
