using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using IOPath = System.IO.Path;

namespace Demo.CustomOpenFileDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFileCommand = ReactiveCommand.Create(OpenFile);
    }

    public string? Path
    {
        get => path;
        private set => this.RaiseAndSetIfChanged(ref path, value);
    }

    public ICommand OpenFileCommand { get; }

    private async Task OpenFile()
    {
        var settings = new OpenFileDialogSettings
        {
            Title = "This Is The Title",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            }
        };

        var result = await dialogService.ShowOpenFileDialogAsync(this, settings);
        Path = result;
    }
}
