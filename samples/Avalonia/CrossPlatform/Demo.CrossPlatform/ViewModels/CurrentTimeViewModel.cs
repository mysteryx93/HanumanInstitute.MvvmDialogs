using System.Reactive.Linq;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace Demo.CrossPlatform.ViewModels;

public class CurrentTimeViewModel : ViewModelBase, IModalDialogViewModel, ICloseable, IViewClosed
{
    private readonly IDisposable _clock;

    public CurrentTimeViewModel()
    {
        _clock = ObservableExtensions.Subscribe(
            Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)),
            _ =>
            {
                this.RaisePropertyChanged(nameof(CurrentTime));
            });
        Close = ReactiveCommand.Create(CloseImpl);
    }

    public DateTime CurrentTime => DateTime.Now;

    public bool? DialogResult { get; } = true;
    public event EventHandler? RequestClose;
    public event EventHandler? Closed;

    public ReactiveCommand<RxVoid, RxVoid> Close { get; }

    private void CloseImpl()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public void OnClosed()
    {
        _clock.Dispose();
        Closed?.Invoke(this, EventArgs.Empty);
    }
}
