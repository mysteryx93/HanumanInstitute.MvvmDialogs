using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using IOPath = System.IO.Path;

namespace Demo.OpenFolderDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    private string? path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFolderCommand = ReactiveCommand.Create(OpenFolderAsync);
    }

    public string? Path
    {
        get => path;
        private set => this.RaiseAndSetIfChanged(ref path, value);
    }

    public ICommand OpenFolderCommand { get; }

    private async Task OpenFolderAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = await dialogService.ShowOpenFolderDialogAsync(this, settings);
        Path = result;
    }
}
