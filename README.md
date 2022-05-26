<h1 align="center">
    <br>
    <img src="https://raw.githubusercontent.com/FantasticFiasco/mvvm-dialogs/master/doc/resources/Icon_200x200.png" alt="MVVM Dialogs" width="200">
    <br>
    MVVM Dialogs
    <br>
</h1>

<h4 align="center">Library simplifying the concept of opening dialogs from a view model when using MVVM</h4>
<h6 align="center">Cross-platform solution derived from <a href="https://github.com/FantasticFiasco/mvvm-dialogs">FantasticFiasco/mvvm-dialogs</a></h6>

<p>

</p>

UI Frameworks currently supported:
- WPF <a href="https://www.nuget.org/packages/HanumanInstitute.MvvmDialogs.Wpf/"><img src="https://img.shields.io/nuget/v/HanumanInstitute.MvvmDialogs.Wpf.svg"></a> (Windows Presentation Foundation)
- [Avalonia](https://avaloniaui.net/) <a href="https://www.nuget.org/packages/HanumanInstitute.MvvmDialogs.Avalonia/"><img src="https://img.shields.io/nuget/v/HanumanInstitute.MvvmDialogs.Avalonia.svg"></a> (mature cross-platform UI framework with WPF-like syntax)

UI Frameworks that can easily be added through community efforts:
- WinUI 3 (promising Android/iOS support but won't support Linux)
- Blazor (full app in web browser)
- UWP (Universal Windows Platform, this thing is dead)

## Table of contents

- [Introduction](#introduction)
- [Generic Usage](#generic-usage)
- [WPF Usage](#wpf-usage)
- [Avalonia Usage](#avalonia-usage)
- [IModalDialogViewModel / ICloseable / IActivable](#imodaldialogviewmodel--icloseable--iactivable)
- [Custom Windows](#custom-windows)
- [Custom Framework Dialogs](#custom-framework-dialogs)
- [Unit Testing](#unit-testing)
- [Differences from FantasticFiasco/mvvm-dialogs](#differences-from-fantasticfiascomvvm-dialogs)
- [Contributions Are Welcomed](#contributions-are-welcomed)
---

## Introduction

MVVM Dialogs is a library simplifying the concept of opening dialogs from a view model when using MVVM. It enables the developer to easily write unit tests for view models in the same manner unit tests are written for other classes.

The library has built in support for the following dialogs:

- Modal window
- Non-modal window
- Message box
- Open file dialog
- Save file dialog
- Open folder dialog

## Generic Usage

The interface `IDialogService` provides a platform-agnostic way of managing dialogs:

```c#
using HanumanInstitute.MvvmDialogs

public class ModalDialogTabContentViewModel : INotifyPropertyChanged
{
    private readonly IDialogService dialogService;

    public ModalDialogTabContentViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    ...

    private async Task ShowDialogAsync()
    {
        var dialogViewModel = new AddTextDialogViewModel();

        bool? success = await dialogService.ShowDialogAsync(this, dialogViewModel);
        if (success == true)
        {
            Texts.Add(dialogViewModel.Text);
        }
    }
}
```

The recommended way of accessing your dialogs is to create a `DialogExtensions` class containing strongly-typed access to all your dialogs.

```c#
public static class DialogExtensions
{
    public static async Task<PresetItem?> ShowLoadPresetViewAsync(this IDialogService dialog, INotifyPropertyChanged ownerViewModel)
    {
        dialog.CheckNotNull(nameof(dialog)); // using HanumanInstitute.Validators

        var viewModel = ViewModelLocator.SelectPreset.Load(false);
        var result = await dialog.ShowDialogAsync(ownerViewModel, viewModel).ConfigureAwait(true);
        return result == true ? viewModel.SelectedItem : null;
    }

    public static async Task<string?> ShowSavePresetViewAsync(this IDialogService dialog, INotifyPropertyChanged ownerViewModel)
    {
        dialog.CheckNotNull(nameof(dialog));

        var viewModel = ViewModelLocator.SelectPreset.Load(true);
        var result = await dialog.ShowDialogAsync(ownerViewModel, viewModel).ConfigureAwait(true);
        return result == true ? viewModel.PresetName : null;
    }
}
```

Then the usage is super *sexy*! (a long way from ReactiveUI [Interactions](https://www.reactiveui.net/docs/handbook/interactions/)...)

```c#
private async Task<string?> SavePreset()
{
    var presetName = await dialogService.ShowSavePresetViewAsync(this);
    if (presetName != null) { ... }
    return presetName;
}
```

Meanwhile...
- You can use IDialogService within class libraries that have no reference to Avalonia nor Wpf (referencing only `HanumanInstitute.MvvmDialogs`)
- It is friendly for unit tests

## WPF Usage

Add a reference to `HanumanInstitute.MvvmDialogs.Wpf`

You must decorate the views with the attached property `DialogServiceViews.IsRegistered`:

```xaml
<UserControl
    x:Class="DemoApplication.Features.Dialog.Modal.Views.ModalDialogTabContent"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:md="https://github.com/mysteryx93/HanumanInstitute.MvvmDialogs"
    md:DialogServiceViews.IsRegistered="True">

  ...

</UserControl>
```

DialogService must be registered in the DependencyInjection container of your choice. Note that IDialogService is defined in `HanumanInstitute.MvvmDialogs` and DialogService is defined in `HanumanInstitute.MvvmDialogs.Wpf`.

```c#
public partial class App
{
    // Registering in CommunityToolkit.Mvvm
    protected override void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService, DialogService>()
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}
```

`IDialogService` exposes platform-agnostic async methods. For WPF (only), sync methods
are also available. If you plan to ever use the ViewModel with a different UI framework,
it is recommended to use the async methods.

```c#
    private bool? ShowDialog()
    {
        var dialogViewModel = new AddTextDialogViewModel();
        return dialogService.ShowDialog(this, dialogViewModel); // Sync
    }
}
```

### AppDialogSettings
When creating DialogService, you can pass AppDialogSettings with application-wide settings.


#### bool MessageBoxRightToLeft

Gets or sets whether message boxes are displayed right-to-left (RightAlign+RtlReading).

#### bool MessageBoxDefaultDesktopOnly

Gets or sets whether to display on the default desktop of the interactive window station. Specifies that the message box is displayed from a .NET Windows Service application in order to notify the user of an event.

#### bool MessageBoxServiceNotification

Gets or sets whether to display on the currently active desktop even if a user is not logged on to the computer. Specifies that the message box is displayed from a .NET Windows Service application in order to notify the user of an event.

### RunUiAsync

There is a useful extension method in `HanumanInstitute.MvvmDialogs.Wpf` to run a synchronous UI method as an async method:

```c#
window.RunUiAsync(func)
```

## Avalonia Usage

Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia`

DialogService must be registered in the DependencyInjection container of your choice. Note that IDialogService is defined in `HanumanInstitute.MvvmDialogs` and DialogService is defined in `HanumanInstitute.MvvmDialogs.Wpf`.

In the constructor, you must pass the ViewLocator from the Avalonia project template.

```c#
public class App : Application
{
    // Registering in ReactiveUI/Splat
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var build = Locator.CurrentMutable;
        build.RegisterLazySingleton(() => (IDialogService)new DialogService(viewLocator: new ViewLocator()));

        SplatRegistrations.Register<MainWindowViewModel>();
        SplatRegistrations.Register<CurrentTimeDialogViewModel>();
        SplatRegistrations.SetupIOC();
    }
```

Replace `ViewLocator.cs` with this, inheriting from [ViewLocatorBase](src/MvvmDialogs.Avalonia/ViewLocatorBase.cs). Alternatively, you can create your custom class that inherits both `IDataTemplate` (for Avalonia) and `IViewLocator` (for MvvmDialogs).

```c#
using HanumanInstitute.MvvmDialogs.Avalonia;
namespace MyDemoApp;

/// <summary>
/// Maps view models to views in Avalonia.
/// </summary>
public class ViewLocator : ViewLocatorBase
{
    /// <inheritdoc />
    protected override string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "");
}
```



#### AppDialogSettings

For the moment there are no application-wide settings used for Avalonia.

## IModalDialogViewModel / ICloseable / IActivable

All dialog ViewModels must implement `IModalDialogViewModel` to set `DialogResult` with the result of the dialog.

In your ViewModel, implement `ICloseable` to add `RequestClose` event which will automatically close the View when raised.

In your ViewModel, implement `IActivable` to add `RequestActivate` event which will automatically activate the View when raised.

## Custom Windows

To display custom dialogs that are not of type `Window` or `ContentDialog`,
your dialog class must implement [IWindow](src/MvvmDialogs/IWindow.cs)
([sample](samples/Wpf/Demo.ModalCustomDialog/AddTextCustomDialog.cs)).
The usage will the same as a standard `Window`.

## Custom Framework Dialogs

This part is the most different from the FantasticFiasco version and will require some work to port.

First, create a custom FrameworkDialogFactory like this. Note that you can create entirely new methods by adding new settings types.

Note: Since the dialog class selection is based on the settings type, you cannot have two different framework dialog classes using the same settings class. However, you can create a base settings class and various derived setting types, even if the derived type adds nothing.

```c#
public class CustomFrameworkDialogFactory : FrameworkDialogFactory
{
    public override IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
    {
        var s2 = (AppDialogSettings)appSettings;
        return settings switch
        {
            TaskMessageBoxSettings s => (IFrameworkDialog<TResult>)new CustomMessageBox(s, s2),
            _ => base.Create<TSettings, TResult>(settings, appSettings)
        };
    }
}
```

Second, you must pass the new FrameworkDialogFactory when creating the DialogService.

```c#
new DialogService(dialogManager: new DialogManager(new CustomFrameworkDialogFactory()))
```

Third, create a framework dialog class that implements `IFrameworkDialog<T>`.
For new methods, you can customize the return type, but to override standard methods, you must specify the expected return type.

Implement `ShowDialogAsync` and access `IWindow` as `owner.AsWrapper().Ref`

```c#
Task<T> ShowDialogAsync(IWindow owner)
```

Finally, if you're creating a new method, you must create a new extension method on `IDialogService`

```c#
namespace HanumanInstitute.MvvmDialogs;

public static class Extensions
{
    public static Task<TaskDialogButton> ShowTaskMessageBoxAsync(
        this IDialogService service, INotifyPropertyChanged ownerViewModel,
        string text, string title = "")
    {
        var settings = new TaskMessageBoxSettings(text, title);
        return ShowTaskMessageBoxAsync(service, ownerViewModel, settings);
    }

    public static Task<TaskDialogButton> ShowTaskMessageBoxAsync(this IDialogService service,
        INotifyPropertyChanged ownerViewModel, TaskMessageBoxSettings? settings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

        DialogLogger.Write($"TASK Caption: {settings?.Title}; Message: {settings?.Text}");

        return service.DialogManager.ShowFrameworkDialogAsync<MessageBoxSettings, TaskDialogButton>(
            ownerViewModel, settings ?? new MessageBoxSettings());
    }
}
```

[Sample here](samples/Wpf/Demo.CustomMessageBox/)

You could create a class library providing a new set of `IDialogService` methods.

## Unit Testing

To unit-test your project, mock [IDialogManager](src/MvvmDialogs/DialogTypeLocators/IDialogManager.cs). All UI interactions pass through `DialogManager`.

Pass your mock when creating your `DialogService`.

```c#
// Using Moq
var dialogManagerMock = new Mock<IDialogManager>();
new DialogService(dialogManager: dialogManagerMock.Object);
```

From here you can configure your mock to validate calls to `Show`, `ShowDialogAsync` and `ShowFrameworkDialogAsync`.

## Differences from FantasticFiasco/mvvm-dialogs

It is very easy to port an application from `FantasticFiasco/mvvm-dialogs` to `HanumanInstitute.MvvmDialogs`, yet there are also differences due to the fact that this library's API is framework-agnostic.

The internal code structure is completely different, while the public API remains mostly compatible.

Here are the differences:
- Namespace changed from `MvvmDialogs` to `HanumanInstitute.MvvmDialogs`
- Platform-specific code is in separate Wpf/Avalonia assembly
- XAML registration is no longer required
- All dialogs are shown using async methods. For WPF (only), sync methods remain available for compatibility.
- Custom dialogs are treated the same way as standard dialogs
- ICloseable / IActivable allow easily closing and activating the View from the ViewModel.
- Framework dialogs use framework-agnostic settings classes.
- There is no longer a separate namespace for each framework dialog.
- Framework dialog methods return the selected value instead of bool.
- FolderBrowserDialog has been renamed to OpenFolderDialog.
- Factory classes are implemented differently.
- Default naming convention, for `ViewModels/MainViewModel`, FantasticFiasco looks for `Views/Main`. This version looks for `Views/MainView`.
- Maps view models to views using Avalonia's ViewLocator design that is easily customizable.

## Contributions Are Welcomed

Implementation could easily be added for UWP, WinUI and Blazor.
I will leave this task to someone who is going to use it with such framework.

Contributions are also welcomed to add unit tests and automated builds.

Todo: Avalonia Open/Save dialogs need to display validation messages based on settings.
