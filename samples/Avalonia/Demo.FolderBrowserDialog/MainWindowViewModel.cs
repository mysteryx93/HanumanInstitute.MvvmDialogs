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
    private string path = string.Empty;
    public ICommand OpenFolderCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFolderCommand = ReactiveCommand.CreateFromTask(BrowseFolderAsync);
    }

    public string Path
    {
        get => path;
        private set => this.RaiseAndSetIfChanged(ref path, value, nameof(Path));
    }

    private async Task BrowseFolderAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a title",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        };

        var result = await dialogService.ShowOpenFolderDialogAsync(this, settings).ConfigureAwait(true);
        if (result != null)
        {
            Path = result;
        }
    }
}
