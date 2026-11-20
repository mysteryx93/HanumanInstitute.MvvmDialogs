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
    /// Attaches the navigation host. Replacing it closes the previous navigation stack.
    /// </summary>
    /// <param name="customNavigationRoot">If specified, a custom user control will be set as the main view instead of the default NavigationRoot.</param>
    void Launch(Control? customNavigationRoot = null);
    /// <summary>
    /// Gets or sets the current view. A custom navigation root can bind to this property.
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
    /// <exception cref="OperationCanceledException">The owner or navigation host closed before the dialog.</exception>
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
    /// <returns>Whether activation succeeded.</returns>
    bool Activate(INotifyPropertyChanged viewModel);

    /// <summary>
    /// Gets whether a page or a framework dialog can handle Back.
    /// The default implementation checks whether the history contains more than one page.
    /// Override this property if a framework dialog can handle Back.
    /// </summary>
    bool CanGoBack => History.Count > 1;

    /// <summary>
    /// Goes back one page or dismisses the framework dialog that handles Back.
    /// The root page stays in place. The default implementation requests a close and immediately checks whether the current page changed.
    /// Override this method to await deferred close decisions or dismiss framework dialogs.
    /// </summary>
    /// <returns>True when a page left or a dialog dismissal started.</returns>
    Task<bool> GoBackAsync()
    {
        var current = CurrentViewModel;
        if (!CanGoBack || current == null) { return Task.FromResult(false); }

        Close(current);
        return Task.FromResult(!ReferenceEquals(CurrentViewModel, current));
    }
}
