using DynamicData;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

internal class FluentApi : IFluentApi
{
    public async Task<ContentDialogResult> ShowContentDialog(ContentControl? owner, ContentDialogSettings settings)
    {
        var dialog = new ContentDialog()
        {
            Title = settings.Title,
            Content = settings.Content,
            CloseButtonText = settings.CloseButtonText,
            PrimaryButtonText = settings.PrimaryButtonText,
            SecondaryButtonText = settings.SecondaryButtonText,
            DefaultButton = settings.DefaultButton,
            IsPrimaryButtonEnabled = settings.IsPrimaryButtonEnabled,
            IsSecondaryButtonEnabled = settings.IsSecondaryButtonEnabled,
            FullSizeDesired = settings.FullSizeDesired
        };

        // Allow the dialog to be closed by mobile back navigation.
        void Cancel() => dialog.Hide();
        CancellableActions.Add(Cancel);
        var result = await dialog.ShowAsync().ConfigureAwait(true);
        CancellableActions.Remove(Cancel);
        return result;
    }

    public async Task<object> ShowTaskDialog(ContentControl? owner, TaskDialogSettings settings)
    {
        var dialog = new TaskDialog()
        {
            Title = settings.Title,
            Header = settings.Header,
            SubHeader = settings.SubHeader,
            Content = settings.Content,
            IconSource = settings.IconSource,
            FooterVisibility = settings.FooterVisibility,
            IsFooterExpanded = settings.IsFooterExpanded,
            Footer = settings.Footer,
            ShowProgressBar = settings.ShowProgressBar
        };
        dialog.Buttons.AddRange(settings.Buttons);
        if (owner != null)
        {
            dialog.XamlRoot = TopLevel.GetTopLevel(owner);
        }

        // Allow the dialog to be closed by mobile back navigation.
        void Cancel() => dialog.Hide();
        CancellableActions.Add(Cancel);
        var result = await dialog.ShowAsync().ConfigureAwait(true);
        CancellableActions.Remove(Cancel);
        return result;
    }
}
