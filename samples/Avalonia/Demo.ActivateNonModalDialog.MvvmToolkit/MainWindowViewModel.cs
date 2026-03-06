using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.ActivateNonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    private INotifyPropertyChanged? _dialogViewModel;
    public INotifyPropertyChanged? DialogViewModel
    {
        get => _dialogViewModel;
        set
        {
            if (SetProperty(ref _dialogViewModel, value))
            {
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
}
