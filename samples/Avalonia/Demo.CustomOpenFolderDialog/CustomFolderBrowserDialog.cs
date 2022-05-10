using System;
using System.Threading.Tasks;
using Avalonia.Win32;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.WinForms;

namespace Demo.CustomOpenFolderDialog;

public class CustomOpenFolderDialog : IFrameworkDialog<string?>
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
        var platformImpl = (WindowImpl)window.PlatformImpl;
        var handle = platformImpl.Handle.Handle;

        var result = await window.RunUiAsync(() => folderBrowserDialog.ShowDialog(handle));

        return result == true ? folderBrowserDialog.SelectedPath : null;
    }
}
