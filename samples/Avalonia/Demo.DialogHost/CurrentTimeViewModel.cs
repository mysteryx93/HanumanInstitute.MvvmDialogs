using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.DialogHost;

public partial class CurrentTimeViewModel : ViewModelBase, IViewClosing, IViewClosed
{
    public DateTime CurrentTime => DateTime.Now;

    [ReactiveUI.SourceGenerators.Reactive]
    private bool _confirmClose;

    [ReactiveUI.SourceGenerators.Reactive]
    private bool _stayOpen;
    private readonly IDisposable _clock;

    public CurrentTimeViewModel() =>
        _clock = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            this.RaisePropertyChanged(nameof(CurrentTime));
        });

    public void OnClosed() => _clock.Dispose();

    public void OnClosing(CancelEventArgs e)
    {
        if (ConfirmClose)
        {
            e.Cancel = StayOpen;
        }
    }

    public Task OnClosingAsync(CancelEventArgs e) => Task.CompletedTask;
}
