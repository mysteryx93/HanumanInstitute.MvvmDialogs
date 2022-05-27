using System.Text;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

internal abstract class FileDialogBase<TSettings, TResult> : FrameworkDialogBase<TSettings, TResult>
    where TSettings : FileDialogSettings
{
    internal FileDialogBase(IFrameworkDialogsApi api, IPathInfoFactory pathInfo, TSettings settings, AppDialogSettings appSettings)
        : base(settings, appSettings, pathInfo, api)
    {
    }

    /// <summary>
    /// Adds common file dialog settings to the settings class.
    /// </summary>
    /// <param name="d">The dialog settings class to add settings to.</param>
    protected void AddSharedSettings(FileApiSettings d)
    {
        var s = Settings;
        d.DefaultExt = s.DefaultExtension;
        d.AddExtension = !string.IsNullOrEmpty(s.DefaultExtension);
        d.CheckFileExists = s.CheckFileExists;
        d.CheckPathExists = s.CheckPathExists;
        d.InitialDirectory = s.InitialDirectory;
        d.FileName = s.InitialFile;
        d.DereferenceLinks = s.DereferenceLinks;
        d.Filter = SyncFilters(s.Filters);
        d.Title = s.Title;
        d.ShowHelp = s.HelpRequest != null;
        d.HelpRequest = s.HelpRequest;
    }

    /// <summary>
    /// Encodes the list of filters in the Win32 API format:
    /// "Image Files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
    /// </summary>
    /// <param name="filters">The list of filters to encode.</param>
    /// <returns>A string representation of the list compatible with Win32 API.</returns>
    private static string SyncFilters(List<FileFilter> filters)
    {
        StringBuilder result = new StringBuilder();
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
}
