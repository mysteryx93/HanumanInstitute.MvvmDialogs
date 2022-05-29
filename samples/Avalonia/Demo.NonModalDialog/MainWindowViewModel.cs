using System;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.NonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        ImplicitShowCommand = ReactiveCommand.Create(ImplicitShow);
        ExplicitShowCommand = ReactiveCommand.Create(ExplicitShow);
    }

    public ICommand ImplicitShowCommand { get; }

    public ICommand ExplicitShowCommand { get; }

    private void ImplicitShow()
    {
        Show(viewModel => dialogService.Show(this, viewModel));
    }

    private void ExplicitShow()
    {
        Show(viewModel => dialogService.Show<CurrentTimeDialog>(this, viewModel));
    }

    private void Show(Action<CurrentTimeDialogViewModel> show)
    {
        var dialogViewModel = dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        show(dialogViewModel);
    }
}
