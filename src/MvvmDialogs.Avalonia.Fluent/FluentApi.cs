
using Avalonia.VisualTree;
using DynamicData;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

internal class FluentApi : IFluentApi
{
    public Task<ContentDialogResult> ShowContentDialog(Window? owner, ContentDialogSettings settings)
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
        return dialog.ShowAsync();
    }

    public Task<object> ShowTaskDialog(Window? owner, TaskDialogSettings settings)
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
            dialog.XamlRoot = owner.GetVisualRoot();
        }
        return dialog.ShowAsync();
    }
}
