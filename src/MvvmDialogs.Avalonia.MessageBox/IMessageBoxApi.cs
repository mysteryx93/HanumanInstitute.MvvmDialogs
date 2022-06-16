
namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal interface IMessageBoxApi
{
    Task<ButtonResult> ShowMessageBox(Window owner, MessageBoxApiSettings settings);
}
