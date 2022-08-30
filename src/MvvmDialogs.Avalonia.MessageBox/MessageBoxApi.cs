using MessageBox.Avalonia.DTO;

namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal class MessageBoxApi : IMessageBoxApi
{
    public Task<ButtonResult> ShowMessageBox(Window? owner, MessageBoxApiSettings settings) =>
        MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams() {
            ContentTitle = settings.Title,
            ContentMessage = settings.Text,
            ButtonDefinitions = settings.Buttons,
            Icon = settings.Icon,
            WindowStartupLocation = settings.StartupLocation,
            EnterDefaultButton = settings.EnterDefaultButton,
            EscDefaultButton = settings.EscDefaultButton
        }).ShowDialog(owner);
}
