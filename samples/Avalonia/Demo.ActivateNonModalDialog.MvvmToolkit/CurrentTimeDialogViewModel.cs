using System;
using System.Threading;

namespace Demo.Avalonia.ActivateNonModalDialog;

public class CurrentTimeDialogViewModel : ViewModelBase
{
    public DateTime CurrentTime => DateTime.Now;
    private Timer _timer;

    public CurrentTimeDialogViewModel()
    {
        _timer = new Timer(_ => OnPropertyChanged(nameof(CurrentTime)), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }
}
