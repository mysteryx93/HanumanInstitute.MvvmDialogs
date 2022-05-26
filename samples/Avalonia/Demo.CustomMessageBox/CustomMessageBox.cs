using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.WinForms;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Avalonia.Win32;

namespace Demo.CustomMessageBox;

public class CustomMessageBox : IFrameworkDialog<TaskDialogButton>
{
    private readonly TaskMessageBoxSettings settings;
    private readonly AppDialogSettings appSettings;
    private readonly TaskDialog messageBox;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomMessageBox"/> class.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    public CustomMessageBox(TaskMessageBoxSettings settings, AppDialogSettings appSettings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        messageBox = new TaskDialog
        {
            Content = settings.Text,
            WindowTitle = GetTitle(),
            MainIcon = GetIcon()
        };
        foreach (var item in GetButtons())
        {
            messageBox.Buttons.Add(item);
        }
    }

    private string GetTitle() =>
        string.IsNullOrEmpty(settings.Title) ? " " : settings.Title;

    private IEnumerable<TaskDialogButton> GetButtons() =>
        settings.Button switch
        {
            MessageBoxButton.OkCancel => new [] { NewButton(ButtonType.Ok), NewButton(ButtonType.Cancel)},
            MessageBoxButton.YesNo => new[] { NewButton(ButtonType.Yes), NewButton(ButtonType.No) },
            MessageBoxButton.YesNoCancel => new[] { NewButton(ButtonType.Yes), NewButton(ButtonType.No), NewButton(ButtonType.Cancel) },
            _ => new[] { NewButton(ButtonType.Ok) }
        };

    private TaskDialogButton NewButton(ButtonType type) => new TaskDialogButton(type);

    private TaskDialogIcon GetIcon() =>
        settings.Icon switch
        {
            MessageBoxImage.Error => TaskDialogIcon.Error,
            MessageBoxImage.Information => TaskDialogIcon.Information,
            MessageBoxImage.Warning => TaskDialogIcon.Warning,
            _ => TaskDialogIcon.Custom
        };

    /// <summary>
    /// Opens a message box with specified owner.
    /// </summary>
    /// <param name="owner">
    /// Handle to the window that owns the dialog.
    /// </param>
    /// <returns>
    /// The TaskDialogButton clicked by the user.
    /// </returns>
    public Task<TaskDialogButton> ShowDialogAsync(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = owner.AsWrapper().Ref;
        var platformImpl = (WindowImpl)window.PlatformImpl;
        var handle = platformImpl.Handle.Handle;

        return window.RunUiAsync(() => messageBox.ShowDialog(handle));
    }
}
