using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.SaveFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        SaveFile = new RelayCommand(() => SaveFileImpl(SetOwner ? this : null));
        SaveFileAsync = new AsyncRelayCommand(() => SaveFileImplAsync(SetOwner ? this : null));
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

    public ICommand SaveFile { get; }
    public ICommand SaveFileAsync { get; }

    private void SaveFileImpl(INotifyPropertyChanged? owner)
    {
        var settings = new SaveFileDialogSettings
        {
            Title = "This is the title",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            }
        };

        var result = dialogService.ShowSaveFileDialog(owner, settings);
        Path = result;
    }

    private async Task SaveFileImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = new SaveFileDialogSettings
        {
            Title = "This is the title",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            }
        };

        var result = await dialogService.ShowSaveFileDialogAsync(owner, settings);
        Path = result?.Path?.ToString();
    }
}
