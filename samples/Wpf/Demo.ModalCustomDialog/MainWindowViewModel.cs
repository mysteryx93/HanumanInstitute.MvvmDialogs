using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.ModalCustomDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ImplicitShowDialogCommand = new AsyncRelayCommand(ImplicitShowDialogAsync);
        ExplicitShowDialogCommand = new AsyncRelayCommand(ExplicitShowDialogAsync);
    }

    public ObservableCollection<string> Texts { get; } = new ObservableCollection<string>();

    public ICommand ImplicitShowDialogCommand { get; }

    public ICommand ExplicitShowDialogCommand { get; }

    private Task ImplicitShowDialogAsync() =>
        ShowDialogAsync(viewModel => _dialogService.ShowDialogAsync(this, viewModel));

    private Task ExplicitShowDialogAsync() =>
        ShowDialogAsync(viewModel => _dialogService.ShowDialogAsync<AddTextCustomDialog>(this, viewModel));

    private async Task ShowDialogAsync(Func<AddTextCustomDialogViewModel, Task<bool?>> showDialog)
    {
        var dialogViewModel = _dialogService.CreateViewModel<AddTextCustomDialogViewModel>();

        bool? success = await showDialog(dialogViewModel);
        if (success == true)
        {
            Texts.Add(dialogViewModel.Text!);
        }
    }
}
