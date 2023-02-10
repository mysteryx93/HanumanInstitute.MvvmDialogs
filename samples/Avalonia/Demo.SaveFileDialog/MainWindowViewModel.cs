using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

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
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", new[] { "txt", "md" }),
                new FileFilter("All Files", "*")
            },
            DefaultExtension = "mp3"
        };

        var result = await _dialogService.ShowSaveFileDialogAsync(this, settings);
        Path = result?.Path?.LocalPath;
    }
}
