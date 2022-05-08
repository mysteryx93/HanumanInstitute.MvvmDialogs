using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.OpenFolderDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFolderCommand = new RelayCommand(OpenFolder);
    }

    public string? Path
    {
        get => path;
        private set => SetProperty(ref path, value);
    }

    public ICommand OpenFolderCommand { get; }

    private void OpenFolder()
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialPath = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = dialogService.ShowOpenFolderDialog(this, settings);
        Path = result;
    }
}
