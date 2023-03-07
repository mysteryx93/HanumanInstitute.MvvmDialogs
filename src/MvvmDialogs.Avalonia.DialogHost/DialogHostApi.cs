using Avalonia.VisualTree;

namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

internal class DialogHostApi : IDialogHostApi
{
    private static readonly object s_lockCreateHost = new();

    public async Task<object?> ShowDialogHostAsync(ContentControl? owner, DialogHostSettings settings)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        DialogHostAvalonia.DialogHost? host;
        lock (s_lockCreateHost) // lock to avoid creating host twice
        {
            host = owner.FindDescendantOfType<DialogHostAvalonia.DialogHost>();
            if (host == null)
            {
                host = new DialogHostAvalonia.DialogHost();
                var temp = owner.Content;
                owner.Content = null;
                host.Content = temp;
                owner.Content = host;
            }
            host.CloseOnClickAway = settings.CloseOnClickAway;
            host.CloseOnClickAwayParameter = settings.CloseOnClickAwayParameter;
            host.PopupPositioner = settings.PopupPositioner;
            host.OverlayBackground = settings.OverlayBackground ?? host.OverlayBackground;
            host.DialogMargin = settings.DialogMargin ?? host.DialogMargin;
            host.DisableOpeningAnimation = settings.DisableOpeningAnimation;
        }

        var closingHandler = settings.ClosingHandler ?? ((_, _) => { });
        return await DialogHostAvalonia.DialogHost.Show(settings.ContentViewModel!, host, closingHandler);
    }
}
