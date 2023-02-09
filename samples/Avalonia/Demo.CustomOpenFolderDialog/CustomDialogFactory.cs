using System;
using System.Threading.Tasks;
using Avalonia.Win32;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Ookii.Dialogs.WinForms;
using Avalonia.Controls;

namespace Demo.CustomOpenFolderDialog;

public class CustomDialogFactory : DialogFactoryBase
{
    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public CustomDialogFactory(IDialogFactory? chain = null)
        : base(chain)
    {
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s, appSettings),
            _ => base.ShowDialogAsync(owner, settings, appSettings)
        };

    private async Task<string?> ShowOpenFolderDialogAsync(IView? owner, OpenFolderDialogSettings settings, AppDialogSettingsBase appSettings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = TopLevel.GetTopLevel(owner.GetRef());
        var platformImpl = window?.PlatformImpl as WindowImpl;
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

        return result == true ? dialog.SelectedPath : null;
    }
}
