using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.Avalonia.DialogHost;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowDialogHost = ReactiveCommand.CreateFromTask(ShowDialogHostImplAsync);
        AskTextDialogHost = ReactiveCommand.CreateFromTask(AskTextDialogHostImplAsync);
    }

    public ICommand ShowDialogHost { get; }
    public ICommand AskTextDialogHost { get; }
    
    [Reactive]
    public string? TextOutput { get; set; }

    private async Task ShowDialogHostImplAsync()
    {
        var dialogViewModel = _dialogService.CreateViewModel<CurrentTimeViewModel>();
        await _dialogService.ShowCurrentTimeAsync(
            this,
            dialogViewModel,
            (_, e) =>
            {
                if (dialogViewModel.StayOpen)
                {
                    e.Cancel();
                }
            });
    }

    private async Task AskTextDialogHostImplAsync()
    {
        TextOutput = await _dialogService.AskTextAsync(this);
    }
}
