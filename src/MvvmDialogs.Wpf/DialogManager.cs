using System.Linq;
using System.Windows.Media;
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
        protected override bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <inheritdoc />
        public virtual void ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
        {
            var viewDef = ViewLocator.Locate(viewModel);
            Logger?.LogInformation("View: {View}; ViewModel: {ViewModel}; Owner: {OwnerViewModel}", viewDef.ViewType, viewModel.GetType(), ownerViewModel.GetType());

            var dialog = CreateDialog(viewModel, viewDef);
            dialog.AsSync().ShowDialog(FindViewByViewModelOrThrow(ownerViewModel)!);

            Logger?.LogInformation("View: {View}; Result: {Result}", viewDef.ViewType.GetType(), viewModel.DialogResult);
        }

        /// <inheritdoc />
        public object? ShowFrameworkDialog<TSettings>(INotifyPropertyChanged? ownerViewModel, TSettings settings, Func<object?, string>? resultToString = null)
            where TSettings : DialogSettingsBase
        {
            Logger?.LogInformation("Dialog: {Dialog}; Title: {Title}", settings.GetType().Name, settings.Title);

            IView? owner = null;
            if (ownerViewModel != null)
            {
                owner = FindViewByViewModelOrThrow(ownerViewModel);
            }
            var result = DialogFactory.AsSync().ShowDialog(owner, settings);

            Logger?.LogInformation("Dialog: {Dialog}; Result: {Result}", settings?.GetType().Name, resultToString != null ? resultToString(result) : result?.ToString());
            return result;
        }

        /// <inheritdoc />
        protected override IView CreateWrapper(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
        {
            var wrapper = new ViewWrapper();
            wrapper.Initialize(viewModel, viewDef);
            return wrapper;
        }

        /// <inheritdoc />
        protected override IView AsWrapper(Window view) => view.AsWrapper();

        private static IEnumerable<Window> Windows =>
            Application.Current.Windows.Cast<Window>();

        /// <inheritdoc />
        public override IView? FindViewByViewModel(INotifyPropertyChanged viewModel) =>
            Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();

        /// <inheritdoc />
        public override IView? GetMainWindow() =>
            Application.Current.MainWindow.AsWrapper();

        /// <inheritdoc />
        public override IView? GetDummyWindow()
        {
            var parent = new Window()
            {
                Height = 1,
                Width = 1,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = Brushes.Transparent
            };
            parent.Show();
            return parent.AsWrapper();
        }

        /// <inheritdoc />
        protected override void Dispatch(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.Invoke(action, DispatcherPriority.Render);
            }
        }

        /// <inheritdoc />
        protected override Task<T> DispatchAsync<T>(Func<T> action) =>
            _dispatcher.CheckAccess() ? Task.FromResult(action()) : _dispatcher.InvokeAsync(action, DispatcherPriority.Render).Task;
    }
}
