# Change Log

All notable changes to this project will be documented in this file.

## 1.2.0 - 2022-05-28

- Logging is now done using standard `ILogger<DialogService>` interface. See doc
- Revamped the whole logging mechanisms
- All demos now output logs to the Debug window
- Added `IWindow.RefObj`, which can be implemented like this for custom dialogs: `public object RefObj => this;`
- IDialogManager.ShowFrameworkDialogAsync now takes an extra optional parameter `resultToString` to convert the result to string for logging. If null, it uses `object.ToString()`.
- Bug fix: WPF.DialogService.ShowDialog (sync method) was not working

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
