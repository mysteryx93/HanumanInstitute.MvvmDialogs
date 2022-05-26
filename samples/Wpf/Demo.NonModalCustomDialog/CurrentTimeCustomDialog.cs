using System;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IWindow, IWindowSync
{
    private readonly CurrentTimeDialog dialog = new();

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

    public IWindow? Owner {
        get => dialog.Owner.AsWrapper();
        set => dialog.Owner = value.AsWrapper()?.Ref;
    }

    Task<bool?> IWindow.ShowDialogAsync() => dialog.RunUiAsync(() => dialog.ShowDialog());

    public bool? ShowDialog() => dialog.ShowDialog();

    void IWindow.Show() => dialog.Show();

    public void Activate() => dialog.Activate();

    public void Close() => dialog.Close();
}
