using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.OpenFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    public ObservableCollection<string> Paths { get; private set; } = new();

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFileCommand = new AsyncRelayCommand(OpenFileAsync);
        OpenFilesCommand = new AsyncRelayCommand(OpenFilesAsync);
    }

    public ICommand OpenFileCommand { get; }
    public ICommand OpenFilesCommand { get; }

    private async Task OpenFileAsync()
    {
        var settings = GetSettings(false);
        var result = await dialogService.ShowOpenFileDialogAsync(this, settings);
        Paths.Clear();
        if (result != null)
        {
            Paths.Add(result);
        }
    }

    private async Task OpenFilesAsync()
    {
        var settings = GetSettings(true);
        var result = await dialogService.ShowOpenFilesDialogAsync(this, settings);
        Paths.Clear();
        foreach (var item in result)
        {
            Paths.Add(item);
        }
    }

    private OpenFileDialogSettings GetSettings(bool multiple) => new OpenFileDialogSettings
    {
        Title = multiple ? "Open multiple files" : "Open single file",
        InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            }
    };
}
