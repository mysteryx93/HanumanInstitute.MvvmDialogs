using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Wpf.CustomDialogTypeLocator.ComponentA;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.Wpf.CustomDialogTypeLocator;

public class MainWindowVM : ObservableObject
{
    private readonly IDialogService dialogService;

    public MainWindowVM(IDialogService dialogService)
    {
        this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        ShowDialogCommand = new RelayCommand(ShowDialog);
    }

    public ICommand ShowDialogCommand { get; }

    private void ShowDialog()
    {
        var dialogViewModel = dialogService.CreateViewModel<MyDialogVM>();
        dialogService.ShowDialog(this, dialogViewModel);
    }
}
