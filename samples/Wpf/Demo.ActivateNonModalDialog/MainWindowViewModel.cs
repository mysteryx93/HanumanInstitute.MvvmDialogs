using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.ActivateNonModalDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private INotifyPropertyChanged? _dialogViewModel;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowCommand = new RelayCommand(Show, CanShow);
        ActivateCommand = new RelayCommand(Activate, CanActivate);
    }

    public RelayCommand ShowCommand { get; }

    public RelayCommand ActivateCommand { get; }

    private void Show()
    {
        _dialogViewModel = _dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        _dialogService.Show(this, _dialogViewModel);

        ShowCommand.NotifyCanExecuteChanged();
        ActivateCommand.NotifyCanExecuteChanged();
    }

    private bool CanShow()
    {
        return _dialogViewModel == null;
    }

    private void Activate()
    {
        _dialogService.Activate(_dialogViewModel!);
    }

    private bool CanActivate()
    {
        return _dialogViewModel != null;
    }
}
