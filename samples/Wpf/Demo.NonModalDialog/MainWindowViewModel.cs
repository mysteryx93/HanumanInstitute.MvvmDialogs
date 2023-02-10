using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.NonModalDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    
    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ImplicitShowCommand = new RelayCommand(ImplicitShow);
        ExplicitShowCommand = new RelayCommand(ExplicitShow);
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
