using System;
using System.Threading.Tasks;
using System.Windows;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.Wpf;

namespace Demo.CustomFolderBrowserDialog
{
    public class CustomFolderBrowserDialog : IFrameworkDialog<string?>
    {
        private readonly OpenFolderDialogSettings settings;
        private readonly VistaFolderBrowserDialog folderBrowserDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserDialogWrapper"/> class.
        /// </summary>
        /// <param name="settings">The settings for the folder browser dialog.</param>
        public CustomFolderBrowserDialog(OpenFolderDialogSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

            folderBrowserDialog = new VistaFolderBrowserDialog
            {
                Description = settings.Title,
                SelectedPath = settings.InitialPath
            };
        }

        /// <summary>
        /// Opens a folder browser dialog with specified owner.
        /// </summary>
        /// <param name="owner">
        /// Handle to the window that owns the dialog.
        /// </param>
        /// <returns>
        /// true if user clicks the OK or YES button; otherwise false.
        /// </returns>
        public async Task<string?> ShowDialogAsync(IWindow owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            var window = (Window)owner;
            var result = await window.RunUiAsync(() => folderBrowserDialog.ShowDialog(window));

            return result == true ? folderBrowserDialog.SelectedPath : null;
        }
    }
}
