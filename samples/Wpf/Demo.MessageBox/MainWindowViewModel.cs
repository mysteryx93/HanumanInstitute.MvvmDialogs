using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace Demo.Wpf.MessageBox;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private string? _confirmation;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowWithMessage = new AsyncRelayCommand(() => Show(ShowWithMessageImpl, ShowWithMessageImplAsync));
        ShowWithCaption = new AsyncRelayCommand(() => Show(ShowWithCaptionImpl, ShowWithCaptionImplAsync));
        ShowWithButton = new AsyncRelayCommand(() => Show(ShowWithButtonImpl, ShowWithButtonImplAsync));
        ShowWithIcon = new AsyncRelayCommand(() => Show(ShowWithIconImpl, ShowWithIconImplAsync));
        ShowWithDefaultResult = new AsyncRelayCommand(() => Show(ShowWithDefaultResultImpl, ShowWithDefaultResultImplAsync));
    }

    public ICommand ShowWithMessage { get; }
    public ICommand ShowWithCaption { get; }
    public ICommand ShowWithButton { get; }
    public ICommand ShowWithIcon { get; }
    public ICommand ShowWithDefaultResult { get; }

    private Task Show(Action action, Func<Task> asyncAction)
    {
        if (UseAsync)
        {
            return asyncAction();
        }
        action();
        return Task.CompletedTask;
    }

    public bool UseAsync
    {
        get => _useAsync;
        set => SetProperty(ref _useAsync, value);
    }
    private bool _useAsync = true;

    public bool SetOwner
    {
        get => _setOwner;
        set => SetProperty(ref _setOwner, value);
    }
    private bool _setOwner = true;

    public string? Confirmation
    {
        get => _confirmation;
        private set => SetProperty(ref _confirmation, value);
    }

    private void ShowWithMessageImpl()
    {
        var result = _dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.");

        UpdateResult(result);
    }

    private async Task ShowWithMessageImplAsync()
    {
        var result = await _dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.");

        UpdateResult(result);
    }

    private void ShowWithCaptionImpl()
    {
        var result = _dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption");

        UpdateResult(result);
    }

    private async Task ShowWithCaptionImplAsync()
    {
        var result = await _dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption");

        UpdateResult(result);
    }

    private void ShowWithButtonImpl()
    {
        var result = _dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel);

        UpdateResult(result);
    }

    private async Task ShowWithButtonImplAsync()
    {
        var result = await _dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel);

        UpdateResult(result);
    }

    private void ShowWithIconImpl()
    {
        var result = _dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information);

        UpdateResult(result);
    }

    private async Task ShowWithIconImplAsync()
    {
        var result = await _dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information);

        UpdateResult(result);
    }

    private void ShowWithDefaultResultImpl()
    {
        var result = _dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information, false);

        UpdateResult(result);
    }

    private async Task ShowWithDefaultResultImplAsync()
    {
        var result = await _dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information, false);

        UpdateResult(result);
    }

    private void UpdateResult(bool? result)
    {
        Confirmation = result == true ? "We got confirmation to continue!" : string.Empty;
    }
}
