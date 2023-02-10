using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.Wpf.CustomOpenFolderDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private string? _path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        BrowseFolderCommand = new AsyncRelayCommand(OpenFolderAsync);
    }

    public string? Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }

    public ICommand BrowseFolderCommand { get; }

    private async Task OpenFolderAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = await _dialogService.ShowOpenFolderDialogAsync(this, settings);
        Path = result?.LocalPath;
    }
}
