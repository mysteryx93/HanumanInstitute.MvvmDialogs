using System;
using System.Reactive.Linq;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Avalonia.NonModalDialog;

public class CurrentTimeDialogViewModel : ViewModelBase, IViewClosed
{
    public DateTime CurrentTime => DateTime.Now;

    private readonly IDisposable _clock;

    public CurrentTimeDialogViewModel() =>
        _clock = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            this.RaisePropertyChanged(nameof(CurrentTime));
        });

    public void OnClosed() => _clock.Dispose();
}
