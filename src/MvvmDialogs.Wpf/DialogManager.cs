using System.Linq;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

namespace HanumanInstitute.MvvmDialogs.Wpf
{
    /// <summary>
    /// DialogManager that supports extra sync methods to show dialogs.
    /// </summary>
    public class DialogManager : DialogManagerBase<Window>, IDialogManagerSync
    {
        /// <inheritdoc />
        public DialogManager(IFrameworkDialogFactory? frameworkDialogFactory = null) :
            base(frameworkDialogFactory ?? new FrameworkDialogFactory())
        {
        }

        /// <inheritdoc />
        public virtual void ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, object? view)
        {
            var dialog = CreateDialog(ownerViewModel, viewModel, view);
            dialog.AsSync().ShowDialog();
        }

        /// <inheritdoc />
        public TResult ShowFrameworkDialog<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings)
            where TSettings : DialogSettingsBase
        {
            var dialog = FrameworkDialogFactory.Create<TSettings, TResult>(settings, appSettings);
            var owner = FindWindowByViewModel(ownerViewModel) ?? throw new ArgumentException($"No view found with specified ownerViewModel of type {ownerViewModel.GetType()}.");
            return dialog.AsSync().ShowDialog(owner);
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
