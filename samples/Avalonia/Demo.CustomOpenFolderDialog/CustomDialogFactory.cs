using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.WinForms;

namespace Demo.Avalonia.CustomOpenFolderDialog;

/// <summary>
/// Initializes a new instance of a FrameworkDialog.
/// </summary>
/// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
public class CustomDialogFactory(IDialogFactory? chain = null) : DialogFactoryBase(chain)
{
    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s),
            _ => base.ShowDialogAsync(owner, settings)
        };

    private async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(IView? owner, OpenFolderDialogSettings settings)
    {
        ArgumentNullException.ThrowIfNull(owner);

        var window = TopLevel.GetTopLevel(owner.GetRef());
        var handle = (window?.TryGetPlatformHandle()?.Handle) ?? throw new NullReferenceException("Cannot obtain HWND handle for owner.");
        var dialog = new VistaFolderBrowserDialog
        {
            Description = settings.Title,
            SelectedPath = settings.SuggestedStartLocation?.LocalPath
        };
        var result = await UiExtensions.RunUiAsync(() => dialog.ShowDialog(handle));

        return result == true ? [new DesktopDialogStorageFolder(dialog.SelectedPath!)] : [];
    }
}
