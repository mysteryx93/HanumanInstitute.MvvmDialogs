namespace Demo.Wpf.ActivateNonModalDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private CurrentTimeDialogViewModel? _dialogViewModel;

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
        SetDialogViewModel(_dialogService.CreateViewModel<CurrentTimeDialogViewModel>());
        _dialogService.Show(this, _dialogViewModel);
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

    private void SetDialogViewModel(CurrentTimeDialogViewModel? value)
    {
        if (_dialogViewModel != null) _dialogViewModel.Closed -= DialogViewModel_Closed;
        _dialogViewModel = value;
        if (_dialogViewModel != null) _dialogViewModel.Closed += DialogViewModel_Closed;
        ShowCommand.NotifyCanExecuteChanged();
        ActivateCommand.NotifyCanExecuteChanged();
    }

    private void DialogViewModel_Closed(object? sender, EventArgs e) => SetDialogViewModel(null);
}
