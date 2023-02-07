using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.CrossPlatform.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        var canShow = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x == null);
        Show = ReactiveCommand.Create(ShowImpl, canShow);

        ShowDialog = ReactiveCommand.CreateFromTask(ShowDialogImplAsync, canShow);

        var canActivate = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x != null);
        Activate = ReactiveCommand.Create(ActivateImpl, canActivate);
        Close = ReactiveCommand.Create(CloseImpl, canActivate);
        OpenFile = ReactiveCommand.CreateFromTask(OpenFileImplAsync);
        OpenFiles = ReactiveCommand.CreateFromTask(OpenFilesImplAsync);
        OpenFolder = ReactiveCommand.CreateFromTask(OpenFolderImplAsync);
        OpenFolders = ReactiveCommand.CreateFromTask(OpenFoldersImplAsync);
        SaveFile = ReactiveCommand.CreateFromTask(SaveFileImplAsync);
        MessageBox = ReactiveCommand.CreateFromTask(MessageBoxImplAsync);
    }
    
    [Reactive]
    public string? Output { get; set; }

    private CurrentTimeViewModel? dialogViewModel;
    protected CurrentTimeViewModel? DialogViewModel
    {
        get => dialogViewModel;
        set
        {
            if (DialogViewModel != null)
            {
                DialogViewModel.ViewClosed -= Dialog_ViewClosed;
            }
            this.RaiseAndSetIfChanged(ref dialogViewModel, value, nameof(DialogViewModel));
            if (DialogViewModel != null)
            {
                DialogViewModel.ViewClosed += Dialog_ViewClosed;
            }
        }
    }

    private void Dialog_ViewClosed(object? sender, EventArgs e) => DialogViewModel = null;

    public RxCommandUnit Show { get; }
    public RxCommandUnit ShowDialog { get; }
    public RxCommandUnit Close { get; }
    public RxCommandUnit Activate { get; }
    public RxCommandUnit OpenFile { get; }
    public RxCommandUnit OpenFiles { get; }
    public RxCommandUnit OpenFolder { get; }
    public RxCommandUnit OpenFolders { get; }
    public RxCommandUnit SaveFile { get; }
    public RxCommandUnit MessageBox { get; }

    private void ShowImpl()
    {
        DialogViewModel = dialogService.CreateViewModel<CurrentTimeViewModel>();
        dialogService.Show(this, DialogViewModel);
    }
    
    private async Task ShowDialogImplAsync()
    {
        var vm = dialogService.CreateViewModel<CurrentTimeViewModel>();
        await dialogService.ShowDialogAsync(this, vm);
    }

    private void ActivateImpl() => dialogService.Activate(DialogViewModel!);

    private void CloseImpl()
    {
        dialogService.Close(DialogViewModel!);
        DialogViewModel = null;
    }

    private async Task OpenFileImplAsync()
    {
        var file = await dialogService.ShowOpenFileDialogAsync(this);
        Output = file?.Path?.ToString();
    }

    private async Task OpenFilesImplAsync()
    {
        var files = await dialogService.ShowOpenFilesDialogAsync(this);
        Output = string.Join(Environment.NewLine, files.Select(x => x?.Path?.ToString() ?? ""));
    }

    private async Task OpenFolderImplAsync()
    {
        var folder = await dialogService.ShowOpenFolderDialogAsync(this);
        Output = folder?.Path?.ToString();
    }

    private async Task OpenFoldersImplAsync()
    {
        var folders = await dialogService.ShowOpenFoldersDialogAsync(this);
        Output = string.Join(Environment.NewLine, folders.Select(x => x?.Path?.ToString() ?? ""));
    }

    private async Task SaveFileImplAsync()
    {
        var file = await dialogService.ShowSaveFileDialogAsync(this);
        Output = file?.Path?.ToString();
    }
    
    private async Task MessageBoxImplAsync()
    {
        var result = await dialogService.ShowMessageBoxAsync(this, "Do you want it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
        Output = result.ToString();
    }
}
