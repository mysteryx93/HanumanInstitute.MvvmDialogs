using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using IOPath = System.IO.Path;

namespace Demo.Avalonia.CustomOpenFolderDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    private string? _path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        BrowseFolderCommand = ReactiveCommand.Create(OpenFolderAsync);
    }

    public string? Path
    {
        get => _path;
        private set => this.RaiseAndSetIfChanged(ref _path, value);
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
        Path = result?.Path?.ToString();
    }
}
