﻿using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// DialogManager for Avalonia.
/// </summary>
public class DialogManager : DialogManagerBase<Window>
{
    /// <inheritdoc />
    public DialogManager(IViewLocator? viewLocator = null,
        IFrameworkDialogFactory? frameworkDialogFactory = null,
        ILogger<DialogManager>? logger = null) :
        base(viewLocator ?? new ViewLocatorBase(),
            frameworkDialogFactory ?? new FrameworkDialogFactory(), logger)
    {
    }

    /// <inheritdoc />
    protected override IWindow CreateWrapper(Window window) => window.AsWrapper();

    private static IEnumerable<Window> Windows =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows ?? Array.Empty<Window>();

    /// <inheritdoc />
    public override IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel) =>
        Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();
}
