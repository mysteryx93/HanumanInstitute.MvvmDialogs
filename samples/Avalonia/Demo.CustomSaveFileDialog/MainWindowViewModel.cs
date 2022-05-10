using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using IOPath = System.IO.Path;

namespace Demo.CustomSaveFileDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        SaveFileCommand = ReactiveCommand.Create(SaveFile);
    }

    public string? Path
    {
        get => path;
        private set => this.RaiseAndSetIfChanged(ref path, value);
    }

    public ICommand SaveFileCommand { get; }

    private async Task SaveFile()
    {
        var settings = new SaveFileDialogSettings
        {
            Title = "This Is The Title",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            },
            CheckFileExists = false
        };

        var result = await dialogService.ShowSaveFileDialogAsync(this, settings);
        Path = result;
    }
}
