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

        this.Loaded += (_, _) => Text = "Loaded!";
        this.Closing += (_, e) => e.Cancel = true;
        this.Closed += async (_, _) => await _dialogService.ShowMessageBoxAsync(null, "It's over.", "Closed");
    }

    /// <inheritdoc />
    public event EventHandler? Loaded;
    /// <inheritdoc />
    public event EventHandler<CancelEventArgs> Closing;
    /// <inheritdoc />
    public event EventHandler? Closed;
    /// <inheritdoc />
    public void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
    /// <inheritdoc />
    public void RaiseClosing(CancelEventArgs e) => Closing?.Invoke(this, e);
    /// <inheritdoc />
    public void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    private string _text = string.Empty;

    public async Task OnClosingAsync(CancelEventArgs e)
    {
        var quit = await _dialogService.ShowMessageBoxAsync(this, "Do you really want to quit? ", "Confirmation", HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton.YesNo);
        e.Cancel = quit != true;
    }
}
