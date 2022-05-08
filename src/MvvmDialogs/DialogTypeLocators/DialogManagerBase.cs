using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs.DialogTypeLocators;

/// <inheritdoc />
public class DialogManagerBase : IDialogManager
{
    /// <summary>
    /// A factory to resolve dialog types.
    /// </summary>
    protected IDialogFactory DialogFactory { get; }
    /// <summary>
    /// A factory to resolve framework dialog types.
    /// </summary>
    protected IFrameworkDialogFactory FrameworkDialogFactory { get; }

    /// <summary>
    /// Initializes a new instance of the DisplayManager class.
    /// </summary>
    /// <param name="dialogFactory">A factory to resolve dialog types.</param>
    /// <param name="frameworkDialogFactory">A factory to resolve framework dialog types.</param>
    public DialogManagerBase(IDialogFactory dialogFactory, IFrameworkDialogFactory frameworkDialogFactory)
    {
        DialogFactory = dialogFactory;
        FrameworkDialogFactory = frameworkDialogFactory;
    }

    /// <inheritdoc />
    public virtual void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel, Type dialogType)
    {
        var dialog = CreateDialog(ownerViewModel, viewModel, dialogType);
        dialog.Show();
    }

    /// <inheritdoc />
    public virtual async Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, Type dialogType)
    {
        var dialog = CreateDialog(ownerViewModel, viewModel, dialogType);
        await dialog.ShowDialogAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Creates a new IWindow from the configured IDialogFactory.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="dialogType">The type of the dialog to show.</param>
    /// <returns>The new IWindow.</returns>
    protected IWindow CreateDialog(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel, Type dialogType)
    {
        var dialog = DialogFactory.Create(dialogType);
        dialog.Owner = ViewRegistration.FindView(ownerViewModel);
        dialog.DataContext = viewModel;
        HandleDialogEvents(viewModel, dialog);
        return dialog;
    }

    /// <summary>
    /// Handles window events. By default, ICloseable and IActivable are handled.
    /// </summary>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="dialog">The dialog being shown.</param>
    protected virtual void HandleDialogEvents(INotifyPropertyChanged viewModel, IWindow dialog)
    {
        if (viewModel is ICloseable c)
        {
            c.RequestClose += (_, _) => dialog.Close();
        }
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (viewModel is IActivable activable)
        {
            activable.RequestActivate += (_, _) => dialog.Activate();
        }
    }

    /// <inheritdoc />
    public virtual Task<TResult> ShowFrameworkDialogAsync<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings)
        where TSettings : DialogSettingsBase
    {
        var dialog = FrameworkDialogFactory.Create<TSettings, TResult>(settings, appSettings);
        return dialog.ShowDialogAsync(ViewRegistration.FindView(ownerViewModel));
    }
}
