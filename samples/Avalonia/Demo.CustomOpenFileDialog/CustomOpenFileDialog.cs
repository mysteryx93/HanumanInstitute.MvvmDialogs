using System;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.WinForms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Avalonia.Win32;

namespace Demo.CustomOpenFileDialog;

public class CustomOpenFileDialog : IFrameworkDialog<string[]>
{
    private readonly OpenFileDialogSettings settings;
    private readonly VistaOpenFileDialog openFileDialog;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomOpenFileDialog"/> class.
    /// </summary>
    /// <param name="settings">The settings for the open file dialog.</param>
    public CustomOpenFileDialog(OpenFileDialogSettings settings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        openFileDialog = new VistaOpenFileDialog
        {
            AddExtension = true,
            CheckFileExists = settings.CheckFileExists,
            CheckPathExists = settings.CheckPathExists,
            DefaultExt = settings.DefaultExtension,
            Filter = SyncFilters(settings.Filters),
            InitialDirectory = settings.InitialDirectory,
            FileName = settings.InitialFile,
            Multiselect = settings.AllowMultiple ?? false,
            Title = settings.Title
        };
    }

    /// <summary>
    /// Encodes the list of filters in the Win32 API format:
    /// "Image Files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
    /// </summary>
    /// <param name="filters">The list of filters to encode.</param>
    /// <returns>A string representation of the list compatible with Win32 API.</returns>
    private static string SyncFilters(List<FileFilter> filters)
    {
        var result = new StringBuilder();
        foreach (var item in filters)
        {
            // Add separator.
            if (result.Length > 0)
            {
                result.Append('|');
            }

            // Get all extensions as a string.
            var extDesc = item.ExtensionsToString();
            // Get name including extensions.
            var name = item.NameToString(extDesc);
            // Add name+extensions for display.
            result.Append(name);
            // Add extensions again for the API.
            result.Append("|");
            result.Append(extDesc);
        }
        return result.ToString();
    }

    /// <summary>
    /// Opens a open file dialog with specified owner.
    /// </summary>
    /// <param name="owner">
    /// Handle to the window that owns the dialog.
    /// </param>
    /// <returns>
    /// true if user clicks the OK button; otherwise false.
    /// </returns>
    public async Task<string[]> ShowDialogAsync(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var window = owner.AsWrapper().Ref;
        var platformImpl = (WindowImpl)window.PlatformImpl;
        var handle = platformImpl.Handle.Handle;

        var result = await window.RunUiAsync(() => openFileDialog.ShowDialog(handle));

        return result == true ? openFileDialog.FileNames : Array.Empty<string>();
    }
}
