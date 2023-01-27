using System;
using System.Reactive.Linq;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.CrossPlatform.ViewModels;

public class CurrentTimeViewModel : ViewModelBase, IModalDialogViewModel, ICloseable, IViewClosed
{
    public CurrentTimeViewModel()
    {
        Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(
            (_) =>
            {
                this.RaisePropertyChanged(nameof(CurrentTime));
            });
        Close = ReactiveCommand.Create(CloseImpl);
    }

    public DateTime CurrentTime => DateTime.Now;

    public bool? DialogResult { get; } = true;
    public event EventHandler? RequestClose;
    public event EventHandler? ViewClosed;

    public ICommand Close { get; }

    public void CloseImpl()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public void OnClosed() => ViewClosed?.Invoke(this, EventArgs.Empty);
}
