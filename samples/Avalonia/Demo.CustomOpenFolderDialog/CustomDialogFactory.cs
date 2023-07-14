﻿using System;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Ookii.Dialogs.WinForms;
using Avalonia.Controls;
using System.Collections.Generic;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.PathInfo;
using Avalonia.Platform;

namespace Demo.Avalonia.CustomOpenFolderDialog;

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
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s),
            _ => base.ShowDialogAsync(owner, settings)
        };

    private async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(IView? owner, OpenFolderDialogSettings settings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = TopLevel.GetTopLevel(owner.GetRef());
        var platformImpl = window?.PlatformImpl as IWindowImpl;
        if (platformImpl == null)
        {
            throw new NullReferenceException("Cannot obtain HWND handle for owner.");
        }

        var handle = platformImpl.Handle.Handle;

        var dialog = new VistaFolderBrowserDialog
        {
            Description = settings.Title,
            SelectedPath = settings.InitialDirectory
        };
        var result = await UiExtensions.RunUiAsync(() => dialog.ShowDialog(handle));

        return result == true ? new IDialogStorageFolder[] { new DialogStorageFolder(_infoFactory.GetDirectoryInfo(dialog.SelectedPath!)) } :
            Array.Empty<IDialogStorageFolder>();
    }
}
