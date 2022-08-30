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

Optinally, create `ViewLocator.cs` with this, inheriting from [ViewLocatorBase](src/MvvmDialogs.Wpf/ViewLocatorBase.cs). Alternatively, you can create your custom class that inherits `IViewLocator`.

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

Replace `ViewLocator.cs` with this, inheriting from [ViewLocatorBase](src/MvvmDialogs.Avalonia/ViewLocatorBase.cs). Alternatively, you can create your custom class that inherits both `IDataTemplate` (for Avalonia) and `IViewLocator` (for MvvmDialogs).

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

#### AppDialogSettings

For the moment there are no application-wide settings used for Avalonia.

### Avalonia.MessageBox

Avalonia has no built-in support for message boxes. This extension handles message box requests
using [MessageBox.Avalonia](https://github.com/AvaloniaCommunity/MessageBox.Avalonia) library.

1. Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia.MessageBox`
2. Register the MessageBox handler on IDialogService like this:
 
```c#
new DialogService(new DialogManager(dialogFactory: new DialogFactory().AddMessageBox()))
```

### Avalonia.Fluent

[FluentAvalonia](https://github.com/amwx/FluentAvalonia/) brings more of Fluent design and WinUI controls into Avalonia.

1. Add a reference to `HanumanInstitute.MvvmDialogs.Avalonia.Fluent`
2. Register the handlers on IDialogService like this:

```c#
new DialogService(new DialogManager(dialogFactory: new DialogFactory().AddFluent(FluentMessageBoxType.ContentDialog)))
```

It will add `IDialogService.ShowContentDialogAsync` and `IDialogService.ShowTaskDialogAsync`.

Additionally, `AddFluent` takes a parameter specifying whether to handle `IDialogService.ShowMessageBoxAsync` calls with ContentDialog or with TaskDialog. 


## IModalDialogViewModel / ICloseable / IActivable

All dialog ViewModels must implement `IModalDialogViewModel` to set `DialogResult` with the result of the dialog.

In your ViewModel, implement `ICloseable` to add `RequestClose` event which will automatically close the View when raised.

In your ViewModel, implement `IActivable` to add `RequestActivate` event which will automatically activate the View when raised.

## IViewLoaded / IViewClosing / IViewClosed

Handling Loading, Closing and Closed events presents a few annoyances.

Loading is a common business concern. Why would you have to write code in your View for it?

Closing is generally used to display a confirmation before exit. Calling async code from the Closing event would require complex code, both in the ViewModel and the View.

Closed cannot even call a command via an XAML behavior!

As a simple solution, you can implement `IViewLoaded`, `IViewClosing` and/or `IViewClosed` from your ViewModel with no code required in your View.

**IViewLoaded**  
`void ViewLoaded();` Called after the view is displayed.

**IViewClosed**  
`void ViewClosed();` Called after the view is closed.

**IViewClosing**  
`void ViewClosing(CancelEventArgs e);` Called when closing the view.  
`Task ViewClosingAsync(CancelEventArgs e);` Called if `e.Cancel` has been set to True in `ViewClosing`

Setting `e.Cancel = true` in `ViewClosing` will...

1. Cancel the close
2. Call ViewClosingAsync
3. Setting `e.Cancel = false` in `ViewClosingAsync` will close the view

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
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        DialogService.Show(null, MainWindow);
        desktop.MainWindow = desktop.Windows[0];
    }
    base.OnFrameworkInitializationCompleted();
}

public static MainWindowViewModel MainWindow => Locator.Current.GetService<MainWindowViewModel>()!;
public static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;
```

## Custom Windows

To display custom dialogs that are not of type `Window` or `ContentDialog`,
your dialog class must implement [IView](src/MvvmDialogs/IView.cs)
([sample](samples/Wpf/Demo.ModalCustomDialog/AddTextCustomDialog.cs)).
The usage will the same as a standard `Window`.

## Custom Framework Dialogs

This part is the most different from the FantasticFiasco version and will require some work to port. The implementation further changed as of v1.3 to be simpler and more modular.

First, create a custom DialogFactory like this. Note that you can create entirely new methods by adding new settings types.

For new methods, you can customize the return type, but to override standard methods, you must specify the expected return type.

```c#
public class CustomDialogFactory : DialogFactoryBase
{
    public CustomDialogFactory(IDialogFactory? chain = null)
        : base(chain)
    {
    }

    public override async Task<object?> ShowDialogAsync<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s, appSettings),
            _ => base.ShowDialogAsync(owner, settings, appSettings)
        };

    private async Task<string?> ShowOpenFolderDialogAsync(WindowWrapper owner, OpenFolderDialogSettings settings, AppDialogSettings appSettings) =>
        "Action here";
}
```

Second, create an extension method to facilitate registration of the new Dialog Factory.

```c#
public static class DialogFactoryExtensions
{
    public static IDialogFactory AddCustomOpenFolder(this IDialogFactory factory) => new CustomDialogFactory(factory);
}
```

Third, you must register DialogFactory when creating the DialogService. You can form a chain of DialogFactory 
where each instance handles some types and passes unhandled requests to the next DialogFactory in the chain. 
The extension method facilitates the creation of such chain. In this example, our new class handles OpenFolder, 
and all other requests fallback to the default implementation.

```c#
new DialogService(dialogManager: new DialogManager(
    dialogFactory: new DialogFactory().AddCustomOpenFolder()))
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

[Sample demo here](samples/Wpf/Demo.CustomMessageBox/)

[MvvmDialogs.Avalonia.MessageBox is an example of custom implementation](src/MvvmDialogs.Avalonia.MessageBox)

You could create a class library providing a new set of `IDialogService` methods.

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

## Contributions Are Welcomed

Todo:
- Implement for WinUI 3 (I'll leave this task to someone who is going to use it)
- Implement for UWP (this thing is dead... not worth implementing IMO)
- Implement for Blazor?
- Automated builds?

### Author

Brought to you by [Etienne Charland aka Hanuman](https://www.spiritualselftransformation.com/). Made by a Lightworker in his spare time.
