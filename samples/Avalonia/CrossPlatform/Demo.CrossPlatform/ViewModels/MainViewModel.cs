using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.CrossPlatform.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        var canShow = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x == null);
        Show = ReactiveCommand.Create(ShowImpl, canShow);

        ShowDialog = ReactiveCommand.Create(ShowDialogImplAsync, canShow);

        var canActivate = this.WhenAnyValue(x => x.DialogViewModel).Select(x => x != null);
        Activate = ReactiveCommand.Create(ActivateImpl, canActivate);
        Close = ReactiveCommand.Create(CloseImpl, canActivate);
    }

    private CurrentTimeViewModel? dialogViewModel;
    protected CurrentTimeViewModel? DialogViewModel
    {
        get => dialogViewModel;
        set
        {
            if (DialogViewModel != null)
            {
                DialogViewModel.ViewClosed -= Dialog_ViewClosed;
            }
            this.RaiseAndSetIfChanged(ref dialogViewModel, value, nameof(DialogViewModel));
            if (DialogViewModel != null)
            {
                DialogViewModel.ViewClosed += Dialog_ViewClosed;
            }
        }
    }

    private void Dialog_ViewClosed(object? sender, EventArgs e) => DialogViewModel = null;

    public ICommand Show { get; }
    public ICommand ShowDialog { get; }
    public ICommand Close { get; }
    public ICommand Activate { get; }

    private void ShowImpl()
    {
        DialogViewModel = dialogService.CreateViewModel<CurrentTimeViewModel>();
        dialogService.Show(this, DialogViewModel);
    }
    
    private async Task ShowDialogImplAsync()
    {
        var vm = dialogService.CreateViewModel<CurrentTimeViewModel>();
        await dialogService.ShowDialogAsync(this, vm);
    }

    private void ActivateImpl() => dialogService.Activate(DialogViewModel!);

    private void CloseImpl()
    {
        dialogService.Close(DialogViewModel!);
        DialogViewModel = null;
    }
}
