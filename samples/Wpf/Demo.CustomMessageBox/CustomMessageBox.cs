using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.Wpf;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.CustomMessageBox;

public class CustomMessageBox : FrameworkDialogBase<TaskMessageBoxSettings, TaskDialogButton>, IFrameworkDialogSync<TaskDialogButton>
{
    private readonly TaskMessageBoxSettings settings;
    private readonly TaskDialog messageBox;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomMessageBox"/> class.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    public CustomMessageBox(TaskMessageBoxSettings settings, AppDialogSettings appSettings) : base(settings, appSettings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

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
    /// A bool? value that specifies which message box button is
    /// clicked by the user.
    /// </returns>
    public override async Task<TaskDialogButton> ShowDialogAsync(WindowWrapper owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var result = await owner.Ref.RunUiAsync(() => messageBox.ShowDialog(owner.Ref));
        return result;
    }

    /// <summary>
    /// Opens a message box with specified owner.
    /// </summary>
    /// <param name="owner">
    /// Handle to the window that owns the dialog.
    /// </param>
    /// <returns>
    /// A bool? value that specifies which message box button is
    /// clicked by the user.
    /// </returns>
    public override TaskDialogButton ShowDialog(WindowWrapper owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var result = messageBox.ShowDialog(owner.Ref);
        return result;
    }
}
