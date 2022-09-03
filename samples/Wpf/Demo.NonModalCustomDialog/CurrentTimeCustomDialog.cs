using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IView, IViewSync
{
    private readonly CurrentTimeDialog dialog = new();

    public object RefObj => this;

    event EventHandler IView.Closed
    {
        add => dialog.Closed += value;
        remove => dialog.Closed -= value;
    }

    event EventHandler IView.Loaded
    {
        add { }
        remove { }
    }

    event EventHandler<CancelEventArgs> IView.Closing
    {
        add { }
        remove { }
    }

    object? IView.DataContext
    {
        get => dialog.DataContext;
        set => dialog.DataContext = value;
    }

    public IView? Owner
    {
        get => dialog.Owner.AsWrapper();
        set => dialog.Owner = value.AsWrapper()?.Ref;
    }

    Task<bool?> IView.ShowDialogAsync() => UiExtensions.RunUiAsync(() => dialog.ShowDialog());

    public bool? ShowDialog() => dialog.ShowDialog();

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
