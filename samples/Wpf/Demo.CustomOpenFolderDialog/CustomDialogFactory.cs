using System;
using System.Threading.Tasks;
using System.Windows.Interop;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using Ookii.Dialogs.Wpf;

namespace Demo.Wpf.CustomOpenFolderDialog;

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
    public override async Task<object?> ShowDialogAsync<TSettings>(ViewWrapper? owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await UiExtensions.RunUiAsync(() => ShowOpenFolderDialog(owner, s, appSettings)).ConfigureAwait(true),
            _ => base.ShowDialogAsync(owner, settings, appSettings)
        };

    /// <inheritdoc />
    public override object? ShowDialog<TSettings>(ViewWrapper? owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => ShowOpenFolderDialog(owner, s, appSettings),
            _ => base.ShowDialog(owner, settings, appSettings)
        };

    private string? ShowOpenFolderDialog(ViewWrapper? owner, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
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

        return result == true ? dialog.SelectedPath : null;
    }
}
