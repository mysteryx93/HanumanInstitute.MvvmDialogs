using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Demo.Avalonia.CloseNonModalDialog;

public class CurrentTimeDialogViewModel : ViewModelBase
{
    public DateTime CurrentTime => DateTime.Now;

    public CurrentTimeDialogViewModel() =>
        Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe((_) =>
        {
            this.RaisePropertyChanged(nameof(CurrentTime));
        });
}
