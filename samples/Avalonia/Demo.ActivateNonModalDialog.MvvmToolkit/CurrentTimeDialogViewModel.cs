using System;
using System.Threading;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.ActivateNonModalDialog;

public class CurrentTimeDialogViewModel : ViewModelBase, IViewLoaded, IViewClosed
{
    public event EventHandler? Closed;

    public DateTime CurrentTime => DateTime.Now;
    private Timer? _timer;

    public void OnLoaded()
    {
        _timer?.Dispose();
        _timer = new Timer(_ => OnPropertyChanged(nameof(CurrentTime)), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public void OnClosed()
    {
        _timer?.Dispose();
        _timer = null;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}
