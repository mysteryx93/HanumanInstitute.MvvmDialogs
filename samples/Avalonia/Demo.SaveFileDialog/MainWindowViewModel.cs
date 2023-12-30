using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace Demo.Avalonia.SaveFileDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    private string? _path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
    }

    public string? Path
    {
        get => _path;
        private set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public ICommand SaveFileCommand { get; }

    private async Task SaveFileAsync()
    {
        var settings = new SaveFileDialogSettings
        {
            Title = "This is the title",
            SuggestedStartLocation = new DesktopDialogStorageFolder(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!),
            SuggestedFileName = "DefaultName",
            Filters = new List<FileFilter>()
            {
                new("Text Documents", new[] { "txt", "md" }),
                new("MP3", new[] { "mp3" }),
                new("All Files", "*")
            },
            DefaultExtension = "mp3"
        };

        var result = await _dialogService.ShowSaveFileDialogAsync(this, settings);
        Path = result?.Path?.LocalPath;
    }
}
