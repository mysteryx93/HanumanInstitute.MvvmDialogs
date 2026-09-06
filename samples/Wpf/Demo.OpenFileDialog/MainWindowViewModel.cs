using System.Collections.Generic;
using System.Reflection;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.Wpf.OpenFileDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    public ObservableCollection<string> Paths { get; private set; } = [];

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

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
        get => _setOwner;
        set => SetProperty(ref _setOwner, value);
    }
    private bool _setOwner = true;

    private void OpenFileImpl(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(false);
        var result = _dialogService.ShowOpenFileDialog(owner, settings);
        Paths.Clear();
        if (result != null)
        {
            Paths.Add(result.LocalPath);
        }
    }

    private void OpenFilesImpl(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(true);
        var result = _dialogService.ShowOpenFilesDialog(owner, settings);
        Paths.Clear();
        foreach (var item in result)
        {
            Paths.Add(item.LocalPath);
        }
    }

    private async Task OpenFileImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(false);
        var result = await _dialogService.ShowOpenFileDialogAsync(owner, settings);
        Paths.Clear();
        if (result != null)
        {
            Paths.Add(result.LocalPath ?? "");
        }
    }

    private async Task OpenFilesImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = GetSettings(true);
        var result = await _dialogService.ShowOpenFilesDialogAsync(owner, settings);
        Paths.Clear();
        foreach (var item in result)
        {
            Paths.Add(item.LocalPath ?? "");
        }
    }

    private static OpenFileDialogSettings GetSettings(bool multiple) => new()
    {
        Title = multiple ? "Open multiple files" : "Open single file",
        SuggestedStartLocation = new DesktopDialogStorageFolder(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!),
        SuggestedFileName = "InitialName",
        Filters = new List<FileFilter>()
            {
                new("Text Documents", "txt"),
                new("All Files", "*")
            }
    };
}
