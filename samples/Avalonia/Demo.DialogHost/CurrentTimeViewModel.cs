using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Demo.Avalonia.DialogHost;

public class CurrentTimeViewModel : ViewModelBase
{
    public DateTime CurrentTime => DateTime.Now;
    
    public bool StayOpen { get; set; }

    public CurrentTimeViewModel() =>
        Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe((_) =>
        {
            this.RaisePropertyChanged(nameof(CurrentTime));
        });
}
