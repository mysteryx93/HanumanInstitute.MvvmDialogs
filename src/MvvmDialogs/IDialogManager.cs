using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface responsible for UI interactions.
/// </summary>
public interface IDialogManager
{
    /// <summary>
    /// Gets the ILogger that captures MvvmDialogs logs.
    /// </summary>
    ILogger<IDialogManager>? Logger { get; }

    /// <summary>
    /// Shows a new window of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    void Show(INotifyPropertyChanged? ownerViewModel, INotifyPropertyChanged viewModel);

    /// <summary>
    /// Shows a new dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <returns>The dialog result.</returns>
    Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel);

    /// <summary>
    /// Shows a framework dialog whose type depends on the settings type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings to pass to the <see cref="IDialogFactory"/></param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <param name="resultToString">A function to convert the result into a string for logging. If null, ToString will be used.</param>
    /// <typeparam name="TSettings">The settings type used to determine which dialog to show.</typeparam>
    /// <returns>The dialog result.</returns>
    Task<object?> ShowFrameworkDialogAsync<TSettings>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings, Func<object?, string>? resultToString = null)
        where TSettings : DialogSettingsBase;

    /// <summary>
    /// Returns the window with a DataContext equal to specified ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel to search for.</param>
    /// <returns>A Window, or null.</returns>
    IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel);
}
