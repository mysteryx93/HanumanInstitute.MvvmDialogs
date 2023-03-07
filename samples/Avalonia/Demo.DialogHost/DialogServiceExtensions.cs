using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Demo.Avalonia.DialogHost;
using DialogHostAvalonia;
using HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

// ReSharper disable once CheckNamespace

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Provides IDialogService extensions for fluent dialogs.
/// </summary>
public static class DialogServiceExtensions
{
    public static async Task ShowCurrentTimeAsync(
        this IDialogService service,
        INotifyPropertyChanged ownerViewModel,
        INotifyPropertyChanged contentViewModel,
        DialogClosingEventHandler? closingHandler = null,
        AppDialogSettingsBase? appSettings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

        var settings = new DialogHostSettings()
        {
            ContentViewModel = contentViewModel,
            ClosingHandler = closingHandler,
            CloseOnClickAway = true
        };
        await service.ShowDialogHostAsync(ownerViewModel, settings, appSettings).ConfigureAwait(true);
    }
    
    public static async Task<string?> AskTextAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel, AppDialogSettingsBase? appSettings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

        var vm = service.CreateViewModel<TextBoxViewModel>();
        var settings = new DialogHostSettings();
        settings.ContentViewModel = vm;
        return (string?)await service.ShowDialogHostAsync(ownerViewModel, settings, appSettings).ConfigureAwait(true);
    }
}
