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
    private readonly IDialogService dialogService;

    private string? confirmation;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

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
        get => useAsync;
        set => SetProperty(ref useAsync, value);
    }
    private bool useAsync = true;

    public bool SetOwner
    {
        get => setOwner;
        set => SetProperty(ref setOwner, value);
    }
    private bool setOwner = true;

    public string? Confirmation
    {
        get => confirmation;
        private set => SetProperty(ref confirmation, value);
    }

    private void ShowWithMessageImpl()
    {
        var result = dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.");

        UpdateResult(result);
    }

    private async Task ShowWithMessageImplAsync()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.");

        UpdateResult(result);
    }

    private void ShowWithCaptionImpl()
    {
        var result = dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption");

        UpdateResult(result);
    }

    private async Task ShowWithCaptionImplAsync()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption");

        UpdateResult(result);
    }

    private void ShowWithButtonImpl()
    {
        var result = dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel);

        UpdateResult(result);
    }

    private async Task ShowWithButtonImplAsync()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel);

        UpdateResult(result);
    }

    private void ShowWithIconImpl()
    {
        var result = dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information);

        UpdateResult(result);
    }

    private async Task ShowWithIconImplAsync()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information);

        UpdateResult(result);
    }

    private void ShowWithDefaultResultImpl()
    {
        var result = dialogService.ShowMessageBox(
            SetOwner ? this : null,
            "This is the text.", "This Is The Caption",
            MessageBoxButton.OkCancel, MessageBoxImage.Information, false);

        UpdateResult(result);
    }

    private async Task ShowWithDefaultResultImplAsync()
    {
        var result = await dialogService.ShowMessageBoxAsync(
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
