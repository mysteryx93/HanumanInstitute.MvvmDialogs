using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HanumanInstitute.MvvmDialogs;

namespace Demo.ModalCustomDialog;

public class AddTextCustomDialog : IWindow
{
    private readonly AddTextDialog dialog;

    public AddTextCustomDialog()
    {
        dialog = new AddTextDialog();
    }

    object IWindow.DataContext
    {
        get => dialog.DataContext;
        set => dialog.DataContext = value;
    }

    bool? IWindow.DialogResult
    {
        get => dialog.DialogResult;
        set => dialog.DialogResult = value;
    }

    ContentControl IWindow.Owner
    {
        get => dialog.Owner;
        set => dialog.Owner = (Window)value;
    }

    Task<bool?> IWindow.ShowDialogAsync()
    {
        return dialog.ShowDialog();
    }

    void IWindow.Show()
    {
        dialog.Show();
    }
}
