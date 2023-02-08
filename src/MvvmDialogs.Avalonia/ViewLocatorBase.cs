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
    /// <summary>
    /// Gets or sets whether to force single-page navigation. Setting this to true can allow running in single-page mode on desktop.
    /// </summary>
    public bool ForceSinglePageNavigation { get; set; }

    /// <summary>
    /// Returns the view type name for specified view model type. By default, it replaces 'ViewModel' with 'View'.
    /// </summary>
    /// <param name="viewModel">The view model to get the view type for.</param>
    /// <returns>The view type name.</returns>
    protected virtual string GetViewName(object viewModel)
    {
        const string ViewModel = "ViewModel";
        var result = viewModel.GetType().FullName!.Replace(".ViewModels.", ".Views.");
        if (result.EndsWith(ViewModel))
        {
            result = result.Remove(result.Length - ViewModel.Length) + (UseSinglePageNavigation ? "View" : "Window");
        }
        return result;
    }

    /// <inheritdoc />
    public virtual Control Build(object? data)
    {
        try
        {
            return (Control)Create(data!);
        }
        catch (Exception)
        {
            return new TextBlock
            {
                Text = "Not Found: " + GetViewName(data!)
            };
        }
    }

    /// <inheritdoc />
    public virtual Type Locate(object viewModel)
    {
        var name = GetViewName(viewModel);
        // var type = Type.GetType(name, x => Assembly.GetEntryAssembly(), null, false);
        var viewType = Assembly.GetAssembly(viewModel.GetType())?.GetType(name);

        if (viewType is null || (!typeof(Control).IsAssignableFrom(viewType) && !typeof(Window).IsAssignableFrom(viewType) && !typeof(IView).IsAssignableFrom(viewType)))
        {
            var message = $"Dialog view of type {name} for view model {viewModel.GetType().FullName} is missing.";
            const string ErrorInfo = "Avalonia project template includes ViewLocator in the project base. " +
                                     "You can customize it to map your view models to your views.";
            throw new TypeLoadException(message + Environment.NewLine + ErrorInfo);
        }
        return viewType;
    }

    /// <inheritdoc />
    public virtual object Create(object viewModel) =>
        Activator.CreateInstance(Locate(viewModel))!;

    /// <inheritdoc />
    public virtual bool Match(object? data) => data is ReactiveObject;

    /// <summary>
    /// Gets whether the application runs in single-page navigation mode.
    /// </summary>
    public bool UseSinglePageNavigation => Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime || ForceSinglePageNavigation;
}
