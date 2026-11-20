namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal class MessageBoxApi : IMessageBoxApi
{
    // Creates a message box that the caller can show and close.
    public IMsBox<ButtonResult> CreateMessageBox(MessageBoxApiSettings settings) =>
        MessageBoxManager.GetMessageBoxStandard(
            new()
            {
                ContentTitle = settings.Title,
                ContentMessage = settings.Text,
                ButtonDefinitions = settings.Buttons,
                Icon = settings.Icon,
                WindowStartupLocation = settings.StartupLocation,
                EnterDefaultButton = settings.EnterDefaultButton,
                EscDefaultButton = settings.EscDefaultButton
            });
}
