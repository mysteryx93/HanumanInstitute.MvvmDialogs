namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface responsible for finding a dialog type matching a view model.
/// </summary>
public interface IViewLocator
{
    /// <summary>
    /// Get the view type based on the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to get the view type for.</param>
    /// <exception cref="TypeLoadException">View not found for view model.</exception>
    ViewDefinition Locate(object viewModel);

    /// <summary>
    /// Creates a view based on the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to create a view for.</param>
    /// <exception cref="TypeLoadException">View not found for view model.</exception>
    object Create(object viewModel);
}
