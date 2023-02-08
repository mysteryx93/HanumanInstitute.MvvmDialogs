using System.Reflection;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Base implementation of Avalonia ViewLocator. Override GetViewName to customize paths.
/// </summary>
public class ViewLocatorBase : IViewLocator
{
    /// <summary>
    /// Returns the view type name for specified view model type. By default, it replaces 'ViewModel' with 'View'.
    /// </summary>
    /// <param name="viewModel">The view model to get the view type for.</param>
    /// <returns>The view type name.</returns>
    protected virtual string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "View");

    /// <summary>
    /// Creates a view based on the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to create a view for.</param>
    public virtual Type Locate(object viewModel)
    {
        var name = GetViewName(viewModel);
        //var type = Assembly.GetEntryAssembly()?.GetType(name);
        var viewType = Assembly.GetAssembly(viewModel.GetType())?.GetType(name);

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (viewType is null || (!typeof(Window).IsAssignableFrom(viewType) && !typeof(IView).IsAssignableFrom(viewType)))
        {
            var message = $"Dialog view of type {name} for view model {viewModel.GetType().FullName} is missing.";
            const string errorInfo = "You can create a ViewLocator class in the project base to map your " +
                                     "view models to your views. See online documentation for more info.";
            throw new TypeLoadException(message + Environment.NewLine + errorInfo);
        }
        return viewType;
    }

    /// <inheritdoc />
    public virtual object Create(object viewModel) =>
        Activator.CreateInstance(Locate(viewModel))!;
}
