using System;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;
using Ookii.Dialogs.Wpf;

namespace Demo.CustomOpenFolderDialog;

public class CustomOpenFolderDialog : IFrameworkDialog<string?>, IFrameworkDialogSync<string?>
{
    private readonly OpenFolderDialogSettings settings;
    private readonly VistaFolderBrowserDialog folderBrowserDialog;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderBrowserDialogWrapper"/> class.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    public CustomOpenFolderDialog(OpenFolderDialogSettings settings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        folderBrowserDialog = new VistaFolderBrowserDialog
        {
            Description = settings.Title,
            SelectedPath = settings.InitialDirectory
        };
    }

    /// <summary>
    /// Opens a folder browser dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>true if user clicks the OK or YES button; otherwise false.</returns>
    public async Task<string?> ShowDialogAsync(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = owner.AsWrapper().Ref;
        var result = await window.RunUiAsync(() => folderBrowserDialog.ShowDialog(window));

        return result == true ? folderBrowserDialog.SelectedPath : null;
    }

    /// <summary>
    /// Opens a folder browser dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>true if user clicks the OK or YES button; otherwise false.</returns>
    public string? ShowDialog(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = owner.AsWrapper().Ref;
        var result = folderBrowserDialog.ShowDialog(window);

        return result == true ? folderBrowserDialog.SelectedPath : null;
    }
}
