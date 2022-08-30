using System.Reflection;
using Avalonia.Controls.Templates;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Base implementation of Avalonia ViewLocator. Override GetViewName to customize paths.
/// </summary>
public class ViewLocatorBase : IDataTemplate, IViewLocator
{
    /// <summary>
    /// Returns the view type name for specified view model type. By default, it replaces 'ViewModel' with 'View'.
    /// </summary>
    /// <param name="viewModel">The view model to get the view type for.</param>
    /// <returns>The view type name.</returns>
    protected virtual string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "View");

    /// <inheritdoc />
    public virtual IControl Build(object data)
    {
        try
        {
            return (IControl)Locate(data);
        }
        catch (Exception)
        {
            return new TextBlock
            {
                Text = "Not Found: " + GetViewName(data)
            };
        }
    }

    /// <summary>
    /// Creates a view based on the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to create a view for.</param>
    public virtual object Locate(object viewModel)
    {
        var name = GetViewName(viewModel);
        // var type = Type.GetType(name, x => Assembly.GetEntryAssembly(), null, false);
        var type = Assembly.GetEntryAssembly()?.GetType(name);

        var view = type != null ? Activator.CreateInstance(type)! : null;

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (view is null || (view is not IControl && view is not Window && view is not IView))
        {
            var message = $"Dialog view of type {name} for view model {viewModel.GetType().FullName} is missing.";
            const string errorInfo = "Avalonia project template includes ViewLocator in the project base. " +
                                     "You can customize it to map your view models to your views.";
            throw new TypeLoadException(message + Environment.NewLine + errorInfo);
        }
        return view;
    }

    /// <inheritdoc />
    public virtual bool Match(object data) => data is ReactiveObject;
}
