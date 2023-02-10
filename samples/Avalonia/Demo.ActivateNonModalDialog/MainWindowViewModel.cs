using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.ActivateNonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    private INotifyPropertyChanged? _dialogViewModel;
    protected INotifyPropertyChanged? DialogViewModel
    {
        get => _dialogViewModel;
        set => this.RaiseAndSetIfChanged(ref _dialogViewModel, value, nameof(DialogViewModel));
    }
    public ICommand ShowCommand { get; }
    public ICommand ActivateCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        var canShow = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x == null);
        ShowCommand = ReactiveCommand.Create(Show, canShow);

        var canActivate = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x != null);
        ActivateCommand = ReactiveCommand.Create(Activate, canActivate);
    }

    public void Show()
    {
        DialogViewModel = _dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        _dialogService.Show(this, DialogViewModel);
    }

    public void Activate() => _dialogService.Activate(DialogViewModel!);
}
