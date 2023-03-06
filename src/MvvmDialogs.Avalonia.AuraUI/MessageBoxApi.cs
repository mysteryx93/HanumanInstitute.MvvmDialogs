
using Aura.UI.Controls;
using Aura.UI.Services;

namespace HanumanInstitute.MvvmDialogs.Avalonia.AuraUI;

internal class MessageBoxApi : IMessageBoxApi
{
    public Task<bool?> ShowMessageBoxAsync(Control? owner, MessageBoxApiSettings settings)
    {
        TaskCompletionSource<bool?> completion = new();
        var dialog = new ContentDialog
        {
            Content = settings.Text,
            OkButtonContent = settings.OkButtonContent,
            CancelButtonContent = settings.CancelButtonContent
        };
        dialog.OkButtonClick += (_, _) =>
        {
            completion.SetResult(true);
            dialog.Close();
        };
        dialog.CancelButtonClick += (_, _) =>
        {
            completion.SetResult(false);
            dialog.Close();
        };
        dialog.SetOwner(owner);
        dialog.Show();
        return completion.Task;
    }

    public Task ShowMessageAsync(Control? owner, MessageBoxApiSettings settings)
    {
        TaskCompletionSource completion = new();
        owner.NewMessageDialog(settings.Title, settings.Text, (_, _) => completion.SetResult());
        return completion.Task;
    }
}
