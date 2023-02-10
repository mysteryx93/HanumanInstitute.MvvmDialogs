using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.CloseNonModalDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private INotifyPropertyChanged? _dialogViewModel;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowCommand = new RelayCommand(Show, CanShow);
        CloseCommand = new RelayCommand(Close, CanClose);
    }

    public RelayCommand ShowCommand { get; }

    public RelayCommand CloseCommand { get; }

    private void Show()
    {
        _dialogViewModel = _dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        _dialogService.Show(this, _dialogViewModel);

        ShowCommand.NotifyCanExecuteChanged();
        CloseCommand.NotifyCanExecuteChanged();
    }

    private bool CanShow()
    {
        return _dialogViewModel == null;
    }

    private void Close()
    {
        _dialogService.Close(_dialogViewModel!);
        _dialogViewModel = null;

        ShowCommand.NotifyCanExecuteChanged();
        CloseCommand.NotifyCanExecuteChanged();
    }

    private bool CanClose()
    {
        return _dialogViewModel != null;
    }
}
