using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.CustomOpenFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFileCommand = new RelayCommand(OpenFile);
    }

    public string? Path
    {
        get => path;
        private set => SetProperty(ref path, value);
    }

    public ICommand OpenFileCommand { get; }

    private void OpenFile()
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

        var result = dialogService.ShowOpenFileDialog(this, settings);
        Path = result;
    }
}
