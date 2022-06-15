using System;
using System.Threading.Tasks;
using Avalonia.Win32;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Ookii.Dialogs.WinForms;

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
    public override async Task<object?> ShowDialogAsync<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s, appSettings),
            _ => base.ShowDialogAsync(owner, settings, appSettings)
        };

    private async Task<string?> ShowOpenFolderDialogAsync(WindowWrapper owner, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = owner.AsWrapper().Ref;
        var platformImpl = (WindowImpl)window.PlatformImpl;
        var handle = platformImpl.Handle.Handle;

        var dialog = new VistaFolderBrowserDialog
        {
            Description = settings.Title,
            SelectedPath = settings.InitialDirectory
        };
        var result = await window.RunUiAsync(() => dialog.ShowDialog(handle));

        return result == true ? dialog.SelectedPath : null;
    }
}
