using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.ModalCustomDialog;

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
        ShowDialogAsync(viewModel => dialogService.ShowDialogAsync<AddTextCustomDialog>(this, viewModel));

    private async Task ShowDialogAsync(Func<AddTextCustomDialogViewModel, Task<bool?>> showDialog)
    {
        var dialogViewModel = new AddTextCustomDialogViewModel();

        bool? success = await showDialog(dialogViewModel);
        if (success == true)
        {
            Texts.Add(dialogViewModel.Text!);
        }
    }
}
