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

namespace Demo.Wpf.SaveFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        SaveFile = new RelayCommand(() => SaveFileImpl(SetOwner ? this : null));
        SaveFileAsync = new AsyncRelayCommand(() => SaveFileImplAsync(SetOwner ? this : null));
    }

    public string? Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }
    private string? _path;

    public bool SetOwner
    {
        get => _setOwner;
        set => SetProperty(ref _setOwner, value);
    }
    private bool _setOwner = true;

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

        var result = _dialogService.ShowSaveFileDialog(owner, settings);
        Path = result?.LocalPath;
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

        var result = await _dialogService.ShowSaveFileDialogAsync(owner, settings);
        Path = result?.LocalPath;
    }
}
