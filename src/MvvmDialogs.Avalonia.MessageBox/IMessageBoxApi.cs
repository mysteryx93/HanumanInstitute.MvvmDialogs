namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal interface IMessageBoxApi
{
    // Creates a message box that the caller can show and close.
    IMsBox<ButtonResult> CreateMessageBox(MessageBoxApiSettings settings);
}
