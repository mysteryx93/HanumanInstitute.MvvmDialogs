# Change Log

All notable changes to this project will be documented in this file.

## 1.3.0

- Calls will only be dispatched if not already on UI thread
- Redesigned FrameworkDialogFactory to be simpler and more modular
- MessageBox for Avalonia is now split into a separate assembly `HanumanInstitute.MvvmDialogs.Avalonia.MessageBox`, since Avalonia doesn't have built-in support for message boxes
- `MvvmDialogs.Avalonia`, removed reference to MessageBox.Avalonia


## 1.2.3 - 2002-06-07

- Fixed an issue with thread-safety

## 1.2.2 - 2002-06-07

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
