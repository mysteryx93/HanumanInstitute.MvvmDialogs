using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.SaveFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        SaveFileCommand = new AsyncRelayCommand(SaveFileAsync);
    }

    public string? Path
    {
        get => path;
        private set => SetProperty(ref path, value);
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
            }
        };

        var result = await dialogService.ShowSaveFileDialogAsync(this, settings);
        Path = result;
    }
}
