using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
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

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFolder = new RelayCommand(() => OpenFolderImpl(SetOwner ? this : null));
        OpenFolderAsync = new AsyncRelayCommand(() => OpenFolderImplAsync(SetOwner ? this : null));
    }

    public string? Path
    {
        get => path;
        private set => SetProperty(ref path, value);
    }
    private string? path;

    public bool SetOwner
    {
        get => setOwner;
        set => SetProperty(ref setOwner, value);
    }
    private bool setOwner = true;


    public ICommand OpenFolder { get; }
    public ICommand OpenFolderAsync { get; }

    private void OpenFolderImpl(INotifyPropertyChanged? owner)
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = dialogService.ShowOpenFolderDialog(owner, settings);
        Path = result;
    }

    private async Task OpenFolderImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = await dialogService.ShowOpenFolderDialogAsync(owner, settings);
        Path = result;
    }
}
