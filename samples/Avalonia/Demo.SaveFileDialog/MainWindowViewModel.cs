using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.SaveFileDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
    }

    public string? Path
    {
        get => path;
        private set => this.RaiseAndSetIfChanged(ref path, value);
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
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            },
            CreatePrompt = true,
            DefaultExtension = ".mp3"
        };

        var result = await dialogService.ShowSaveFileDialogAsync(this, settings);
        Path = result;
    }
}
