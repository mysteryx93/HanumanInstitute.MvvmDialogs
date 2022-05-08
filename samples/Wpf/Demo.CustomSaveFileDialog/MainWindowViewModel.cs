using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.CustomSaveFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        SaveFileCommand = new RelayCommand(SaveFile);
    }

    public string? Path
    {
        get => path;
        private set => SetProperty(ref path, value);
    }

    public ICommand SaveFileCommand { get; }

    private void SaveFile()
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

        var result = dialogService.ShowSaveFileDialog(this, settings);
        Path = result;
    }
}
