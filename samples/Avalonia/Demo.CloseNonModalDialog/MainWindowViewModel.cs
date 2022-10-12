using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.CloseNonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;
    private INotifyPropertyChanged? dialogViewModel;
    public INotifyPropertyChanged? DialogViewModel
    {
        get => dialogViewModel;
        set => this.RaiseAndSetIfChanged(ref dialogViewModel, value, nameof(DialogViewModel));
    }
    public ICommand ShowCommand { get; }
    public ICommand CloseCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        var canShow = this.WhenAnyValue(x => x.DialogViewModel).Select(d => d == null);
        ShowCommand = ReactiveCommand.Create(ShowImpl, canShow);

        var canClose = this.WhenAnyValue(x => x.DialogViewModel).Select(d => d != null);
        CloseCommand = ReactiveCommand.Create(CloseImpl, canClose);
    }

    // Run from background threads
    private void ShowImpl()
    {
        DialogViewModel = dialogService.CreateViewModel<CurrentTimeDialogViewModel>();
        dialogService.Show(this, DialogViewModel);
    }

    private void CloseImpl()
    {
        dialogService.Close(DialogViewModel!);
        DialogViewModel = null;
    }
}
