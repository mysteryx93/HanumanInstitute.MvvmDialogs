using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Demo.Avalonia.NonModalCustomDialog;

public class CurrentTimeCustomDialogViewModel : ViewModelBase
{
    public DateTime CurrentTime => DateTime.Now;

    public CurrentTimeCustomDialogViewModel() =>
        Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe((_) =>
        {
            this.RaisePropertyChanged(nameof(CurrentTime));
        });
}
