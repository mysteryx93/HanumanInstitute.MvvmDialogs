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
- [Avalonia](https://avaloniaui.net/) <a href="https://www.nuget.org/packages/HanumanInstitute.MvvmDialogs.Avalonia/"><img src="https://img.shields.io/nuget/v/HanumanInstitute.MvvmDialogs.Avalonia.svg"></a> (mature cross-platform UI framework with WPF-like syntax, including mobile/browser support)

UI Frameworks that can easily be added through community efforts:
- WinUI 3 (promising Android/iOS support but won't support Linux)
- Blazor (full app in web browser)
- UWP (Universal Windows Platform, this thing is dead)

## Table of contents

- [Introduction](#introduction)
- [Generic Usage](#generic-usage)
- [WPF Usage](#wpf-usage)
- [Avalonia Usage](#avalonia-usage)
- [StrongViewLocator](#strongviewlocator)
- [Framework Dialogs](#framework-dialogs)
- [Cross-Platform File Access](#cross-platform-file-access)
- [IModalDialogViewModel / ICloseable / IActivable](#imodaldialogviewmodel--icloseable--iactivable)
- [IViewLoaded / IViewClosing / IViewClosed](#iviewloaded--iviewclosing--iviewclosed)
- [Custom Windows](#custom-windows)
- [Custom Framework Dialogs](#custom-framework-dialogs)
- [Unit Testing](#unit-testing)
- [Logging](#logging)
- [Thread Safety](#thread-safety)
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

Avalonia apps can use the same dialog API for desktop windows and [single-page navigation](#mobileweb-applications).

[Here's a tutorial for building modern cross-platform applications.](https://github.com/mysteryx93/Modern.Net-Tutorial)

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
        var dialogViewModel = dialogService.CreateViewModel<AddTextDialogViewModel>();

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

        var viewModel = dialog.CreateViewModel<SelectPresetViewModel>();
        viewModel.Load(false);
        var result = await dialog.ShowDialogAsync(ownerViewModel, viewModel).ConfigureAwait(true);
        return result == true ? viewModel.SelectedItem : null;
    }

    public static async Task<string?> ShowSavePresetViewAsync(this IDialogService dialog, INotifyPropertyChanged ownerViewModel)
    {
        dialog.CheckNotNull(nameof(dialog));

        var viewModel = dialog.CreateViewModel<SelectPresetViewModel>();
        viewModel.Load(true);
        var result = await dialog.ShowDialogAsync(ownerViewModel, viewModel).ConfigureAwait(true);
        return result == true ? viewModel.PresetName : null;
    }
}
```

To make your code testable, use `IDialogService.CreateViewModel<T>` to create your dialog view models. It will call `viewModelFactory` function that you set in `DialogService` constructor.

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

### XAML Registrations

XAML registrations are no longer required as of v1.1 and these lines must be removed.

    md:DialogServiceViews.IsRegistered="True"

## WPF Usage

Add a reference to `HanumanInstitute.MvvmDialogs.Wpf`

DialogService must be registered in the DependencyInjection container of your choice. Note that IDialogService is defined in `HanumanInstitute.MvvmDialogs` and DialogService is defined in `HanumanInstitute.MvvmDialogs.Wpf`.

To customize ViewModels to Views naming convention, in the constructor, you must pass a ViewLocator.

```c#
public partial class App
{
    // Registering in CommunityToolkit.Mvvm
    protected override void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(new DialogService(
                    new DialogManager(viewLocator: new ViewLocator()),
                    viewModelFactory: x => Ioc.Default.GetService(x)))
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}
```

To associate view models with views, the default naming convention is to replace "ViewModel" with "View".

To specify your own convention, create `ViewLocator.cs` with this, inheriting from [ViewLocatorBase](src/MvvmDialogs.Wpf/ViewLocatorBase.cs). You can also use the `StrongViewLocator` to avoid using reflection. Alternatively, you can create your custom class that inherits `IViewLocator`.

```c#
using HanumanInstitute.MvvmDialogs.Wpf;
namespace MyDemoApp;

/// <summary>
/// Maps view models to views.
/// </summary>
public class ViewLocator : ViewLocatorBase
{
    /// <inheritdoc />
    protected override string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "View");
}
```

`IDialogService` exposes platform-agnostic async methods. For WPF (only), sync methods
are also available. If you plan to ever use the ViewModel with a different UI framework,
it is recommended to use the async methods.

```c#
    private bool? ShowDialog()
    {
        var dialogViewModel = dialogService.CreateViewModel<AddTextDialogViewModel>();
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

#### bool AllowConcurrentDialogs

Gets or sets whether multiple dialogs can be shown at the same time.
If false (default), it will wait for the previous dialog to close before showing the next one.


## Avalonia Usage

Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia`

Register `HanumanInstitute.MvvmDialogs.Avalonia.DialogService` as `IDialogService` in your dependency injection container. Pass a view locator to `DialogManager` to customize how it maps view models to views.

```c#
public class App : Application
{
    // Registering in ReactiveUI/Splat
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var build = Locator.CurrentMutable;
        build.RegisterLazySingleton(() => (IDialogService)new DialogService(
            new DialogManager(
                viewLocator: new ViewLocator(),
                dialogFactory: new DialogFactory().AddMessageBox()),
            viewModelFactory: x => Locator.Current.GetService(x)));

        SplatRegistrations.Register<MainWindowViewModel>();
        SplatRegistrations.Register<CurrentTimeDialogViewModel>();
        SplatRegistrations.SetupIOC();
    }
```

To associate view models with views, the default naming convention (as of v2) is to replace folder "ViewModels" with "Views", and then change suffix from "ViewModel" to "Window" for desktop mode and "View" for mobile/navigation mode. Very often, Window simply contains the View.

To specify your own convention, replace `ViewLocator.cs` with this, inheriting from [ViewLocatorBase](src/MvvmDialogs.Avalonia/ViewLocatorBase.cs). You can also use the `StrongViewLocator` to avoid using reflection. Alternatively, you can create your custom class that inherits `IDataTemplate` (for Avalonia), `IViewLocator` and `IViewLocatorNavigation` (for MvvmDialogs). You can use `UseSinglePageNavigation` to know whether the app is running in desktop or navigation mode.

```c#
using HanumanInstitute.MvvmDialogs.Avalonia;
namespace MyDemoApp;

/// <summary>
/// Maps view models to views.
/// </summary>
public class ViewLocator : ViewLocatorBase
{
    /// <inheritdoc />
    protected override string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "");
}
```

### AppDialogSettings

#### bool AllowConcurrentDialogs

Gets or sets whether multiple dialogs can be shown at the same time.
If false (default), it will wait for the previous dialog to close before showing the next one.

### Mobile/Web Applications

MvvmDialogs uses single-page navigation with Avalonia's `ISingleViewApplicationLifetime`, as in mobile and browser apps. `Show` and `ShowDialogAsync` present pages within the same navigation host.

To use this mode on desktop, set `ForceSinglePageNavigation = true` on the view locator passed to `DialogManager`. With `StrongViewLocator`, set it before registering views. See the [CrossPlatform demo's view locator](samples/Avalonia/CrossPlatform/Demo.CrossPlatform/ViewLocator.cs).

System Back first requests dismissal of an active popup in the current host. With no popup, it goes back in the page history, subject to close confirmation. At the root page with no popup, system Back is left to the platform. For a custom Back button, use `CanGoBack` and `GoBackAsync()` on the manager exposed by `DialogManager.NavigationManager`.

If a modal dialog's owner or navigation host closes first, awaiting `ShowDialogAsync()` throws `OperationCanceledException`.

### Avalonia.MessageBox

Avalonia has no built-in support for message boxes. This extension handles message box requests
using [MessageBox.Avalonia](https://github.com/AvaloniaCommunity/MessageBox.Avalonia) library. `Mode=Window` only supports desktop mode. `Mode=Popup` is cross-platform.

1. Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia.MessageBox`
2. Register the MessageBox handler on IDialogService like this:
 
```c#
new DialogService(new DialogManager(dialogFactory: 
    new DialogFactory().AddMessageBox(MessageBoxMode.Popup)))
```

### Avalonia.Fluent

[FluentAvalonia](https://github.com/amwx/FluentAvalonia/) brings more of Fluent design and WinUI controls into Avalonia.

1. Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia.Fluent`
2. Register the handlers on IDialogService like this:

```c#
new DialogService(new DialogManager(dialogFactory: 
    new DialogFactory().AddFluent(FluentMessageBoxType.ContentDialog)))
```

It will add `IDialogService.ShowContentDialogAsync` and `IDialogService.ShowTaskDialogAsync`.

Additionally, `AddFluent` takes a parameter specifying whether to handle `IDialogService.ShowMessageBoxAsync` calls with ContentDialog or with TaskDialog. 


### Avalonia.AuraUI

This extension handles message box requests using [Aura.UI](https://github.com/PieroCastillo/Aura.UI) library.

1. Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia.AuraUI`
2. Register the MessageBox handler on IDialogService like this:

```c#
new DialogService(new DialogManager(dialogFactory: 
    new DialogFactory().AddMessageBoxAuraUI()))
```

Note: Aura.UI does not yet support Avalonia11 and is thus only available for MvvmDialogs v1.4.1

### Avalonia.DialogHost

[DialogHost.Avalonia](https://github.com/AvaloniaUtils/DialogHost.Avalonia) allows displaying views as popup overlays. FluentAvalonia brings WinUI3's ContentDialog that has title, content and button. On the other hand, DialogHost purely shows your view while giving you full control.

1. Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia.DialogHost`
2. Register the handlers on IDialogService like this:

```c#
new DialogService(new DialogManager(dialogFactory: 
    new DialogFactory().AddDialogHost()))
```

It will add `IDialogService.ShowDialogHostAsync` that takes the following settings.

**ContentViewModel**: The view model of the view to show. The view will be resolved through Avalonia's ViewLocator.

**ClosingHandler**: A handler that will be called when the view is closing, allowing to cancel the close.

**CloseOnClickAway**: Whether to close the view when clicking elsewhere in the parent container.

**CloseOnClickAwayParameter**: The close value to set when closing by clicking away.

**PopupPositioner**: A class allowing to customize the positioning of the dialog.

You can then create extension methods for your views like this

```c#
public static async Task<string?> AskTextAsync(this IDialogService service, 
    INotifyPropertyChanged ownerViewModel, AppDialogSettingsBase? appSettings = null)
{
    if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

    var vm = service.CreateViewModel<TextBoxViewModel>();
    var settings = new DialogHostSettings()
    {
        ContentViewModel = vm,
        CloseOnClickAway = true
    };
    return (string?)await service.ShowDialogHostAsync(ownerViewModel, settings, appSettings);
}
```

[See sample project here](https://github.com/mysteryx93/HanumanInstitute.MvvmDialogs/tree/master/samples/Avalonia/Demo.DialogHost)

## StrongViewLocator

Instead of resolving views via reflection and naming conventions, you can also configure ViewModel-View pairs manually using the `StrongViewLocator`.

This will be necessary if you want to use Assembly Trimming, otherwise it will trim your View classes for a lack of hard references.

Create your View Locator like this, then pass it to your Dialog Manager as explained above.

```c#
public class ViewLocator : StrongViewLocator
{
    public ViewLocator()
    {
        Register<MainViewModel, MainView, MainWindow>()
        Register<CurrentTimeViewModel, CurrentTimeView, CurrentTimeWindow>()
        Register<ConfirmCloseViewModel, ConfirmCloseView, ConfirmCloseWindow>();
    }
}
```

## Framework Dialogs

`IDialogService` provides the following standard framework dialog methods:

- `bool? ShowMessageBoxAsync`
- `IDialogStorageFile? ShowOpenFileDialogAsync`
- `IDialogStorageFile? ShowSaveFileDialogAsync`
- `IDialogStorageFolder? ShowOpenFolderDialogAsync`
- `IReadOnlyList<IDialogStorageFile> ShowOpenFilesDialogAsync`
- `IReadOnlyList<IDialogStorageFolder> ShowOpenFoldersDialogAsync`

As of v2, files and folders are returned as `IDialogStorageFile` and `IDialogStorageFolder` instead of `string`. On desktop, `TryGetLocalPath()` returns the filesystem path. It returns null for browser files and most Android content items; read those through `OpenReadAsync`. `LocalPath` throws when no filesystem path exists. 

For extra dialog options, see Custom Framework Dialogs section. 

## Cross-Platform File Access

To access files and folders across desktop, mobile and web, `IDialogStorageFile` and `IDialogStorageFolder` provide shared methods for working with picked items. Use streams to read and write file contents when a local filesystem path is unavailable.

They copy Avalonia's `IStorageFile` and `IStorageFolder` [documented here](https://docs.avaloniaui.net/docs/concepts/services/storage-provider/storage-item). The reasons to copy those interfaces are to allow you to write platform-agnostic code, and to have consistency between WPF and Avalonia. 

Avalonia provides various methods in `IStorageProvider` to get `IStorageFile` and `IStorageFolder` instances, as [documented here](https://docs.avaloniaui.net/docs/concepts/services/storage-provider/). You can convert them to the `MvvmDialogs` interface using the `ToDialog()` extension method.

For persistent access in Avalonia, check whether the picked item is an `AvaloniaDialogStorageItem`. It exposes `CanBookmark`, `SaveBookmarkAsync()`, and `ReleaseBookmarkAsync()`. Reopen saved bookmarks through Avalonia's `IStorageProvider`.

Here's a example of a service that returns the Documents folder.

```c#
public class StorageService : IStorageService
{
    protected virtual IStorageProvider Storage => _storage ??= GetTopLevel()?.StorageProvider ?? throw new NullReferenceException("No StorageProvider found.");
    private IStorageProvider? _storage;
    
    public async Task<IDialogStorageFolder?> GetDocumentsFolderAsync()
    {
        var result = await Storage.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        return result?.ToDialog();
    }

    private TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            var visualRoot = viewApp.MainView?.GetVisualRoot();
            return visualRoot as TopLevel;
        }
        return null;
    }
}
```
Using your `StorageService` class, you can show a SaveDialog with default location set to Documents.
```c#
var _storage = new StorageService();

var settings = new SaveFileDialogSettings
{
    SuggestedStartLocation = await _storage.GetDocumentsFolderAsync()
};
var file = await _dialogService.ShowSaveFileDialogAsync(this, settings);
```

On desktop platforms, you can convert a file path to `IDialogStorageFile` using either `HanumanInstitute.MvvmDialogs.FileSystem.DesktopDialogStorageFile` or `Avalonia.Platform.Storage.FileIO.BclStorageFile` which are nearly the same.

`DesktopDialogStorageFile` has these differences: it accepts a string path in the constructor instead of only a `FileInfo`, it is of type `IDialogStorageFile` instead of `IStorageFile`, and it is available in non-Avalonia projects. `IDesktopDialogStorageFactory` is also available for easier unit test mocking. 

## IModalDialogViewModel / ICloseable / IActivable

All dialog ViewModels must implement `IModalDialogViewModel` to set `DialogResult` with the result of the dialog.

In your ViewModel, implement `ICloseable` to add `RequestClose` event which will automatically close the View when raised.

In your ViewModel, implement `IActivable` to add `RequestActivate` event which will automatically activate the View when raised.

## IViewLoaded / IViewClosing / IViewClosed

Handling Loading, Closing and Closed events presents a few annoyances.

Loading is a common business concern. Why would you have to write code in your View for it?

Closing is generally used to display a confirmation before exit. Calling async code from the Closing event would require complex code, both in the ViewModel and the View.

Closed cannot even call a command via an XAML behavior! Yet you need it to unregister some events to avoid memory leaks.

As a simple solution, you can implement `IViewLoaded`, `IViewClosing` and/or `IViewClosed` from your ViewModel with no code required in your View.

**IViewLoaded**  
`void OnLoaded();` Called when the view is loaded.

**IViewClosed**  
`void RaiseClosed();` Called when the view is closed.

**IViewClosing**  
`void RaiseClosing(CancelEventArgs e);` Called when closing the view.  
`Task OnClosingAsync(CancelEventArgs e);` Called if `e.Cancel` has been set to True in `OnClosing`

Setting `e.Cancel = true` in `OnClosing` will...

1. Cancel the close
2. Call OnClosingAsync
3. Setting `e.Cancel = false` in `OnClosingAsync` will close the view

See [Wpf/Demo.ViewEvents](samples/Wpf/Demo.ViewEvents/MainWindowViewModel.cs) for a sample implementation.

**IMPORTANT**: To use these added features in your main ViewModel, your main window must be initialized via `IDialogService`.

Initializing your main window in WPF in `App.xaml.cs`

```c#
protected override void OnStartup(StartupEventArgs e)
{
    // ...
    var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
    var vm = dialogService.CreateViewModel<MainWindowViewModel>();
    dialogService.Show(null, vm);
    Application.Current.MainWindow = Application.Current.Windows[0];
}
```

Initializing your main window in Avalonia in `App.axaml.cs`

```c#
public override void OnFrameworkInitializationCompleted()
{
    GC.KeepAlive(typeof(DialogService));
    DialogService.Show(null, MainWindow);
    base.OnFrameworkInitializationCompleted();
}

public static MainWindowViewModel MainWindow => Locator.Current.GetService<MainWindowViewModel>()!;
public static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;
```

## Custom Windows

To display custom dialogs that are not of type `Window` or `ContentDialog`,
your dialog class must implement [IView](src/MvvmDialogs/IView.cs)
([sample](samples/Wpf/Demo.ModalCustomDialog/AddTextCustomDialog.cs)).
The usage will be the same as a standard `Window`.

## Custom Framework Dialogs

To replace a built-in framework dialog, subclass your platform's `DialogFactoryBase` and override `ShowDialogAsync`. For synchronous WPF calls, also override `ShowDialog`. Handle the relevant settings type, return the result expected by `IDialogService`, and pass other requests along the chain by calling the base implementation.

Register the factory with `DialogManager` through its `dialogFactory` argument. The custom folder picker demos show the factory and its registration for [Avalonia](samples/Avalonia/Demo.CustomOpenFolderDialog/) and [WPF](samples/Wpf/Demo.CustomOpenFolderDialog/).

To add a new dialog, define a settings type derived from `DialogSettingsBase`. You can expose a typed `IDialogService` extension that calls `service.DialogManager.ShowFrameworkDialogAsync(...)`. See the [Fluent dialog extensions](src/MvvmDialogs.Avalonia.Fluent/DialogServiceExtensions.cs).

### Avalonia overlays

For inline dialogs, [OverlayExtensions](src/MvvmDialogs.Avalonia/OverlayExtensions.cs) provides two helpers on the dialog owner. Both register a dismissal action for Back and request dismissal when the owner leaves.

- Use `RunOverlayAsync()` when the dialog provides its own overlay and needs the owner's `TopLevel`. See [FluentContentView](src/MvvmDialogs.Avalonia.Fluent/FluentContentView.cs).
- Use `WithOverlaySurfaceAsync()` when the dialog needs a temporary `ContentControl` over the owner. See [MessageBoxDialogFactory](src/MvvmDialogs.Avalonia.MessageBox/MessageBoxDialogFactory.cs).

### RunUiAsync

There is a useful extension method in `HanumanInstitute.MvvmDialogs.Wpf` and `HanumanInstitute.MvvmDialogs.Avalonia` to run a synchronous UI method as an async method:

```c#
UiExtensions.RunUiAsync(func)
```


## Unit Testing

To unit-test your project, mock [IDialogManager](src/MvvmDialogs/DialogTypeLocators/IDialogManager.cs). All UI interactions pass through `DialogManager`.

Pass your mock when creating your `DialogService`.

```c#
// Using Moq
var dialogManagerMock = new Mock<IDialogManager>();
new DialogService(dialogManager: dialogManagerMock.Object);
```

From here you can configure your mock to validate calls to `Show`, `ShowDialogAsync` and `ShowFrameworkDialogAsync`.

One problem you may face with unit testing dialogs is with the creation of your view model if it has dependencies.

In the `DialogService` constructor, pass `viewModelFactory`

    new DialogService(viewModelFactory: x => Locator.Current.GetService(x))

Create your view model instances using `IDialogService.CreateViewModel<T>`

    var vm = dialogService.CreateViewModel<MyDialogViewModel>();

*This* can easily be mocked. [Here's a sample unit test](samples/Avalonia/Demo.ModalDialog.Tests/MainWindowViewModelTests.cs)

## Logging

To enable logging, create a `DialogManager` and pass an `ILogger<DialogManager>` to its constructor.

```c#
var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());
var dialogService = new DialogService(
    new DialogManager(logger: loggerFactory.CreateLogger<DialogManager>()));
```

## Thread Safety

All methods to show windows and dialogs are thread-safe and can be called from background threads.

`IDialogService.Activate` and `IDialogService.Close` are NOT thread-safe and must be called from the UI thread.

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
- Logging is done using standard ILogging interface
- Supports view navigation on mobile devices with Avalonia! 

## Contributions Are Welcomed

TODO:
- Implement for WinUI 3 (I'll leave this task to someone who is going to use it)
- Implement for UWP (this thing is dead... not worth implementing IMO)
- Implement for Blazor?
- Ensure that non-visible views can be released from memory
- Adapt this [StaticViewLocatorGenerator](https://github.com/zkSNACKs/WalletWasabi/blob/1b8d142d0ec6892f13f6c50c3fec5d7ceb7f06ce/WalletWasabi.Fluent.Generators/StaticViewLocatorGenerator.cs#L12) ([usage here](https://github.com/zkSNACKs/WalletWasabi/blob/1b8d142d0ec6892f13f6c50c3fec5d7ceb7f06ce/WalletWasabi.Fluent/ViewLocator.cs#L7))
  to replace the default naming convention ViewLocator with Source Generator instead of Reflection

### Author

Brought to you by [Etienne Charland aka Hanuman](https://www.spiritualselftransformation.com/). Made by a Lightworker in his spare time.
