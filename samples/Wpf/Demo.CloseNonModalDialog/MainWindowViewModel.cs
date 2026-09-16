namespace Demo.Wpf.CloseNonModalDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private CurrentTimeDialogViewModel? _dialogViewModel;

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
        SetDialogViewModel(_dialogService.CreateViewModel<CurrentTimeDialogViewModel>());
        _dialogService.Show(this, _dialogViewModel);
    }

    private bool CanShow()
    {
        return _dialogViewModel == null;
    }

    private void Close()
    {
        _dialogService.Close(_dialogViewModel!);
        SetDialogViewModel(null);
    }

    private bool CanClose()
    {
        return _dialogViewModel != null;
    }

    private void SetDialogViewModel(CurrentTimeDialogViewModel? value)
    {
        if (_dialogViewModel != null) _dialogViewModel.Closed -= DialogViewModel_Closed;
        _dialogViewModel = value;
        if (_dialogViewModel != null) _dialogViewModel.Closed += DialogViewModel_Closed;
        ShowCommand.NotifyCanExecuteChanged();
        CloseCommand.NotifyCanExecuteChanged();
    }

    private void DialogViewModel_Closed(object? sender, EventArgs e) => SetDialogViewModel(null);
}
