using System;
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

        this.ViewLoaded += (_, _) => Text = "Loaded!";
        this.ViewClosing += (_, e) => e.Cancel = true;
        this.ViewClosed += async (_, _) => await _dialogService.ShowMessageBoxAsync(null, "It's over.", "Closed");
    }

    /// <inheritdoc />
    public event EventHandler? ViewLoaded;
    /// <inheritdoc />
    public event EventHandler<CancelEventArgs> ViewClosing;
    /// <inheritdoc />
    public event EventHandler? ViewClosed;
    /// <inheritdoc />
    public void RaiseViewLoaded() => ViewLoaded?.Invoke(this, EventArgs.Empty);
    /// <inheritdoc />
    public void RaiseViewClosing(CancelEventArgs e) => ViewClosing?.Invoke(this, e);
    /// <inheritdoc />
    public void RaiseViewClosed() => ViewClosed?.Invoke(this, EventArgs.Empty);

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    private string _text = string.Empty;

    public async Task OnViewClosingAsync(CancelEventArgs e)
    {
        var quit = await _dialogService.ShowMessageBoxAsync(this, "Do you really want to quit? ", "Confirmation", HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton.YesNo);
        e.Cancel = quit != true;
    }
}
