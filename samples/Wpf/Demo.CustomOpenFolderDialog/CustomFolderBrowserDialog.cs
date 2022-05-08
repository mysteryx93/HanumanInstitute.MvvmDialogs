using System;
using System.Threading.Tasks;
using System.Windows;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.Wpf;
using HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs;

namespace Demo.CustomOpenFolderDialog;

public class CustomOpenFolderDialog : FrameworkDialogBase<OpenFolderDialogSettings, string?>
{
    private readonly OpenFolderDialogSettings settings;
    private readonly VistaFolderBrowserDialog folderBrowserDialog;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderBrowserDialogWrapper"/> class.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    public CustomOpenFolderDialog(OpenFolderDialogSettings settings, AppDialogSettings appSettings) :
        base(settings, appSettings)
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
    public override async Task<string?> ShowDialogAsync(WindowWrapper owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var result = await owner.Ref.RunUiAsync(() => folderBrowserDialog.ShowDialog(owner.Ref));

        return result == true ? folderBrowserDialog.SelectedPath : null;
    }

    /// <summary>
    /// Opens a folder browser dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>true if user clicks the OK or YES button; otherwise false.</returns>
    public override string? ShowDialog(WindowWrapper owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var result = folderBrowserDialog.ShowDialog(owner.Ref);

        return result == true ? folderBrowserDialog.SelectedPath : null;
    }
}
