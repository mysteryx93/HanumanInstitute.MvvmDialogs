using System.Reflection;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Base implementation of Avalonia ViewLocator. Override GetViewName to customize paths.
/// </summary>
public class ViewLocatorBase : IDataTemplate, IViewLocator
{
    /// <inheritdoc />
    public bool? SinglePageNavigation { get; set; }
    
    /// <summary>
    /// Returns the view type name for specified view model type. By default, it replaces 'ViewModel' with 'View'.
    /// </summary>
    /// <param name="viewModel">The view model to get the view type for.</param>
    /// <returns>The view type name.</returns>
    protected virtual string GetViewName(object viewModel, bool singlePageNavigation)
    {
        const string ViewModel = "ViewModel";
        var result = viewModel.GetType().FullName!.Replace(".ViewModels.", ".Views.");
        if (result.EndsWith(ViewModel))
        {
            result = result.Remove(result.Length - ViewModel.Length) + (singlePageNavigation ? "View" : "Window");
        }
        return result;
    }

    /// <inheritdoc />
    public virtual IControl Build(object? data)
    {
        try
        {
            return (IControl)Create(data!);
        }
        catch (Exception)
        {
            return new TextBlock
            {
                Text = "Not Found: " + GetViewName(data!, UseNavigation)
            };
        }
    }

    /// <inheritdoc />
    public virtual Type Locate(object viewModel)
    {
        var name = GetViewName(viewModel, UseNavigation);
        // var type = Type.GetType(name, x => Assembly.GetEntryAssembly(), null, false);
        var viewType = Assembly.GetAssembly(viewModel.GetType())?.GetType(name);

        if (viewType is null || (!typeof(IControl).IsAssignableFrom(viewType) && !typeof(Window).IsAssignableFrom(viewType) && !typeof(IView).IsAssignableFrom(viewType)))
        {
            var message = $"Dialog view of type {name} for view model {viewModel.GetType().FullName} is missing.";
            const string errorInfo = "Avalonia project template includes ViewLocator in the project base. " +
                                     "You can customize it to map your view models to your views.";
            throw new TypeLoadException(message + Environment.NewLine + errorInfo);
        }
        return viewType;
    }

    /// <inheritdoc />
    public virtual object Create(object viewModel) =>
        Activator.CreateInstance(Locate(viewModel))!;

    /// <inheritdoc />
    public virtual bool Match(object? data) => data is ReactiveObject;

    private bool UseNavigation => Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime || SinglePageNavigation == true;
}
