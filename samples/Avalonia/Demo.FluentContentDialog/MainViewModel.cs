using System.Reactive;
using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.Avalonia.FluentContentDialog;

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
        ContentDialogSettings settings = new()
        {
            Content = "This is the text.",
            Title = "This Is The Caption",
            PrimaryButtonText = "OK",
            SecondaryButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Secondary
        };
        var result = await _dialogService.ShowContentDialogAsync(this, settings);

        UpdateResult(result == ContentDialogResult.Primary);
    }

    private void UpdateResult(bool? result) =>
        TextOutput = result == true ? "We got confirmation to continue!" : string.Empty;

    private async Task AskTextImplAsync()
    {
        var vm = _dialogService.CreateViewModel<AskTextBoxViewModel>();
        vm.Title = "Title within the View";
        ContentDialogSettings settings = new()
        {
            Content = vm,
            Title = "Please enter some text",
            PrimaryButtonText = "OK",
            SecondaryButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await _dialogService.ShowContentDialogAsync(this, settings);
        if (result == ContentDialogResult.Primary)
        {
            TextOutput = vm.Text;
        }
    }

    private async Task ShowViewModelImplAsync()
    {
        var dialogViewModel = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        dialogViewModel.ConfirmClose = true;
        await _dialogService.ShowContentDialogAsync(this, new ContentDialogSettings(dialogViewModel)
        {
            PrimaryButtonText = "OK"
        }).ConfigureAwait(true);
    }

    private async Task ShowControlImplAsync()
    {
        var content = new MessageView();
        var result = await _dialogService.ShowContentDialogAsync(this, new ContentDialogSettings(content)
        {
            PrimaryButtonText = "OK"
        }).ConfigureAwait(true);
        TextOutput = result.ToString();
    }
}
