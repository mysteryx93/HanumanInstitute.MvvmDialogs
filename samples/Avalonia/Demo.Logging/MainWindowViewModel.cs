﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Logging;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;
    public ICommand ImplicitShowDialogCommand { get; }
    public ICommand ExplicitShowDialogCommand { get; }
    public ObservableCollection<string> Texts { get; } = new ObservableCollection<string>();

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        ImplicitShowDialogCommand = ReactiveCommand.Create(ImplicitShowDialog);
        ExplicitShowDialogCommand = ReactiveCommand.Create(ExplicitShowDialog);
    }

    private Task ImplicitShowDialog() =>
        ShowDialogAsync(viewModel => dialogService.ShowDialogAsync(this, viewModel));

    private Task ExplicitShowDialog() =>
        ShowDialogAsync(viewModel => dialogService.ShowDialogAsync<AddTextDialog>(this, viewModel));

    private async Task ShowDialogAsync(Func<AddTextDialogViewModel, Task<bool?>> showDialogAsync)
    {
        var dialogViewModel = new AddTextDialogViewModel();

        var success = await showDialogAsync(dialogViewModel).ConfigureAwait(true);
        if (success == true)
        {
            Texts.Add(dialogViewModel.Text);
        }
    }
}
