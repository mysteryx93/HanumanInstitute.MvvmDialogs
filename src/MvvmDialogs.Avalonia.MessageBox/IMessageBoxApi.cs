
namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal interface IMessageBoxApi
{
    Task<ButtonResult> ShowMessageBoxAsync(Window? owner, MessageBoxApiSettings settings);
}
