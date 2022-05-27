﻿using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface responsible for UI interactions.
/// </summary>
public interface IDialogManager
{
    /// <summary>
    /// Shows a new window of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="view">The view to show.</param>
    void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel, object? view);

    /// <summary>
    /// Shows a new dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <param name="view">The view to show.</param>
    /// <returns>The dialog result.</returns>
    Task ShowDialogAsync(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, object? view);

    /// <summary>
    /// Shows a framework dialog whose type depends on the settings type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings to pass to the <see cref="IFrameworkDialog{TResult}"/></param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <typeparam name="TSettings">The settings type used to determine the implementation of <see cref="IFrameworkDialog{TResult}"/> to create.</typeparam>
    /// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
    /// <returns>A framework dialog implementing <see cref="IFrameworkDialog{TResult}"/>.</returns>
    Task<TResult> ShowFrameworkDialogAsync<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings)
        where TSettings : DialogSettingsBase;

    /// <summary>
    /// Returns the window with a DataContext equal to specified ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel to search for.</param>
    /// <returns>A Window, or null.</returns>
    IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel);
}