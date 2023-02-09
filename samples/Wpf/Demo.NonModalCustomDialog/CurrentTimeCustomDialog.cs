using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.Wpf.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IView, IViewSync
{
    private readonly CurrentTimeDialog dialog = new();

    public object RefObj => this;

    public void Initialize(INotifyPropertyChanged viewModel, Type viewType)
    {
        ViewModel = viewModel;
    }

    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
    }

    public event EventHandler Closed
    {
        add => dialog.Closed += value;
        remove => dialog.Closed -= value;
    }

    public event EventHandler Loaded
    {
        add { }
        remove { }
    }

    public event EventHandler<CancelEventArgs> Closing
    {
        add { }
        remove { }
    }

    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)dialog.DataContext;
        set => dialog.DataContext = value;
    }

    public IView? Owner
    {
        get => dialog.Owner.AsWrapper();
        set => dialog.Owner = value.AsWrapper()?.Ref;
    }

    public Task ShowDialogAsync(IView owner) => UiExtensions.RunUiAsync(() => ShowDialog(owner));

    public void ShowDialog(IView owner)
    {
        dialog.Owner = owner.GetRef();
        dialog.ShowDialog();
    }

    public void Show(IView? owner) => dialog.Show();

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
