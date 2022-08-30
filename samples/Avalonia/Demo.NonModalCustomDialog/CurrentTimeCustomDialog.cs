using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IView
{
    private readonly CurrentTimeDialog dialog = new();

    public object RefObj => this;

    event EventHandler IView.Loaded
    {
        add => dialog.Opened += value;
        remove => dialog.Opened -= value;
    }

    event EventHandler IView.Closed
    {
        add => dialog.Closed += value;
        remove => dialog.Closed -= value;
    }

    event EventHandler<CancelEventArgs> IView.Closing
    {
        add => dialog.Closing += value;
        remove => dialog.Closing -= value;
    }

    object? IView.DataContext
    {
        get => dialog.DataContext;
        set => dialog.DataContext = value;
    }

    public IView? Owner { get; set; }

    public Task<bool?> ShowDialogAsync()
    {
        if (Owner is not ViewWrapper w) throw new InvalidOperationException("{nameof(Owner)} must be set before calling {nameof(ShowDialogAsync)}");

        return dialog.ShowDialog<bool?>(w.Ref);
    }

    void IView.Show() => dialog.Show();

    public void Activate() => dialog.Activate();

    public void Close() => dialog.Close();

    public bool IsEnabled
    {
        get => dialog.IsEnabled;
        set => dialog.IsEnabled = value;
    }

    public bool ClosingConfirmed { get; set; }
}
