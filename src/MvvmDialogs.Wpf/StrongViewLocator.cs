
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
        /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
        /// <typeparam name="TView">The view type to associate with the view model.</typeparam>
        public void Register<TViewModel, TView>()
            where TViewModel : INotifyPropertyChanged
            where TView : Control, new() =>
            Register<TViewModel>(
                new ViewDefinition(typeof(TView), () => new TView()));
    }
}
