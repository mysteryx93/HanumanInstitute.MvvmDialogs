using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Interop;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using Ookii.Dialogs.Wpf;

namespace Demo.Wpf.CustomOpenFolderDialog;

/// <summary>
/// Initializes a new instance of a FrameworkDialog.
/// </summary>
/// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
public class CustomDialogFactory(IDialogFactory? chain = null) : DialogFactoryBase(chain)
{
    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await UiExtensions.RunUiAsync(() => ShowOpenFolderDialog(owner, s)).ConfigureAwait(true),
            _ => base.ShowDialogAsync(owner, settings)
        };

    /// <inheritdoc />
    public override object? ShowDialog<TSettings>(ViewWrapper? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => ShowOpenFolderDialog(owner, s),
            _ => base.ShowDialog(owner, settings)
        };

    private IDialogStorageFolder[] ShowOpenFolderDialog(ViewWrapper? owner, OpenFolderDialogSettings settings)
    {
        ArgumentNullException.ThrowIfNull(owner);

        var window = owner.AsWrapper().Ref;
        var handle = new WindowInteropHelper(window).Handle;

        var dialog = new VistaFolderBrowserDialog
        {
            Description = settings.Title,
            SelectedPath = settings.SuggestedStartLocation?.LocalPath
        };
        var result = dialog.ShowDialog(handle);

        return result == true ? [new DesktopDialogStorageFolder(dialog.SelectedPath!)] : [];
    }
}
