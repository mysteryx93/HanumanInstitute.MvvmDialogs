using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.Avalonia.FluentTaskDialog;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    public ReactiveCommand<Unit, Unit> ShowMessageBox { get; }
    public ReactiveCommand<Unit, Unit> AskText { get; }
    public ReactiveCommand<Unit, Unit> ShowViewModel { get; }
    public ReactiveCommand<Unit, Unit> ShowControl { get; }

    public MainViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowMessageBox = ReactiveCommand.CreateFromTask(ShowMessageBoxImplAsync);
        AskText = ReactiveCommand.CreateFromTask(AskTextImplAsync);
        ShowViewModel = ReactiveCommand.CreateFromTask(ShowViewModelImplAsync);
        ShowControl = ReactiveCommand.CreateFromTask(ShowControlImplAsync);
    }

    [Reactive]
    public string? TextOutput { get; set; }

    private async Task ShowMessageBoxImplAsync()
    {
        TaskDialogSettings settings = new()
        {
            Content = "This is the text.",
            Title = "This Is The Caption",
            Buttons = new List<TaskDialogButton>()
            {
                TaskDialogButton.OKButton,
                TaskDialogButton.CancelButton
            }
        };
        var result = await _dialogService.ShowTaskDialogAsync(this, settings);

        UpdateResult(result == TaskDialogStandardResult.OK);
    }

    private void UpdateResult(bool? result) =>
        TextOutput = result == true ? "We got confirmation to continue!" : string.Empty;

    private async Task AskTextImplAsync()
    {
        var vm = _dialogService.CreateViewModel<AskTextBoxViewModel>();
        vm.Title = "Title within the View";
        TaskDialogSettings settings = new()
        {
            Content = vm,
            Title = "Please enter some text",
            Buttons = new List<TaskDialogButton>()
            {
                TaskDialogButton.OKButton,
                TaskDialogButton.CancelButton
            }
        };
        var result = await _dialogService.ShowTaskDialogAsync(this, settings);
        if (result == TaskDialogStandardResult.OK)
        {
            TextOutput = vm.Text;
        }
    }

    private async Task ShowViewModelImplAsync()
    {
        var dialogViewModel = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        dialogViewModel.ConfirmClose = true;
        await _dialogService.ShowTaskDialogAsync(this, new TaskDialogSettings(dialogViewModel)
        {
            Buttons = new List<TaskDialogButton>()
            {
                TaskDialogButton.OKButton
            }

        }).ConfigureAwait(true);
    }

    private async Task ShowControlImplAsync()
    {
        var content = new MessageView();
        var result = await _dialogService.ShowTaskDialogAsync(this, new TaskDialogSettings(content)
        {
            Buttons = new List<TaskDialogButton>()
            {
                TaskDialogButton.OKButton
            }
        }).ConfigureAwait(true);
        TextOutput = result.ToString();
    }
}
