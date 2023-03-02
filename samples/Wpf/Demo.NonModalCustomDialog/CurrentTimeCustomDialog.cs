using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.Wpf.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IView, IViewSync
{
    private readonly CurrentTimeDialog _dialog = new();

    public object RefObj => this;

    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        ViewModel = viewModel;
    }

    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
    }

    public event EventHandler Closed
    {
        add => _dialog.Closed += value;
        remove => _dialog.Closed -= value;
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
        get => (INotifyPropertyChanged)_dialog.DataContext;
        set => _dialog.DataContext = value;
    }

    public IView? Owner
    {
        get => _dialog.Owner.AsWrapper();
        set => _dialog.Owner = value.AsWrapper()?.Ref;
    }

    public Task ShowDialogAsync(IView owner) => UiExtensions.RunUiAsync(() => ShowDialog(owner));

    public void ShowDialog(IView owner)
    {
        _dialog.Owner = owner.GetRef();
        _dialog.ShowDialog();
    }

    public void Show(IView? owner)
    {
        _dialog.Owner = owner.GetRef();
        _dialog.Show();
    }

    public void Activate() => _dialog.Activate();

    public void Close() => _dialog.Close();

    public bool IsEnabled
    {
        get => _dialog.IsEnabled;
        set => _dialog.IsEnabled = value;
    }

    public bool IsVisible => _dialog.IsVisible;

    public bool ClosingConfirmed { get; set; }
}
