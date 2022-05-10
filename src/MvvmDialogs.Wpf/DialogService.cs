using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;
using Application = System.Windows.Application;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in WPF.
/// </summary>
public class DialogService : DialogServiceBase, IDialogServiceSync
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogServiceBase"/> class.
    /// </summary>
    /// <remarks>
    /// By default <see cref="ReflectionDialogFactory"/> is used as dialog factory,
    /// <see cref="NamingConventionDialogTypeLocator"/> is used as dialog type locator
    /// and <see cref="FrameworkDialogFactory"/> is used as framework dialog factory.
    /// </remarks>
    public DialogService()
        : this(settings: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="dialogManager">Class responsible for UI interactions.</param>
    /// <param name="dialogTypeLocator">Locator responsible for finding a dialog type matching a view model. Default value is
    /// an instance of <see cref="NamingConventionDialogTypeLocator"/>.</param>
    /// <param name="settings">Set application-wide settings.</param>
    public DialogService(
        IDialogManager? dialogManager = null,
        IDialogTypeLocator? dialogTypeLocator = null,
        AppDialogSettings? settings = null)
        : base(settings ?? new AppDialogSettings(),
            dialogManager ?? new DialogManager(new FrameworkDialogFactory(), new ReflectionDialogFactory()),
            dialogTypeLocator ?? new NamingConventionDialogTypeLocator())
    {
    }

    private static IEnumerable<Window> Windows =>
        Application.Current.Windows.Cast<Window>();

    /// <inheritdoc />
    protected override IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel) =>
        Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();

    protected Window? FindOwnerWindow(INotifyPropertyChanged ownerViewModel) =>
        (ViewRegistration.FindView(ownerViewModel) as WindowWrapper)?.Ref;

    /// <inheritdoc />
    public bool? ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternal(ownerViewModel, viewModel, DialogTypeLocator.Locate(viewModel));

    /// <inheritdoc />
    public bool? ShowDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        ShowDialogInternal(ownerViewModel, viewModel, typeof(T));

    /// <summary>
    /// Displays a modal dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="dialogType">The type of the dialog to show.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    protected bool? ShowDialogInternal(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, Type dialogType)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        DialogLogger.Write($"Dialog: {dialogType}; View model: {viewModel.GetType()}; Owner: {ownerViewModel.GetType()}");
        DialogManager.AsSync().ShowDialog(ownerViewModel, viewModel, dialogType);
        DialogLogger.Write($"Dialog: {dialogType}; Result: {viewModel.DialogResult}");
        return viewModel.DialogResult;
    }
}
