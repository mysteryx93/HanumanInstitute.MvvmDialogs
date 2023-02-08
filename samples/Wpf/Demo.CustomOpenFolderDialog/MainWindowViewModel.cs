using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.CustomOpenFolderDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        BrowseFolderCommand = new AsyncRelayCommand(OpenFolderAsync);
    }

    public string? Path
    {
        get => path;
        private set => SetProperty(ref path, value);
    }

    public ICommand BrowseFolderCommand { get; }

    private async Task OpenFolderAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = await dialogService.ShowOpenFolderDialogAsync(this, settings);
        Path = result?.Path?.ToString();
    }
}
