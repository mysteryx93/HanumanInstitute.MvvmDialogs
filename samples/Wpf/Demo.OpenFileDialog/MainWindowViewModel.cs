using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

namespace Demo.Wpf.OpenFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    public ObservableCollection<string> Paths { get; private set; } = new();

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenFile = new RelayCommand(() => OpenFileImpl(SetOwner ? this : null));
        OpenFiles = new RelayCommand(() => OpenFilesImpl(SetOwner ? this : null));
        OpenFileAsync = new AsyncRelayCommand(() => OpenFileImplAsync(SetOwner ? this : null));
        OpenFilesAsync = new AsyncRelayCommand(() => OpenFilesImplAsync(SetOwner ? this : null));
    }

    public ICommand OpenFile { get; }
    public ICommand OpenFiles { get; }
    public ICommand OpenFileAsync { get; }
    public ICommand OpenFilesAsync { get; }

    public bool SetOwner
    {
        get => setOwner;
        set => SetProperty(ref setOwner, value);
    }
    private bool setOwner = true;

    private void OpenFileImpl(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(false);
        var result = dialogService.ShowOpenFileDialog(owner, settings);
        Paths.Clear();
        if (result != null)
        {
            Paths.Add(result);
        }
    }

    private void OpenFilesImpl(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(true);
        var result = dialogService.ShowOpenFilesDialog(owner, settings);
        Paths.Clear();
        foreach (var item in result)
        {
            Paths.Add(item);
        }
    }

    private async Task OpenFileImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(false);
        var result = await dialogService.ShowOpenFileDialogAsync(owner, settings);
        Paths.Clear();
        if (result != null)
        {
            Paths.Add(result.Path?.ToString() ?? "");
        }
    }

    private async Task OpenFilesImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(true);
        var result = await dialogService.ShowOpenFilesDialogAsync(owner, settings);
        Paths.Clear();
        foreach (var item in result)
        {
            Paths.Add(item.Path?.ToString() ?? "");
        }
    }

    private OpenFileDialogSettings GetSettings(bool multiple) => new OpenFileDialogSettings
    {
        Title = multiple ? "Open multiple files" : "Open single file",
        InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        Filters = new List<FileFilter>()
            {
                new FileFilter("Text Documents", "txt"),
                new FileFilter("All Files", "*")
            }
    };
}
