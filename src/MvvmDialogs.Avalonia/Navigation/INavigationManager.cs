using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Manages navigation for single-view applications. 
/// </summary>
public interface INavigationManager
{
    /// <summary>
    /// Returns the navigation history.
    /// </summary>
    public IReadOnlyList<INotifyPropertyChanged> History { get; }   
    /// <summary>
    /// Initializes a new instance of the NavigationManager class.
    /// </summary>
    /// <param name="customNavigationRoot">If specified, a custom user control will be set as the main view instead of the default NavigationRoot.</param>
    void Launch(Control? customNavigationRoot = null);
    /// <summary>
    /// Gets or sets the view to display in the NavigationRoot control. NavigationRoot contains a binding to this property. 
    /// </summary>
    UserControl? CurrentView { get; set; }
    /// <summary>
    /// Gets the view model of the current view.
    /// </summary>
    INotifyPropertyChanged? CurrentViewModel { get; }
    /// <summary>
    /// Returns a View for specified ViewModel type. It will only work if such a View has been created before.
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <returns>The View instance, or null.</returns>
    UserControl? GetViewForViewModel(INotifyPropertyChanged viewModel);
    /// <summary>
    /// Shows specified view.
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    void Show(INotifyPropertyChanged viewModel, ViewDefinition viewDef);
    /// <summary>
    /// Shows specified view and waits for a response.
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <returns>The dialog result.</returns>
    Task ShowDialogAsync(INotifyPropertyChanged viewModel, ViewDefinition viewDef, INotifyPropertyChanged ownerViewModel);
    /// <summary>
    /// Closes specified view and shows the previous one. 
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    void Close(INotifyPropertyChanged viewModel);
    /// <summary>
    /// Activates specified view, pumping it in front of the navigation history. 
    /// </summary>
    /// <param name="viewModel">The ViewModel to display in the View.</param>
    /// <returns>Whether a matching view from history has been activated.</returns>
    bool Activate(INotifyPropertyChanged viewModel);
}
