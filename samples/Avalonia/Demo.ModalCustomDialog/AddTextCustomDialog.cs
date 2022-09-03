using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.ModalCustomDialog;

public class AddTextCustomDialog : IView
{
    public object RefObj => this;

    private readonly AddTextDialog dialog = new();

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

    Task<bool?> IView.ShowDialogAsync() => dialog.ShowDialog<bool?>(Owner.AsWrapper()!.Ref);

    void IView.Show() => dialog.Show();

    public void Activate() => dialog.Activate();

    public void Close() => dialog.Close();

    public bool IsEnabled
    {
        get => dialog.IsEnabled;
        set => dialog.IsEnabled = value;
    }

    public bool IsVisible => dialog.IsVisible;

    public bool ClosingConfirmed { get; set; }
}
