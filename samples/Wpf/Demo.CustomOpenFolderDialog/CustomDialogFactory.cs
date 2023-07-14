using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Interop;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.PathInfo;
using HanumanInstitute.MvvmDialogs.Wpf;
using Ookii.Dialogs.Wpf;

namespace Demo.Wpf.CustomOpenFolderDialog;

public class CustomDialogFactory : DialogFactoryBase
{
    private readonly IPathInfoFactory _infoFactory;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public CustomDialogFactory(IPathInfoFactory infoFactory, IDialogFactory? chain = null)
        : base(chain)
    {
        _infoFactory = infoFactory;
    }

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

    private IReadOnlyList<IDialogStorageFolder> ShowOpenFolderDialog(ViewWrapper? owner, OpenFolderDialogSettings settings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = owner.AsWrapper().Ref;
        var handle = new WindowInteropHelper(window).Handle;

        var dialog = new VistaFolderBrowserDialog
        {
            Description = settings.Title,
            SelectedPath = settings.InitialDirectory
        };
        var result = dialog.ShowDialog(handle);

        return result == true ? new IDialogStorageFolder[] { new DialogStorageFolder(_infoFactory.GetDirectoryInfo(dialog.SelectedPath!)) } :
            Array.Empty<IDialogStorageFolder>();
    }
}
