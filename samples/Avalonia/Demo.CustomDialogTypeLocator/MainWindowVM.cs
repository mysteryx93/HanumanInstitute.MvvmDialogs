using System;
using System.Windows.Input;
using System.Threading.Tasks;
using ReactiveUI;
using HanumanInstitute.MvvmDialogs;
using Demo.CustomDialogTypeLocator.ComponentA;

namespace Demo.CustomDialogTypeLocator;

public class MainWindowVM : ViewModelBase
{
    private readonly IDialogService dialogService;

    public MainWindowVM(IDialogService dialogService)
    {
        this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        ShowDialogCommand = ReactiveCommand.Create(ShowDialogAsync);
    }

    public ICommand ShowDialogCommand { get; }

    private async Task ShowDialogAsync()
    {
        var dialogViewModel = dialogService.CreateViewModel<MyDialogVM>();
        await dialogService.ShowDialogAsync(this, dialogViewModel);
    }
}
