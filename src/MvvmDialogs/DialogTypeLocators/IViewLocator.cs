namespace HanumanInstitute.MvvmDialogs.DialogTypeLocators;

/// <summary>
/// Interface responsible for finding a dialog type matching a view model.
/// </summary>
public interface IViewLocator
{
    /// <summary>
    /// Creates a view based on the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to create a view for.</param>
    object? Locate(object viewModel);
}
