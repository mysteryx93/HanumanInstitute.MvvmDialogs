using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IView
{
    private readonly CurrentTimeDialog _dialog = new();

    public object RefObj => this;

    event EventHandler IView.Loaded
    {
        add => _dialog.Opened += value;
        remove => _dialog.Opened -= value;
    }

    event EventHandler IView.Closed
    {
        add => _dialog.Closed += value;
        remove => _dialog.Closed -= value;
    }

    event EventHandler<CancelEventArgs> IView.Closing
    {
        add => _dialog.Closing += value;
        remove => _dialog.Closing -= value;
    }

    object? IView.DataContext
    {
        get => _dialog.DataContext;
        set => _dialog.DataContext = value;
    }

    public IView? Owner { get; set; }

    public Task<bool?> ShowDialogAsync()
    {
        if (Owner is not ViewWrapper w) throw new InvalidOperationException("{nameof(Owner)} must be set before calling {nameof(ShowDialogAsync)}");

        return _dialog.ShowDialog<bool?>(w.Ref);
    }

    void IView.Show() => _dialog.Show();

    public void Activate() => _dialog.Activate();

    public void Close() => _dialog.Close();

    public bool IsEnabled
    {
        get => _dialog.IsEnabled;
        set => _dialog.IsEnabled = value;
    }

    public bool IsVisible => _dialog.IsEnabled;

    public bool ClosingConfirmed { get; set; }
}
