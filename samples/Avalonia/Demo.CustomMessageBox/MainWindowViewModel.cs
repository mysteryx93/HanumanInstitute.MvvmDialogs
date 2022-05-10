using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using Ookii.Dialogs.WinForms;
using ReactiveUI;

namespace Demo.CustomMessageBox;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    private string? confirmation;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        ShowMessageBoxWithMessageCommand = ReactiveCommand.Create(ShowMessageBoxWithMessageAsync);
        ShowMessageBoxWithCaptionCommand = ReactiveCommand.Create(ShowMessageBoxWithCaptionAsync);
        ShowMessageBoxWithButtonCommand = ReactiveCommand.Create(ShowMessageBoxWithButtonAsync);
        ShowMessageBoxWithIconCommand = ReactiveCommand.Create(ShowMessageBoxWithIconAsync);
        ShowMessageBoxWithDefaultResultCommand = ReactiveCommand.Create(ShowMessageBoxWithDefaultResultAsync);
    }

    public ICommand ShowMessageBoxWithMessageCommand { get; }

    public ICommand ShowMessageBoxWithCaptionCommand { get; }

    public ICommand ShowMessageBoxWithButtonCommand { get; }

    public ICommand ShowMessageBoxWithIconCommand { get; }

    public ICommand ShowMessageBoxWithDefaultResultCommand { get; }

    public string? Confirmation
    {
        get => confirmation;
        private set => this.RaiseAndSetIfChanged(ref confirmation, value);
    }

    private async Task ShowMessageBoxWithMessageAsync()
    {
        var result = await dialogService.ShowTaskMessageBoxAsync(
            this,
            "This is the text.");

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithCaptionAsync()
    {
        var result = await dialogService.ShowTaskMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption");

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithButtonAsync()
    {
        var result = await dialogService.ShowTaskMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption",
            MessageBoxButton.OkCancel);

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithIconAsync()
    {
        var result = await dialogService.ShowTaskMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption",
            MessageBoxButton.OkCancel,
            MessageBoxImage.Information);

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithDefaultResultAsync()
    {
        var result = await dialogService.ShowTaskMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption",
            MessageBoxButton.OkCancel,
            MessageBoxImage.Information,
            false);

        UpdateResult(result);
    }

    private void UpdateResult(TaskDialogButton result)
    {
        Confirmation = result.ButtonType.ToString();
    }
}
