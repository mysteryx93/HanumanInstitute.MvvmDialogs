using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Demo.CrossPlatform.ViewModels;

public class ConfirmCloseViewModel : ViewModelBase, IModalDialogViewModel, IViewClosing, IViewLoaded, ICloseable
{
    private readonly IDialogService _dialogService;
    public event EventHandler? RequestClose;
    public bool? DialogResult => true;
    
    public ConfirmCloseViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        Close = ReactiveCommand.Create(CloseImpl);
    }
    
    [Reactive]
    public string Text { get; set; } = string.Empty;
    
    public RxCommandUnit Close { get; }

    public void OnLoaded()
    {
        Text = "This dialog requires close confirmation.";
    }

    public void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
    }
    
    private void CloseImpl()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public async Task OnClosingAsync(CancelEventArgs e)
    {
        var result = await _dialogService.ShowMessageBoxAsync(this, "Do you want to close it?", "Confirmation", MessageBoxButton.YesNo);
        e.Cancel = result == true;
    }
}
