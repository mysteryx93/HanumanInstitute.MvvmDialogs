using System.Reactive;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.Avalonia.DialogHost;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowViewModel = ReactiveCommand.CreateFromTask(ShowViewModelImplAsync);
        AskText = ReactiveCommand.CreateFromTask(AskTextImplAsync);
        ShowMessage = ReactiveCommand.CreateFromTask(ShowMessageImplAsync);
        ShowControl = ReactiveCommand.CreateFromTask(ShowControlImplAsync);
        ConfirmClose = ReactiveCommand.CreateFromTask(ConfirmCloseImplAsync);
    }

    public ReactiveCommand<Unit, Unit> ShowViewModel { get; }
    public ReactiveCommand<Unit, Unit> AskText { get; }
    public ReactiveCommand<Unit, Unit> ShowMessage { get; }
    public ReactiveCommand<Unit, Unit> ShowControl { get; }
    public ReactiveCommand<Unit, Unit> ConfirmClose { get; }

    [Reactive]
    public string? TextOutput { get; set; }

    private async Task ShowViewModelImplAsync()
    {
        var dialogViewModel = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        await _dialogService.ShowDialogHostAsync(
            this,
            new DialogHostSettings(dialogViewModel)
            {
                CloseOnClickAway = true,
                ClosingHandler = (_, e) =>
                {
                    if (dialogViewModel.StayOpen)
                    {
                        e.Cancel();
                    }
                }
            });
    }

    private async Task AskTextImplAsync()
    {
        TextOutput = await _dialogService.AskTextAsync(this);
    }

    private async Task ShowMessageImplAsync()
    {
        var result = await _dialogService.ShowDialogHostAsync(
            this,
            new DialogHostSettings("Hello world!")
            {
                CloseOnClickAway = true
            }).ConfigureAwait(true);
        TextOutput = result?.ToString() ?? "(null)";
    }

    private async Task ShowControlImplAsync()
    {
        var content = new MessageView();
        var result = await _dialogService.ShowDialogHostAsync(this, new DialogHostSettings(content)).ConfigureAwait(true);
        TextOutput = result?.ToString() ?? "(null)";
    }

    private async Task ConfirmCloseImplAsync()
    {
        var dialogViewModel = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        dialogViewModel.ConfirmClose = true;
        // dialogViewModel.Owner = this;
        await _dialogService.ShowDialogHostAsync(
            this,
            new DialogHostSettings(dialogViewModel)
            {
                CloseOnClickAway = true
            });
    }
}
