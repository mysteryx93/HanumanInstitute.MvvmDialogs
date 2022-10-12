using System.Collections.Generic;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Manages navigation for single-view applications. 
/// </summary>
public class NavigationManager : ReactiveObject
{
    /// <summary>
    /// Navigation history contains only ViewModels to avoid keeping all constructed user controls in memory. The Views can be reconstructed from the ViewModels.
    /// </summary>
    private readonly List<INotifyPropertyChanged> _navigationHistory = new();
    private readonly ViewCache _viewCache = new();

    /// <summary>
    /// Initializes a new instance of the NavigationManager class.
    /// </summary>
    /// <param name="customNavigationRoot">If specified, a custom user control will be set as the main view instead of the default NavigationRoot.</param>
    public NavigationManager(UserControl? customNavigationRoot = null)
    {
        // Initialize the NavigationRoot as the main application view.
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime app)
        {
            app.MainView = customNavigationRoot ?? new NavigationRoot();
            app.MainView.DataContext = this;
        }
    }

    /// <summary>
    /// Gets or sets the view to display in the NavigationRoot control. NavigationRoot contains a binding to this property. 
    /// </summary>
    public UserControl? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }
    private UserControl? _currentView;

    /// <summary>
    /// Returns a View for specified ViewModel type. It will only work if such a View has been created before.
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <returns>The View instance, or null.</returns>
    public UserControl? GetViewForViewModel(INotifyPropertyChanged viewModel) =>
        _viewCache.GetViewForViewModel(viewModel.GetType());

    /// <summary>
    /// Shows specified view.
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <param name="viewType">The data type of the View.</param>
    public void Show(INotifyPropertyChanged viewModel, Type viewType)
    {
        var view = _viewCache.GetView(viewModel.GetType(), viewType);
        view.DataContext = viewModel;
        CurrentView = view;
    }

    /// <summary>
    /// Shows specified view and waits for a response.
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <param name="viewType">The data type of the View.</param>
    /// <returns>The dialog result.</returns>
    public Task<bool?> ShowDialogAsync(INotifyPropertyChanged viewModel, Type viewType)
    {
        var view = _viewCache.GetView(viewModel.GetType(), viewType);
        view.DataContext = viewModel;
        CurrentView = view;

        return Task.FromResult((bool?)true);
    }

    /// <summary>
    /// Closes specified view and shows the previous one. 
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <param name="viewType">The data type of the View.</param>
    public void Close(INotifyPropertyChanged viewModel, Type viewType)
    {
        
    }
    
    //
    // public Control? GetMainView() =>
    //     (Application.Current?.ApplicationLifetime as ISingleViewApplicationLifetime)?.MainView;
    //
}
