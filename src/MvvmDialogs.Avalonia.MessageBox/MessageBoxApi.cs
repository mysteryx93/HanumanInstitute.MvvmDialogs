using MsBox.Avalonia.Dto;

namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal class MessageBoxApi : IMessageBoxApi
{
    public Task<ButtonResult> ShowMessageBoxAsync(Window? owner, MessageBoxApiSettings settings, MessageBoxMode mode)
    {
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            new MessageBoxStandardParams()
            {
                ContentTitle = settings.Title,
                ContentMessage = settings.Text,
                ButtonDefinitions = settings.Buttons,
                Icon = settings.Icon,
                WindowStartupLocation = settings.StartupLocation,
                EnterDefaultButton = settings.EnterDefaultButton,
                EscDefaultButton = settings.EscDefaultButton
            });
        return mode == MessageBoxMode.Popup ? msgBox.ShowAsPopupAsync(owner) : msgBox.ShowWindowDialogAsync(owner);
    }
}
