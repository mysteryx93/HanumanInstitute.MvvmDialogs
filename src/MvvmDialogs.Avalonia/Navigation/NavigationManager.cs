using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <inheritdoc cref="INavigationManager"/>
public class NavigationManager : ReactiveObject, INavigationManager
{
    /// <summary>
    /// Navigation history contains only ViewModels to avoid keeping all constructed user controls in memory. The Views can be reconstructed from the ViewModels.
    /// </summary>
    private readonly List<INotifyPropertyChanged> _history = new();
    private readonly ViewCache _viewCache = new();
    private readonly List<DialogTask> _dialogs = new();

    /// <summary>
    /// Returns the navigation history.
    /// </summary>
    public IReadOnlyList<INotifyPropertyChanged> History => _history;

    /// <summary>
    /// Initializes a new instance of the NavigationManager class.
    /// </summary>
    /// <param name="customNavigationRoot">If specified, a custom user control will be set as the main view instead of the default NavigationRoot.</param>
    public void Launch(Control? customNavigationRoot = null)
    {
        // Initialize the NavigationRoot as the main application view.
        var app = Application.Current?.ApplicationLifetime;
        if (app is ISingleViewApplicationLifetime appSingle)
        {
            appSingle.MainView = customNavigationRoot ?? new NavigationRoot();
            appSingle.MainView.DataContext = this;
            appSingle.MainView.Loaded += (_, _) =>
            {
                TopLevel.GetTopLevel(appSingle.MainView)!.BackRequested += TopLevel_BackRequested;
            };
        }
        else if (app is IClassicDesktopStyleApplicationLifetime appDesktop)
        {
            appDesktop.MainWindow = customNavigationRoot as Window ?? new NavigationRootWindow();
            appDesktop.MainWindow.DataContext = this;
        }
    }
    
    /// <summary>
    /// Handle the mobile back button.
    /// </summary>
    private void TopLevel_BackRequested(object? sender, RoutedEventArgs e)
    {
        if (CancellableActions.Any)
        {
            // Cancel message boxes
            CancellableActions.CancelLast();
            e.Handled = true;
        }
        else if (CurrentView != null && _history.Count > 1)
        {
            // Cancel normal views
            var current = CurrentViewModel!;
            CurrentView.AsWrapper(this).Close();
            e.Handled = !object.ReferenceEquals(CurrentViewModel, current);
        }
    }

    /// <inheritdoc />
    public UserControl? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }
    private UserControl? _currentView;

    /// <inheritdoc />
    public INotifyPropertyChanged? CurrentViewModel => (INotifyPropertyChanged?)CurrentView?.DataContext;

    /// <inheritdoc />
    public UserControl? GetViewForViewModel(INotifyPropertyChanged viewModel) =>
        _viewCache.GetViewForViewModel(viewModel);

    /// <inheritdoc />
    public void Show(INotifyPropertyChanged viewModel, Type viewType)
    {
        CurrentView = _viewCache.GetView(viewModel, viewType);
        _history.Remove(viewModel);
        if (_dialogs.Any())
        {
            // Keep only 1 non-dialog history within a dialog.
            var last = _history.Last();
            if (!_dialogs.Any(x => object.ReferenceEquals(x.ViewModel, last)))
            {
                _history.Remove(last);
            }
        }
        _history.Add(viewModel);
    }

    /// <inheritdoc />
    public Task ShowDialogAsync(INotifyPropertyChanged viewModel, Type viewType, INotifyPropertyChanged ownerViewModel)
    {
        var view = _viewCache.GetView(viewModel, viewType);
        CurrentView = view;

        if (_dialogs.Any(x => object.ReferenceEquals(x.ViewModel, viewModel)))
        {
            throw new InvalidOperationException("Dialog is already shown.");
        }
        var dialog = new DialogTask(viewModel, ownerViewModel);
        _dialogs.Add(dialog);
        _history.Add(viewModel);
        return dialog.Completion.Task;
    }

    /// <inheritdoc />
    public void Close(INotifyPropertyChanged viewModel, Type viewType)
    {
        // Remove from history, whether or not it is currently visible.
        _history.Remove(viewModel);
        // If waiting for dialog result, stop waiting.
        var dialog = _dialogs.FirstOrDefault(x => object.ReferenceEquals(x.ViewModel, viewModel));
        if (dialog != null)
        {
            dialog.Completion.SetResult(true);
            _dialogs.Remove(dialog);
        }
        // If visible, show previous one, or dialog owner. Ignore owner for non-dialogs.
        if (object.ReferenceEquals(CurrentView?.DataContext, viewModel))
        {
            var prev = dialog?.OwnerViewModel ?? _history.LastOrDefault();
            if (prev != null)
            {
                var prevView = _viewCache.GetViewForViewModel(prev);
                // Remove all history after owner.
                if (dialog?.OwnerViewModel != null)
                {
                    var pos = _history.IndexOf(dialog.OwnerViewModel);
                    if (pos > -1 && _history.Count > pos + 1)
                    {
                        _history.RemoveRange(pos + 1, _history.Count - pos - 1);
                    }
                }
                CurrentView = prevView;
            }
        }
    }

    /// <inheritdoc />
    public bool Activate(INotifyPropertyChanged viewModel, Type viewType)
    {
        if (_history.Contains(viewModel))
        {
            _history.Remove(viewModel);
            _history.Add(viewModel);
            var view = _viewCache.GetViewForViewModel(viewModel);
            CurrentView = view;
            return true;
        }
        return false;
    }
    
    //
    // public Control? GetMainView() =>
    //     (Application.Current?.ApplicationLifetime as ISingleViewApplicationLifetime)?.MainView;
    //
}
