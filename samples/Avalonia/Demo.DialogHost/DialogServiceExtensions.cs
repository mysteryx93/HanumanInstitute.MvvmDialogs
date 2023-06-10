using System.ComponentModel;
using System.Threading.Tasks;
using Demo.Avalonia.DialogHost;
using HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

// ReSharper disable once CheckNamespace

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Provides IDialogService extensions for fluent dialogs.
/// </summary>
public static class DialogServiceExtensions
{
    public static async Task<string?> AskTextAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel, AppDialogSettingsBase? appSettings = null)
    {
        var vm = service.CreateViewModel<AskTextBoxViewModel>();
        var settings = new DialogHostSettings(vm);
        await service.ShowDialogHostAsync(ownerViewModel, settings, appSettings).ConfigureAwait(true);
        return vm.DialogResult == true ? vm.Text : null;
    }
}
