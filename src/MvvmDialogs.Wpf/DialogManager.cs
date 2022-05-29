using System.Linq;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;
using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs.Wpf
{
    /// <summary>
    /// DialogManager that supports extra sync methods to show dialogs.
    /// </summary>
    public class DialogManager : DialogManagerBase<Window>, IDialogManagerSync
    {
        /// <inheritdoc />
        public DialogManager(IViewLocator? viewLocator = null, IFrameworkDialogFactory? frameworkDialogFactory = null,
            ILogger<DialogManager>? logger = null) :
            base(viewLocator ?? new ViewLocatorBase(),
                frameworkDialogFactory ?? new FrameworkDialogFactory(), logger)
        {
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
        public TResult ShowFrameworkDialog<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings, Func<TResult, string>? resultToString = null)
            where TSettings : DialogSettingsBase
        {
            Logger?.LogInformation("Dialog: {Dialog}; Title: {Title}", settings.GetType().Name, settings.Title);

            var dialog = FrameworkDialogFactory.Create<TSettings, TResult>(settings, appSettings);
            var owner = FindWindowByViewModel(ownerViewModel) ?? throw new ArgumentException($"No view found with specified ownerViewModel of type {ownerViewModel.GetType()}.");
            var result = dialog.AsSync().ShowDialog(owner);

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
    }
}
