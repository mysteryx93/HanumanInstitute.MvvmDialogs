namespace Demo.Wpf.CloseNonModalDialog;

public class CurrentTimeDialogViewModel : ObservableObject, IViewClosed
{
    public event EventHandler? Closed;

    private DispatcherTimer? _timer;

    public CurrentTimeDialogViewModel()
    {
        StartClockCommand = new RelayCommand(StartClock);
    }

    public ICommand StartClockCommand { get; }

    public DateTime CurrentTime => DateTime.Now;

    private void StartClock()
    {
        StopClock();
        _timer = new DispatcherTimer(
            TimeSpan.FromSeconds(1),
            DispatcherPriority.Normal,
            OnTick,
            Dispatcher.CurrentDispatcher);
    }

    private void StopClock()
    {
        if (_timer is null)
        {
            return;
        }

        _timer.Stop();
        _timer.Tick -= OnTick;
        _timer = null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CurrentTime));
    }

    public void OnClosed()
    {
        StopClock();
        Closed?.Invoke(this, EventArgs.Empty);
    }
}
