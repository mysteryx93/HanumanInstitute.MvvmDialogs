using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.ViewEvents;

public class MainWindowViewModel : ViewModelBase, IViewLoaded, IViewClosing, IViewClosed
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;
    }

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    private string _text = string.Empty;

    public async void OnClosed()
    {
        await _dialogService.ShowMessageBoxAsync(null, "It's over.", "Closed");
    }

    public void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
    }

    public async Task OnClosingAsync(CancelEventArgs e)
    {
        var quit = await _dialogService.ShowMessageBoxAsync(this, "Do you really want to quit? ", "Confirmation", HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton.YesNo);
        e.Cancel = quit != true;
    }

    public void OnLoaded()
    {
        Text = "Loaded!";
    }
}
