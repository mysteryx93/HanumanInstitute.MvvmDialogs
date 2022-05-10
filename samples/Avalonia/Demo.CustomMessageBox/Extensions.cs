using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Demo.CustomMessageBox;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.WinForms;

namespace HanumanInstitute.MvvmDialogs;

public static class Extensions
{
    public static Task<TaskDialogButton> ShowTaskMessageBoxAsync(
        this IDialogService service,
        INotifyPropertyChanged ownerViewModel,
        string text,
        string title = "",
        MessageBoxButton button = MessageBoxButton.Ok,
        MessageBoxImage icon = MessageBoxImage.None,
        bool? defaultResult = true,
        AppDialogSettingsBase? appSettings = null)
    {
        var settings = new TaskMessageBoxSettings
        {
            Text = text,
            Title = title,
            Button = button,
            Icon = icon,
            DefaultValue = defaultResult
        };

        return ShowTaskMessageBoxAsync(service, ownerViewModel, settings, appSettings ?? service.AppSettings);
    }

    public static Task<TaskDialogButton> ShowTaskMessageBoxAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel,
        TaskMessageBoxSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

        DialogLogger.Write($"Caption: {settings?.Title}; Message: {settings?.Text}");

        return service.DialogManager.ShowFrameworkDialogAsync<MessageBoxSettings, TaskDialogButton>(
            ownerViewModel, settings ?? new MessageBoxSettings(), appSettings ?? service.AppSettings);
    }
}
