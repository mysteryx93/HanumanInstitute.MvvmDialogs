using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Demo.CrossPlatform.Services;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.CrossPlatform.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IStorageService _storage;

    public MainViewModel(IDialogService dialogService, IStorageService storage)
    {
        this._dialogService = dialogService;
        this._storage = storage;

        var canShow = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x == null);
        Show = ReactiveCommand.Create(ShowImpl, canShow);
        var canActivate = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x != null);
        Activate = ReactiveCommand.Create(ActivateImpl, canActivate);
        Close = ReactiveCommand.Create(CloseImpl, canActivate);
        ShowDialog = ReactiveCommand.CreateFromTask(ShowDialogImplAsync);
        DialogConfirmClose = ReactiveCommand.CreateFromTask(DialogConfirmCloseImplAsync);
        OpenFile = ReactiveCommand.CreateFromTask(OpenFileImplAsync);
        OpenFiles = ReactiveCommand.CreateFromTask(OpenFilesImplAsync);
        OpenFolder = ReactiveCommand.CreateFromTask(OpenFolderImplAsync);
        OpenFolders = ReactiveCommand.CreateFromTask(OpenFoldersImplAsync);
        SaveFile = ReactiveCommand.CreateFromTask(SaveFileImplAsync);
        MessageBox = ReactiveCommand.CreateFromTask(MessageBoxImplAsync);
        MessageBoxMultiple = ReactiveCommand.CreateFromTask(MessageBoxMultipleImplAsync);
    }
    
    [Reactive]
    public string? Output { get; set; }

    private CurrentTimeViewModel? _dialogViewModel;
    protected CurrentTimeViewModel? DialogViewModel
    {
        get => _dialogViewModel;
        set
        {
            if (DialogViewModel != null)
            {
                DialogViewModel.Closed -= Dialog_ViewClosed;
            }
            this.RaiseAndSetIfChanged(ref _dialogViewModel, value);
            if (DialogViewModel != null)
            {
                DialogViewModel.Closed += Dialog_ViewClosed;
            }
        }
    }

    private void Dialog_ViewClosed(object? sender, EventArgs e) => DialogViewModel = null;

    public RxCommandUnit Show { get; }
    public RxCommandUnit ShowDialog { get; }
    public RxCommandUnit Close { get; }
    public RxCommandUnit Activate { get; }
    public RxCommandUnit DialogConfirmClose { get; }
    public RxCommandUnit OpenFile { get; }
    public RxCommandUnit OpenFiles { get; }
    public RxCommandUnit OpenFolder { get; }
    public RxCommandUnit OpenFolders { get; }
    public RxCommandUnit SaveFile { get; }
    public RxCommandUnit MessageBox { get; }
    public RxCommandUnit MessageBoxMultiple { get; }

    private void ShowImpl()
    {
        DialogViewModel = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        _dialogService.Show(this, DialogViewModel);
    }
    
    private void ActivateImpl() => _dialogService.Activate(DialogViewModel!);

    private void CloseImpl()
    {
        _dialogService.Close(DialogViewModel!);
        DialogViewModel = null;
    }
    
    private async Task ShowDialogImplAsync()
    {
        var vm = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        await _dialogService.ShowDialogAsync(this, vm);
    }

    private async Task DialogConfirmCloseImplAsync()
    {
        var vm = _dialogService.CreateViewModel<ConfirmCloseViewModel>();
        await _dialogService.ShowDialogAsync(this, vm);
    }

    private async Task OpenFileImplAsync()
    {
        var settings = new OpenFileDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var file = await _dialogService.ShowOpenFileDialogAsync(this, settings);
        Output = file?.Path?.ToString();
    }

    private async Task OpenFilesImplAsync()
    {
        var settings = new OpenFileDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var files = await _dialogService.ShowOpenFilesDialogAsync(this, settings);
        Output = string.Join(Environment.NewLine, files.Select(x => x?.Path?.ToString() ?? ""));
    }

    private async Task OpenFolderImplAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var folder = await _dialogService.ShowOpenFolderDialogAsync(this, settings);
        Output = folder?.Path?.ToString();
    }

    private async Task OpenFoldersImplAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var folders = await _dialogService.ShowOpenFoldersDialogAsync(this, settings);
        Output = string.Join(Environment.NewLine, folders.Select(x => x?.Path?.ToString() ?? ""));
    }

    private async Task SaveFileImplAsync()
    {
        var settings = new SaveFileDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var file = await _dialogService.ShowSaveFileDialogAsync(this, settings);
        Output = file?.Path?.ToString();
    }
    
    private async Task MessageBoxImplAsync()
    {
        var result = await _dialogService.ShowMessageBoxAsync(this, "Do you want it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
        Output = result.ToString();
    }

    private async Task MessageBoxMultipleImplAsync()
    {
        var t1 = _dialogService.ShowMessageBoxAsync(this, "First message box", "Go", MessageBoxButton.YesNo, MessageBoxImage.Exclamation).ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
        var t2 = _dialogService.ShowMessageBoxAsync(this, "Second message box", "Again", MessageBoxButton.YesNo, MessageBoxImage.Exclamation).ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
        var t3 = _dialogService.ShowMessageBoxAsync(this, "Third message box", "Once More!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation).ConfigureAwait(false);
        var r1 = await t1;
        var r2 = await t2;
        var r3 = await t3;
        Output = r1 + Environment.NewLine + r2 + Environment.NewLine + r3;
    }
}
