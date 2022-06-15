
namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
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
