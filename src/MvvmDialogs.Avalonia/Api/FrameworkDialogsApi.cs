namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
    public async Task<string[]?> ShowOpenFileDialogAsync(Window? owner, OpenFileApiSettings settings)
    {
        var dialog = new OpenFileDialog();
        settings.ApplyTo(dialog);
        return await dialog.ShowAsync(owner!).ConfigureAwait(true);
    }

    public async Task<string?> ShowSaveFileDialogAsync(Window? owner, SaveFileApiSettings settings)
    {
        var dialog = new SaveFileDialog();
        settings.ApplyTo(dialog);
        return await dialog.ShowAsync(owner!).ConfigureAwait(true);
    }

    public async Task<string?> ShowOpenFolderDialogAsync(Window? owner, OpenFolderApiSettings settings)
    {
        var dialog = new OpenFolderDialog();
        settings.ApplyTo(dialog);
        return await dialog.ShowAsync(owner!).ConfigureAwait(true);
    }
}
