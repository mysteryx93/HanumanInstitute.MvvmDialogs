
namespace HanumanInstitute.MvvmDialogs.Avalonia.AuraUI;

internal interface IMessageBoxApi
{
    Task<bool?> ShowMessageBoxAsync(Control? owner, MessageBoxApiSettings settings);
    Task ShowMessageAsync(Control? owner, MessageBoxApiSettings settings);
}
