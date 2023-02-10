using System;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.NonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ImplicitShowCommand = ReactiveCommand.Create(ImplicitShow);
        ExplicitShowCommand = ReactiveCommand.Create(ExplicitShow);
    }

    public ICommand ImplicitShowCommand { get; }

    public ICommand ExplicitShowCommand { get; }

    private void ImplicitShow()
    {
        Show(viewModel => _dialogService.Show(this, viewModel));
    }

    private void ExplicitShow()
    {
        Show(viewModel => _dialogService.Show<CurrentTimeDialog>(this, viewModel));
    }

    private void Show(Action<CurrentTimeDialogViewModel> show)
    {
        var dialogViewModel = _dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        show(dialogViewModel);
    }
}
