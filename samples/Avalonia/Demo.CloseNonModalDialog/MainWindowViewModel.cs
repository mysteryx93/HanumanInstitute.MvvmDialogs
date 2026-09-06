using System.Reactive.Linq;
using System;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.CloseNonModalDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private CurrentTimeDialogViewModel? _dialogViewModel;
    public CurrentTimeDialogViewModel? DialogViewModel
    {
        get => _dialogViewModel;
        set
        {
            if (ReferenceEquals(_dialogViewModel, value)) return;
            if (_dialogViewModel != null) _dialogViewModel.Closed -= DialogViewModel_Closed;
            this.RaiseAndSetIfChanged(ref _dialogViewModel, value, nameof(DialogViewModel));
            if (_dialogViewModel != null) _dialogViewModel.Closed += DialogViewModel_Closed;
        }
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

    private void DialogViewModel_Closed(object? sender, EventArgs e) => DialogViewModel = null;
}
