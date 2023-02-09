using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.ModalDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        ImplicitShowDialogCommand = new AsyncRelayCommand(ImplicitShowDialogAsync);
        ExplicitShowDialogCommand = new AsyncRelayCommand(ExplicitShowDialogAsync);
    }

    public ObservableCollection<string> Texts { get; } = new ObservableCollection<string>();

    public ICommand ImplicitShowDialogCommand { get; }

    public ICommand ExplicitShowDialogCommand { get; }

    private Task ImplicitShowDialogAsync() =>
        ShowDialogAsync(viewModel => dialogService.ShowDialogAsync(this, viewModel));

    private Task ExplicitShowDialogAsync() =>
        ShowDialogAsync(viewModel => dialogService.ShowDialogAsync<AddTextDialog>(this, viewModel));

    private async Task ShowDialogAsync(Func<AddTextDialogViewModel, Task<bool?>> showDialog)
    {
        var dialogViewModel = dialogService.CreateViewModel<AddTextDialogViewModel>();

        bool? success = await showDialog(dialogViewModel);
        if (success == true)
        {
            Texts.Add(dialogViewModel.Text!);
        }
    }
}
