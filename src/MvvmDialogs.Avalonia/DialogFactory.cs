using System.Collections.Generic;
using System.IO;
using System.Linq;
using HanumanInstitute.MvvmDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Handles OpenFileDialog, SaveFileDialog and OpenFolderDialog for Avalonia.
/// </summary>
public class DialogFactory : DialogFactoryBase
{
    private readonly IFrameworkDialogsApi _api;
    private readonly IPathInfoFactory _pathInfo;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogFactory(IDialogFactory? chain = null)
        : this(chain, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia framework dialogs.</param>
    /// <param name="pathInfo">Provides information about files and directories.</param>
    internal DialogFactory(IDialogFactory? chain, IFrameworkDialogsApi? api, IPathInfoFactory? pathInfo)
        : base(chain)
    {
        _api = api ?? new FrameworkDialogsApi();
        _pathInfo = pathInfo ?? new PathInfoFactory();
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            OpenFileDialogSettings s => await ShowOpenFileDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            SaveFileDialogSettings s => await ShowSaveFileDialogAsync(owner, s, appSettings).ConfigureAwait(true),
            _ => await base.ShowDialogAsync(owner, settings, appSettings).ConfigureAwait(true)
        };

    private async Task<string?> ShowOpenFolderDialogAsync(WindowWrapper owner, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new OpenFolderApiSettings()
        {
            Title = settings.Title,
            Directory = settings.InitialDirectory
            // d.ShowNewFolderButton = Settings.ShowNewFolderButton;
        };

        return await _api.ShowOpenFolderDialogAsync(owner.Ref, apiSettings).ConfigureAwait(true);
    }

    private async Task<string[]> ShowOpenFileDialogAsync(WindowWrapper owner, OpenFileDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new OpenFileApiSettings()
        {
            AllowMultiple = settings.AllowMultiple ?? false
            // d.ShowReadOnly = Settings.ShowReadOnly;
            // d.ReadOnlyChecked = Settings.ReadOnlyChecked;
        };
        AddSharedSettings(apiSettings, settings);

        string[] result;
        bool resultValid;
        do
        {
            resultValid = true;
            result = await _api.ShowOpenFileDialogAsync(owner.Ref, apiSettings).ConfigureAwait(true) ?? Array.Empty<string>();
            for (var i = 0; i < result.Length; i++)
            {
                // Work around the limitation that async method cannot return ref. We may add DefaultExtension to the result.
                var refItem = new RefClass<string>(result[i]);
                resultValid = await CheckResultAsync(owner, refItem, settings, appSettings).ConfigureAwait(true);
                result[i] = refItem.Value;
                if (!resultValid)
                {
                    apiSettings.InitialFileName = result[i];
                    break;
                }
            }
        } while (!resultValid);

        return result;
    }

    private async Task<string?> ShowSaveFileDialogAsync(WindowWrapper owner, SaveFileDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new SaveFileApiSettings()
        {
            DefaultExtension = settings.DefaultExtension
        };
        AddSharedSettings(apiSettings, settings);

        string? result;
        bool resultValid;
        do
        {
            resultValid = true;
            result = await _api.ShowSaveFileDialogAsync(owner.Ref, apiSettings).ConfigureAwait(true);
            if (result != null)
            {
                // Work around the limitation that async method cannot return ref. We may add DefaultExtension to the result.
                var refItem = new RefClass<string>(result);
                resultValid = await CheckResultAsync(owner, refItem, settings, appSettings).ConfigureAwait(true);
                result = refItem.Value;
                if (!resultValid)
                {
                    apiSettings.InitialFileName = result;
                    apiSettings.Directory = Path.GetDirectoryName(result);
                }
            }
        } while (!resultValid);

        return result;
    }

    private void AddSharedSettings(FileApiSettings d, FileDialogSettings s)
    {
        // d.DereferenceLinks = s.DereferenceLinks;
        d.Directory = s.InitialDirectory;
        d.InitialFileName = s.InitialFile;
        d.Filters = SyncFilters(s.Filters);
        d.Title = s.Title;
    }

    private static List<FileDialogFilter> SyncFilters(List<FileFilter> filters) =>
        filters.Select(
            x => new FileDialogFilter()
            {
                Name = x.NameToString(x.ExtensionsToString()),
                Extensions = x.Extensions.Select(y => y.TrimStart('.')).ToList()
            }).ToList();

    private async Task<bool> CheckResultAsync(WindowWrapper owner, RefClass<string> value, FileDialogSettings settings, AppDialogSettings appSettings)
    {
        var fileInfo = _pathInfo.GetFileInfo(value.Value);

        // DefaultExtension.
        if (!string.IsNullOrEmpty(settings.DefaultExtension) && !fileInfo.Exists && !value.Value.Contains('.'))
        {
            value.Value += "." + settings.DefaultExtension.TrimStart('.');
            fileInfo = _pathInfo.GetFileInfo(value.Value);
        }

        // CheckFileExists.
        if (settings.CheckFileExists && !fileInfo.Exists)
        {
            var msgSettings = new MessageBoxSettings()
            {
                Title = "File Not Found",
                Text = "Selected file does not exist:" + Environment.NewLine + value.Value,
                Icon = MessageBoxImage.Exclamation
            };
            await ChainTop.ShowDialogAsync(owner, msgSettings, appSettings).ConfigureAwait(true);
            return false;
        }

        // CheckPathExists.
        var path = Path.GetDirectoryName(value.Value)!;
        var pathInfo = _pathInfo.GetDirectoryInfo(path);
        if (settings.CheckPathExists && !pathInfo.Exists)
        {
            var msgSettings = new MessageBoxSettings()
            {
                Title = "Folder Not Found",
                Text = "Selected folder does not exist." + Environment.NewLine + path,
                Icon = MessageBoxImage.Exclamation
            };
            await ChainTop.ShowDialogAsync(owner, msgSettings, appSettings).ConfigureAwait(true);
            return false;
        }

        if (settings is SaveFileDialogSettings saveSettings)
        {
            // CreatePrompt.
            if (saveSettings.CreatePrompt && !fileInfo.Exists)
            {
                var msgSettings = new MessageBoxSettings()
                {
                    Title = "Create Confirmation",
                    Text = "File doesn't exist, do you want to create it?" + Environment.NewLine + value.Value,
                    Button = MessageBoxButton.YesNo,
                    DefaultValue = true
                };
                var result = (bool?)await ChainTop.ShowDialogAsync(owner, msgSettings, appSettings).ConfigureAwait(true);
                if (result != true)
                {
                    return false;
                }
            }

            // OverwritePrompt.
            if (saveSettings.OverwritePrompt && fileInfo.Exists)
            {
                var msgSettings = new MessageBoxSettings()
                {
                    Title = "Overwrite Confirmation",
                    Text = "File already exists, do you want to overwrite it?" + Environment.NewLine + value.Value,
                    Button = MessageBoxButton.YesNo,
                    DefaultValue = true
                };
                var result = (bool?)await ChainTop.ShowDialogAsync(owner, msgSettings, appSettings).ConfigureAwait(true);
                if (result != true)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Works around the limitation that async method cannot return ref parameter. Classes are by reference. 
    /// </summary>
    /// <typeparam name="T">The data type to pass by reference.</typeparam>
    private class RefClass<T>
    {
        public RefClass(T value) => Value = value;

        public T Value { get; set; }
    }
}
