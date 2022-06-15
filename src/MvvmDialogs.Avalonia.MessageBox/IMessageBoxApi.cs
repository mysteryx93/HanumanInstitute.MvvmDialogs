using Avalonia.Controls;
using MessageBox.Avalonia.Enums;

namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal interface IMessageBoxApi
{
    Task<ButtonResult> ShowMessageBox(Window owner, MessageBoxApiSettings settings);
}
