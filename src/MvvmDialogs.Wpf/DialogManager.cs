using System;
using System.ComponentModel;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

namespace HanumanInstitute.MvvmDialogs.Wpf
{
    /// <summary>
    /// DialogManager that supports extra sync methods to show dialogs.
    /// </summary>
    public class DialogManager : DialogManagerBase, IDialogManagerSync
    {
        /// <inheritdoc />
        public DialogManager(IFrameworkDialogFactory? frameworkDialogFactory = null, IDialogFactory? dialogFactory = null) :
            base(frameworkDialogFactory ?? new FrameworkDialogFactory(), dialogFactory ?? new ReflectionDialogFactory())
        {
        }

        /// <inheritdoc />
        public virtual void ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, Type dialogType)
        {
            var dialog = CreateDialog(ownerViewModel, viewModel, dialogType);
            dialog.AsSync().ShowDialog();
        }

        /// <inheritdoc />
        public TResult ShowFrameworkDialog<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings)
            where TSettings : DialogSettingsBase
        {
            var dialog = FrameworkDialogFactory.Create<TSettings, TResult>(settings, appSettings);
            return dialog.AsSync().ShowDialog(ViewRegistration.FindView(ownerViewModel));
        }
    }
}
