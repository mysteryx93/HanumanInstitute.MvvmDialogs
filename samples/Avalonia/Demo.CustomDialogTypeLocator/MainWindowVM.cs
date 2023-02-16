using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Demo.Avalonia.CustomDialogTypeLocator.ComponentA;
using ReactiveUI;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.CustomDialogTypeLocator;

public class MainWindowVm : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public MainWindowVm(IDialogService dialogService)
    {
        this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        ShowDialogCommand = ReactiveCommand.Create(ShowDialogAsync);
    }

    public ICommand ShowDialogCommand { get; }

    private async Task ShowDialogAsync()
    {
        var dialogViewModel = _dialogService.CreateViewModel<MyDialogVM>();
        await _dialogService.ShowDialogAsync(this, dialogViewModel);
    }
}
