using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;

namespace Demo.Avalonia.OpenFileDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public ObservableCollection<string> Paths { get; private set; } = new();

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        OpenFileCommand = ReactiveCommand.Create(OpenFileAsync);
        OpenFilesCommand = ReactiveCommand.Create(OpenFilesAsync);
    }

    public ICommand OpenFileCommand { get; }
    public ICommand OpenFilesCommand { get; }

    private async Task OpenFileAsync()
    {
        var settings = GetSettings(false);
        var result = await _dialogService.ShowOpenFileDialogAsync(this, settings);
        Paths.Clear();
        if (result?.Path != null)
        {
            Paths.Add(result.Path.LocalPath);
        }
    }

    private async Task OpenFilesAsync()
    {
        var settings = GetSettings(true);
        var result = await _dialogService.ShowOpenFilesDialogAsync(this, settings);
        Paths.Clear();
        foreach (var item in result)
        {
            Paths.Add(item?.Path?.LocalPath ?? string.Empty);
        }
    }

    private OpenFileDialogSettings GetSettings(bool multiple) => new()
    {
        Title = multiple ? "Open multiple files" : "Open single file",
        SuggestedStartLocation = new DesktopDialogStorageFolder(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!),
        Filters = new List<FileFilter>()
        {
            new(
                "Text Documents",
                new[]
                {
                    "txt", "md"
                }),
            new(
                "Binaries",
                new[]
                {
                    ".exe", ".dll"
                }),
            new("All Files", "*")
        }
    };
}
