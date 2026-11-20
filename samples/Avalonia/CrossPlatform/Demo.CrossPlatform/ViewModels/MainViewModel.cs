using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Demo.CrossPlatform.Services;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;

namespace Demo.CrossPlatform.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IStorageService _storage;

    public MainViewModel(IDialogService dialogService, IStorageService storage)
    {
        this._dialogService = dialogService;
        this._storage = storage;

        var canShow = Observable.Select(this.WhenAnyValue(x => x.DialogViewModel), x => x == null);
        Show = ReactiveCommand.Create(ShowImpl, canShow);
        var canActivate = Observable.Select(this.WhenAnyValue(x => x.DialogViewModel), x => x != null);
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
    public partial string? Output { get; set; }

    private CurrentTimeViewModel? _dialogViewModel;
    protected CurrentTimeViewModel? DialogViewModel
    {
        get => _dialogViewModel;
        set
        {
            DialogViewModel?.Closed -= Dialog_ViewClosed;
            this.RaiseAndSetIfChanged(ref _dialogViewModel, value);
            DialogViewModel?.Closed += Dialog_ViewClosed;
        }
    }

    private void Dialog_ViewClosed(object? sender, EventArgs e) => DialogViewModel = null;

    public ReactiveCommand<RxVoid, RxVoid> Show { get; }
    public ReactiveCommand<RxVoid, RxVoid> ShowDialog { get; }
    public ReactiveCommand<RxVoid, RxVoid> Close { get; }
    public ReactiveCommand<RxVoid, RxVoid> Activate { get; }
    public ReactiveCommand<RxVoid, RxVoid> DialogConfirmClose { get; }
    public ReactiveCommand<RxVoid, RxVoid> OpenFile { get; }
    public ReactiveCommand<RxVoid, RxVoid> OpenFiles { get; }
    public ReactiveCommand<RxVoid, RxVoid> OpenFolder { get; }
    public ReactiveCommand<RxVoid, RxVoid> OpenFolders { get; }
    public ReactiveCommand<RxVoid, RxVoid> SaveFile { get; }
    public ReactiveCommand<RxVoid, RxVoid> MessageBox { get; }
    public ReactiveCommand<RxVoid, RxVoid> MessageBoxMultiple { get; }

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
        if (!_storage.CanPickFiles)
        {
            Output = "This platform cannot open a file picker.";
            return;
        }

        var text = new FileFilter("Text", "txt")
        {
            MimeTypes = new[] { "text/plain" },
            AppleUniformTypeIdentifiers = new[] { "public.plain-text" }
        };
        var image = new FileFilter("Image", new[] { "png", "jpg" })
        {
            MimeTypes = new[] { "image/png", "image/jpeg" },
            AppleUniformTypeIdentifiers = new[] { "public.png", "public.jpeg" }
        };
        var settings = new OpenFileDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync(),
            SuggestedFileType = text,
            Filters = new[] { text, image }
        };
        var file = await _dialogService.ShowOpenFileDialogAsync(this, settings);
        if (file == null)
        {
            Output = "Cancelled";
            return;
        }

        using (file)
        {
            Output = file.Path + Environment.NewLine + "Filter: " + (settings.SelectedFileType?.Name ?? "not reported") + Environment.NewLine;
            await ScanFileProgressAsync(file);
            await RestoreBookmarkAsync(file);
        }
    }

    private async Task ScanFileProgressAsync(IDialogStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        var header = Output + "File opened." + Environment.NewLine;
        var canReportPercent = stream.CanSeek && stream.Length > 0;
        var length = canReportPercent ? stream.Length : 0;
        var lastReport = Environment.TickCount64;
        Output = header;

        var buffer = new byte[8192];
        var total = 0L;
        int read;
        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            total += read;
            var now = Environment.TickCount64;
            if (now - lastReport < 100)
            {
                continue;
            }

            lastReport = now;
            Output = header + FormatScanProgress(canReportPercent, total, length);
        }

        Output = header + "Done " + total + " bytes";
    }

    // One progress line under a fixed header. The bound string does not grow with the file.
    private static string FormatScanProgress(bool canReportPercent, long total, long length) =>
        canReportPercent ? ((float)total / length).ToString("P1") : total + " bytes";

    private async Task RestoreBookmarkAsync(IDialogStorageFile file)
    {
        var bookmark = await _storage.SaveBookmarkAsync(file);
        if (bookmark == null)
        {
            return;
        }

        IDialogStorageFile? restored = null;
        try
        {
            restored = await _storage.OpenFileBookmarkAsync(bookmark);
            if (restored == null)
            {
                Output += "Bookmark was not reopened." + Environment.NewLine;
                return;
            }

            await using (var stream = await restored.OpenReadAsync())
            {
                var buffer = new byte[1];
                var read = await stream.ReadAsync(buffer);
                Output += "Bookmark reopened, first read " + read + " byte." + Environment.NewLine;
            }
        }
        finally
        {
            try
            {
                await _storage.ReleaseBookmarkAsync(restored ?? file);
            }
            finally
            {
                restored?.Dispose();
            }
        }
    }

    private async Task OpenFilesImplAsync()
    {
        var settings = new OpenFileDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var files = await _dialogService.ShowOpenFilesDialogAsync(this, settings);
        Output = string.Join(Environment.NewLine, files.Select(x => x?.Path?.ToString() ?? ""));
        foreach (var file in files)
        {
            file.Dispose();
        }
    }

    private async Task OpenFolderImplAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var folder = await _dialogService.ShowOpenFolderDialogAsync(this, settings);
        Output = folder?.Path?.ToString();
        folder?.Dispose();
    }

    private async Task OpenFoldersImplAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync()
        };
        var folders = await _dialogService.ShowOpenFoldersDialogAsync(this, settings);
        Output = string.Join(Environment.NewLine, folders.Select(x => x?.Path?.ToString() ?? ""));
        foreach (var folder in folders)
        {
            folder.Dispose();
        }
    }

    private async Task SaveFileImplAsync()
    {
        if (!_storage.CanSave)
        {
            Output = "This platform cannot open a save picker.";
            return;
        }

        var text = new FileFilter("Text", "txt")
        {
            MimeTypes = new[] { "text/plain" },
            AppleUniformTypeIdentifiers = new[] { "public.plain-text" }
        };
        var settings = new SaveFileDialogSettings
        {
            SuggestedStartLocation = await _storage.GetDownloadsFolderAsync(),
            SuggestedFileName = "note.txt",
            SuggestedFileType = text,
            Filters = new[] { text },
            ShowOverwritePrompt = true
        };
        var file = await _dialogService.ShowSaveFileDialogAsync(this, settings);
        if (file == null)
        {
            Output = "Cancelled";
            return;
        }

        using (file)
        {
            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync("Saved from MvvmDialogs.");
            Output = file.Path + Environment.NewLine + "Filter: " + (settings.SelectedFileType?.Name ?? "not reported");
        }
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
