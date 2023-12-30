using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Strongly-typed View Locator that does not rely on reflection.
/// </summary>
public abstract class StrongViewLocatorBase : IViewLocator
{
    /// <summary>
    /// The list of registered ViewModel-View combinations.
    /// </summary>
    protected readonly Dictionary<Type, ViewDefinition> Registrations = new();

    /// <summary>
    /// Registers specified views as being associated with specified view model type.
    /// If multiple views are registered, they can be selected based on factors such as platform, such as Desktop vs Mobile vs Web. 
    /// </summary>
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
    protected void Register<TViewModel>(ViewDefinition viewDef)
        where TViewModel : INotifyPropertyChanged
    {
        Registrations.Add(typeof(TViewModel), viewDef);
    }

    /// <inheritdoc />
    public virtual ViewDefinition Locate(object viewModel)
    {
        if (Registrations.TryGetValue(viewModel.GetType(), out var view))
        {
            return view;
        }
        else
        {
            var message = $"No view was registered for view model {viewModel.GetType().FullName}.";
            const string ErrorInfo = "This project uses a StrongViewLocator, which requires manually registering all ViewModel-View combinations.";
            throw new TypeLoadException(message + Environment.NewLine + ErrorInfo);
        }
    }

    /// <inheritdoc />
    public virtual object Create(object viewModel) =>
        Locate(viewModel).Create();
}
