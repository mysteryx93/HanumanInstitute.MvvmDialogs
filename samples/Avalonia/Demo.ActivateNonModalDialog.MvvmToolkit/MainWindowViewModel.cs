using System;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.ActivateNonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    private CurrentTimeDialogViewModel? _dialogViewModel;
    public CurrentTimeDialogViewModel? DialogViewModel
    {
        get => _dialogViewModel;
        set
        {
            if (ReferenceEquals(_dialogViewModel, value))
            {
                return;
            }

            if (_dialogViewModel != null)
            {
                _dialogViewModel.Closed -= DialogViewModel_Closed;
            }

            if (SetProperty(ref _dialogViewModel, value))
            {
                if (_dialogViewModel != null)
                {
                    _dialogViewModel.Closed += DialogViewModel_Closed;
                }
                ShowCommand.NotifyCanExecuteChanged();
                ActivateCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public RelayCommand ShowCommand { get; }
    public RelayCommand ActivateCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowCommand = new RelayCommand(Show, () => DialogViewModel == null);
        ActivateCommand = new RelayCommand(Activate, () => DialogViewModel != null);
    }

    public void Show()
    {
        DialogViewModel = _dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        _dialogService.Show(this, DialogViewModel);
    }

    public void Activate() => _dialogService.Activate(DialogViewModel!);

    private void DialogViewModel_Closed(object? sender, EventArgs e) => DialogViewModel = null;
}
