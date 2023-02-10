using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Wpf.CustomDialogTypeLocator.ComponentA;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.Wpf.CustomDialogTypeLocator;

public class MainWindowVm : ObservableObject
{
    private readonly IDialogService _dialogService;

    public MainWindowVm(IDialogService dialogService)
    {
        this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        ShowDialogCommand = new RelayCommand(ShowDialog);
    }

    public ICommand ShowDialogCommand { get; }

    private void ShowDialog()
    {
        var dialogViewModel = _dialogService.CreateViewModel<MyDialogVm>();
        _dialogService.ShowDialog(this, dialogViewModel);
    }
}
