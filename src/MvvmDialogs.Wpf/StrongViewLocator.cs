
namespace HanumanInstitute.MvvmDialogs.Wpf
{
    /// <summary>
    /// Strongly-typed View Locator that does not rely on reflection.
    /// </summary>
    public class StrongViewLocator : StrongViewLocatorBase
    {
        /// <summary>
        /// Registers specified views as being associated with specified view model type.
        /// </summary>
        /// <param name="viewDef">The view definition including its type and how to create one.</param>
        /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
        public StrongViewLocator Register<TViewModel>(ViewDefinition viewDef)
            where TViewModel : INotifyPropertyChanged
        {
            RegisterBase<TViewModel>(viewDef);
            return this;
        }
    }
}
