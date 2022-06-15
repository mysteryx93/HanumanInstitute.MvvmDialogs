using System.Linq;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs.Wpf
{
    /// <summary>
    /// DialogManager that supports extra sync methods to show dialogs.
    /// </summary>
    public class DialogManager : DialogManagerBase<Window>, IDialogManagerSync
    {
        private readonly Dispatcher _dispatcher;

        /// <inheritdoc />
        public DialogManager(IViewLocator? viewLocator = null, IDialogFactory? dialogFactory = null,
            ILogger<DialogManager>? logger = null, Dispatcher? dispatcher = null) :
            base(viewLocator ?? new ViewLocatorBase(),
                dialogFactory ?? new DialogFactory(), logger)
        {
            _dispatcher = dispatcher ?? Application.Current.Dispatcher;
        }

        /// <inheritdoc />
        public virtual void ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
        {
            var view = ViewLocator.Locate(viewModel);
            Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", view?.GetType(), viewModel.GetType(), ownerViewModel.GetType());

            var dialog = CreateDialog(ownerViewModel, viewModel, view);
            dialog.AsSync().ShowDialog();

            Logger?.LogInformation("View: {View}; Result: {Result}", view?.GetType(), viewModel.DialogResult);
        }

        /// <inheritdoc />
        public object? ShowFrameworkDialog<TSettings>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings, Func<object?, string>? resultToString = null)
            where TSettings : DialogSettingsBase
        {
            Logger?.LogInformation("Dialog: {Dialog}; Title: {Title}", settings.GetType().Name, settings.Title);

            var owner = FindWindowByViewModel(ownerViewModel) ?? throw new ArgumentException($"No view found with specified ownerViewModel of type {ownerViewModel.GetType()}.");
            var result = DialogFactory.AsSync().ShowDialog(owner, settings, appSettings);

            Logger?.LogInformation("Dialog: {Dialog}; Result: {Result}", settings?.GetType().Name, resultToString != null ? resultToString(result) : result?.ToString());
            return result;
        }

        /// <inheritdoc />
        protected override IWindow CreateWrapper(Window window) => window.AsWrapper();

        private static IEnumerable<Window> Windows =>
            Application.Current.Windows.Cast<Window>();

        /// <inheritdoc />
        public override IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel) =>
            Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();

        /// <inheritdoc />
        protected override void Dispatch(Action action) => _dispatcher.Invoke(action, DispatcherPriority.Render);

        /// <inheritdoc />
        protected override Task<T> DispatchAsync<T>(Func<T> action) => _dispatcher.InvokeAsync(action, DispatcherPriority.Render).Task;
    }
}
