# Change Log

All notable changes to this project will be documented in this file.

## 2.1 - 2023-12-30

This version introduces some breaking changes.

- StrongViewLocator must now have its registrations in its constructor. [See updated documentation.](https://github.com/mysteryx93/HanumanInstitute.MvvmDialogs?tab=readme-ov-file#strongviewlocator). This change was required because 2 separate instances are created: one for `MvvmDialogs` and one for `Avalonia` defined in `App.axaml`.
- Now apply picker `SuggestedStartLocation` and `SuggestedFileName`. The property names and types have been modified.
- Complete overhaul and simplification of `FileSystem` classes. `IPathInfoFactory` and `IBookmarkFileSystem` have been removed. [See new documentation here.](https://github.com/mysteryx93/HanumanInstitute.MvvmDialogs?tab=readme-ov-file#cross-platform-file-access) `DialogStorageFile` got renamed to `DesktopDialogStorageFile` and `DesktopDialogStorageFactory` is available.

Other changes:

- Dialog show calls will now be ignored in design mode to avoid errors
- Updated `CrossPlatform.Browser` to run
- ViewLocatorBase now calls `CreateViewInstance` virtual method instead of `Activator.CreateInstance` for easier customization.

Happy New Year!! May 2024 be the best year in a while

## 2.0 - 2023-06-14

- Updated for Avalonia v11.0.0
- Changed license to MIT
- Removed AppSettings. Design was clumsy, and when it's actually needed, it doesn't work well with the modular design.
- AllowConcurrentDialogs option has been moved to the DialogManager
- Other options (in WPF) have been moved to the DialogFactory
- Avalonia.MessageBox: added an option to display message boxes as windows or popups. Option is available when configuring the DialogFactory: new DialogFactory().AddMessageBox(MessageBoxMode.Popup)

## 2.0-rc1 - 2023-06-10

- ViewModel event handling is now supported for ViewModels shown in DialogHost, FluentContentDialog and FluentTaskDialog
- DialogHost now supports mobile back button
- DialogHost can now display any type of content: ViewModel, Control, or direct content. Renamed ContentViewModel property to Content.
- Avalonia now targets .Net Standard 2.0
- Target Avalonia UI v2.0-rc1.1

## 2.0-preview7 - 2023-03-18

- Automatically set MainWindow in Avalonia when showing the first window

## 2.0-preview6 - 2023-03-06

- Async close confirmation now works properly with mobile navigation
- Added an application setting: AllowConcurrentDialogs. When False (default), it will wait for the previous dialog to close before showing the next one. If you call 3 message boxes at the same time, it will show the message boxes one after the other instead of 3 on top of each other.
- Added support for DialogHost.Avalonia popup views.
- Added support for  Aura.UI message boxes for MvvmDialogs v1.4.1 as Aura.UI does not yet support Avalonia11.

## 2.0-preview5 - 2023-03-02

StrongViewLocator to register ViewModel-View combinations in a strongly-typed way to avoid the use of reflection. Useful if you want to use Assembly Trimming! Works for both desktop and mobile and selects the View accordingly.

Sample ViewLocator registration:

```
locator = new StrongViewLocator() { ForceSinglePageNavigation = false }
    .Register<MainViewModel, MainView, MainWindow>()
    .Register<CurrentTimeViewModel, CurrentTimeView, CurrentTimeWindow>()
    .Register<ConfirmCloseViewModel, ConfirmCloseView, ConfirmCloseWindow>();

build.RegisterLazySingleton(() => (IDialogService)new DialogService(
    new DialogManager(viewLocator: locator),
    viewModelFactory: x => Locator.Current.GetService(x)));
```

It could be done even better. This StaticViewLocatorGenerator (usage here) could be adapted to be more generic and serve for our needs. It would use a Source Generator to generate ViewModel-View mapping at compile-time using naming standards and removing the need for reflection. If someone has experience with Source Generators, contribution is welcomed!

## 2.0-preview4 - 2023-02-22

- Fixed file dialogs filters that were broken
- Removed dependency on Reactive

## 2.0-preview3 - 2023-02-16

It now supports 'magic' mobile navigation on Android and iOS using the exact same API, allowing to convert MVVM desktop apps into mobile with very little efforts. Mobile back navigation is also automatically supported.

Documentation has been updated.

Supporting mobile navigation required extensive internal structural changes which resulted in very little changes to the API.

I believe that these are the only minor breaking changes

- The default ViewLocator now replaces ViewModel with Window on desktop and View on mobile. If you want the old standard of only replacing with View, you can easily create your ViewLocator with a single line to replace ViewModel with View.
- File/folder dialogs now return IDialogStorageFile and IDialogStorageFolder instead of string. To get the same path string you had before (on desktop), simply add .LocalPath.

Known limitations:

- Mobile views are held in a weak cache, and non-visible views should be released from memory when memory is needed. The unit test to ensure this happens does not currently pass. Contribution is welcomed to find the problem.
- Closing async cancellation does not yet work with mobile navigation

## 1.4.1 - 2022-09-03

- Renamed the new ViewLoaded/ViewClosing/ViewClosed methods to OnLoaded/OnClosing/OnClosed. Interface names remain the same.
- Fixed a crash with FluentAvalonia TaskDialog when pressing escape.
- When owner is null, it will now use the main window, or create a dummy window.

## 1.4.0 - 2022-08-29

Handling Loading, Closing and Closed events presents a few annoyances.

Loading is a common business concern. Why would you have to write code in your View for it?

Closing is generally used to display a confirmation before exit. Calling async code from the Closing event would require complex code, both in the ViewModel and the View.

Closed cannot even call a command via an XAML behavior!

As a simple solution, you can implement IViewLoaded, IViewClosing and/or IViewClosed from your ViewModel with no code required in your View.

- IViewLoaded, IViewClosing and IViewClosed interfaces have been added. See documentation.
- Owner parameters are now optional, it's up to the implementation as to whether or not it is supported
- IWindow has been renamed to IView
- Fixed a bug with Fluent TaskMessageBox

## 1.3.1 - 2022-07-01

- File dialog filters can now take extensions with or without the dot
- FluentAvalonia TaskDialog now supports default buttons
- Open/save file framework dialog settings are now more consistent in Avalonia and WPF

## 1.3.0 - 2022-06-16

- Calls will only be dispatched if not already on UI thread
- Redesigned FrameworkDialogFactory to be simpler and more modular
- MessageBox for Avalonia is now split into a separate assembly `HanumanInstitute.MvvmDialogs.Avalonia.MessageBox`, since Avalonia doesn't have built-in support for message boxes
- `MvvmDialogs.Avalonia`, removed reference to MessageBox.Avalonia
- Added preliminary support for FluentAvalonia `HanumanInstitute.MvvmDialogs.Avalonia.Fluent`

TODO:
- Support icons with Fluent MessageBox TaskDialogs
- Support more complex usage of Fluent dialogs

## 1.2.3 - 2022-06-07

- Fixed an issue with thread-safety

## 1.2.2 - 2022-06-07

- DialogManager is now thread-safe, dispatching UI calls to the UI thread
- Added optional `dispatcher` parameter to DialogManager

## 1.2.1 - 2022-05-29

Made the library more friendly for unit tests. One area that caused trouble is the creation of the
dialog view model `new MyDialogViewModel()`. If it has dependencies, you might use `Locator.MyDialogViewModel`,
but then that's not unit-test friendly.

As a solution, you will now create the view model using
`IDialogService.CreateViewModel<T>`.
It will call a ViewModelFactory function set in the DialogService constructor.

- Added `viewModelFactory` parameter to DialogService constructor
- Moved `viewLocator` constructor parameter from DialogService to DialogManager
- Updated all demos to use correct constructor parameters
- Updated all demos to use `IDialogService.CreateViewModel<T>`
- Added unit test sample project: `Demo.ModalDialog.Tests`

## 1.2.0 - 2022-05-28

- Logging is now done using standard `ILogger<DialogService>` interface. See doc
- Revamped the whole logging mechanisms
- All demos now output logs to the Debug window
- Added `IWindow.RefObj`, which can be implemented like this for custom dialogs: `public object RefObj => this;`
- `IDialogManager.ShowFrameworkDialogAsync` now takes an extra optional parameter `resultToString` to convert the result to string for logging. If null, it uses `object.ToString()`.
- Bug fix: `WPF.DialogService.ShowDialog` (sync method) was not working

## 1.1.0 - 2022-05-26

BREAKING CHANGES
- XAML registration is no longer required and must be removed

If your convention is different than simply replacing 'ViewModel' with 'View':
- You must add a ViewLocator to your project (see doc)
- You must pass this ViewLocator to the DialogService constructor

Adopting Avalonia's ViewLocator design greatly simplifies the code and avoids duplicating the design.

- Internally, the following classes have been removed:
  `DialogTypeLocatorBase, DialogTypeLocatorCache,
  IDialogFactory, NamingConventionDialogTypeLocator, ReflectionDialogFactoryBase, ViewRegistration,
  DialogServiceViews, ReflectionDialogFactory`
- Renamed `IDialogTypeLocator` to `IViewLocator`
- Removed namespace `HanumanInstitute.MvvmDialogs.DialogTypeLocators`

`MvvmDialogs.Avalonia` is no longer signed because dependency `ReactiveUI` is not signed.

## 1.0.0 - 2022-05-07

Fork from FantasticFiasco/mvvm-dialogs

### What's new:
- Support for Avalonia and WPF
- The core is now platform-agnostic

### TODO:
- UWP support
- Blazor support
- WinUI support
- Improving tests
