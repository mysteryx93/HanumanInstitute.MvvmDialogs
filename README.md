<h1 align="center">
    <br>
    <img src="https://raw.githubusercontent.com/FantasticFiasco/mvvm-dialogs/master/doc/resources/Icon_200x200.png" alt="MVVM Dialogs" width="200">
    <br>
    MVVM Dialogs
    <br>
</h1>

<h4 align="center">Library simplifying the concept of opening dialogs from a view model when using MVVM</h4>
<h6 align="center">Cross-platform solution derived from <a href="https://github.com/FantasticFiasco/mvvm-dialogs">FantasticFiasco/mvvm-dialogs</a></h6>

<p align="center">
    <a href="https://www.nuget.org/packages/MvvmDialogs/"><img src="https://img.shields.io/nuget/v/MvvmDialogs.svg"></a>
    <a href="https://www.nuget.org/packages/MvvmDialogs/"><img src="https://img.shields.io/nuget/dt/MvvmDialogs.svg"></a>
</p>

UI Frameworks currently supported:
- WPF (Windows Presentation Foundation)
- [Avalonia](https://avaloniaui.net/) (mature cross-platform UI framework with WPF-like syntax)

UI Frameworks that can easily be added through community efforts:
- UWP (Universal Windows Platform)
- WinUI 3 (promising Android/iOS support but won't support Linux)
- Blazor (full app in web browser)

## Table of contents

- [Introduction](#introduction)
- [Generic Usage](#generic-usage)
- [WPF Usage](#wpf-usage)
- [Avalonia Usage](#avalonia-usage)
- [IModalDialogViewModel / ICloseable / IActivable](#imodaldialogviewmodel-icloseable-iactivable)
- [Custom Windows](#custom-windows)
- [Custom Framework Dialogs](#custom-framework-dialogs)
- [Differences from FantasticFiasco/mvvm-dialogs](#differences-from-fantasticfiasco-mvvm-dialogs)
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

## WPF Usage

Add a reference to `HanumanInstitute.MvvmDialogs.Wpf`

You must be decorate the views to display with the attached property `DialogServiceViews.IsRegistered`:

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
        return dialogService.ShowDialog(this, dialogViewModel);
    }
}
```

Here is a useful extension method in `HanumanInstitute.MvvmDialogs` to run a synchronous UI method as an async method:

```c#
window.RunUiAsync(func)
```

## Avalonia Usage

Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia`

You must be decorate the views to display with the attached property `DialogServiceViews.IsRegistered`:

```xaml
<Window
    x:Class="Demo.MainView"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:md="https://github.com/mysteryx93/HanumanInstitute.MvvmDialogs"
    md:DialogServiceViews.IsRegistered="True">

  ...

</UserControl>
```

DialogService must be registered in the DependencyInjection container of your choice. Note that IDialogService is defined in `HanumanInstitute.MvvmDialogs` and DialogService is defined in `HanumanInstitute.MvvmDialogs.Wpf`.

```c#
public class App : Application
{
    // Registering in ReactiveUI/Splat
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var build = Locator.CurrentMutable;
        build.RegisterLazySingleton(() => (IDialogService)new DialogService());

        SplatRegistrations.Register<MainWindowViewModel>();
        SplatRegistrations.Register<CurrentTimeDialogViewModel>();
        SplatRegistrations.SetupIOC();
    }
```

[There is currently an issue where the Avalonia previewer will not recognize the XAML namespace.](https://github.com/AvaloniaUI/Avalonia/issues/7200)
The workaround is to force your assembly to be loaded by adding `GC.KeepAlive(typeof(DialogService))` to `App.OnFrameworkInitializationCompleted`.

```c#
public override void OnFrameworkInitializationCompleted()
{
    GC.KeepAlive(typeof(DialogService));
    ...
}
```

## IModalDialogViewModel / ICloseable / IActivable

All dialog ViewModels must implement `IModalDialogViewModel` to set `DialogResult` with the result of the dialog.

In your ViewModel, implement `ICloseable` to add `RequestClose` event which will automatically close the View when raised.

In your ViewModel, implement `IActivable` to add `RequestActivate` event which will automatically activate the View when raised.

## Custom Windows

To display custom dialogs that are not of type `Window` or `ContentDialog`,
your dialog class must implement `IWindow`. The usage will the same as a standard `Window`.

## Custom Framework Dialogs

MVVM Dialogs is by default opening the standard Windows message box or the open file dialog when asked to. This can however be changed by providing your own implementation of `IFrameworkDialogFactory` to `DialogService`.

## Differences from FantasticFiasco/mvvm-dialogs

It is very easy to port an application from `FantasticFiasco/mvvm-dialogs` to `HanumanInstitute.MvvmDialogs`, yet there are also differences due to the fact that this library's API is framework-agnostic.

The internal code structure is completely different, while the public API remains mostly compatible.

Here are the differences:
- Namespace changed from `MvvmDialogs` to `HanumanInstitute.MvvmDialogs`
- Platform-specific code is in separate Wpf/Avalonia assembly
- XAML namespace changed from `https://github.com/FantasticFiasco/mvvm-dialogs` to `https://github.com/mysteryx93/HanumanInstitute.MvvmDialogs`
- All dialogs are shown using async methods. For WPF (only), sync methods remain available for compatibility.
- Custom dialogs are treated the same way as standard dialogs
- ICloseable / IActivable allow easily closing and activating the View from the ViewModel.
- Framework dialogs use framework-agnostic settings classes.
- There is no longer a separate namespace for each framework dialog.
- Framework dialog methods return the selected value instead of bool.
- FolderBrowserDialog has been renamed to OpenFolderDialog.
- Factory classes are implemented differently.

## Contributions Are Welcomed

Implementation could easily be added for UWP, WinUI and Blazor.
I will leave this task to someone who is going to use it with such framework.
