using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.CloseNonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private INotifyPropertyChanged? _dialogViewModel;
    public INotifyPropertyChanged? DialogViewModel
    {
        get => _dialogViewModel;
        set => this.RaiseAndSetIfChanged(ref _dialogViewModel, value, nameof(DialogViewModel));
    }
    public ICommand ShowCommand { get; }
    public ICommand CloseCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        var canShow = this.WhenAnyValue(x => x.DialogViewModel).Select(d => d == null);
        ShowCommand = ReactiveCommand.Create(ShowImpl, canShow);

        var canClose = this.WhenAnyValue(x => x.DialogViewModel).Select(d => d != null);
        CloseCommand = ReactiveCommand.Create(CloseImpl, canClose);
    }

    // Run from background threads
    private void ShowImpl()
    {
        DialogViewModel = _dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        _dialogService.Show(this, DialogViewModel);
    }

    private void CloseImpl()
    {
        _dialogService.Close(DialogViewModel!);
        DialogViewModel = null;
    }
}
