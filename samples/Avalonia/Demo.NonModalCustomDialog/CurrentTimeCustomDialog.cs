using System;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IWindow
{
    private readonly CurrentTimeDialog dialog = new();

    public object RefObj => this;

    event EventHandler IWindow.Closed
    {
        add => dialog.Closed += value;
        remove => dialog.Closed -= value;
    }

    object? IWindow.DataContext
    {
        get => dialog.DataContext;
        set => dialog.DataContext = value;
    }

    public IWindow? Owner { get; set; }

    public Task<bool?> ShowDialogAsync()
    {
        if (Owner is not WindowWrapper w) throw new InvalidOperationException("{nameof(Owner)} must be set before calling {nameof(ShowDialogAsync)}");

        return dialog.ShowDialog<bool?>(w.Ref);
    }

    void IWindow.Show() => dialog.Show();

    public void Activate() => dialog.Activate();

    public void Close() => dialog.Close();
}
