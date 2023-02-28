using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.ModalCustomDialog;

public class AddTextCustomDialog : IView
{
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        ViewModel = viewModel;
    }

    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
    }

    public Type ViewType { get; set; } = default!;

    public object RefObj => this;

    public AddTextDialog Ref { get; } = new();

    public event EventHandler Loaded
    {
        add => Ref.Opened += value;
        remove => Ref.Opened -= value;
    }

    public event EventHandler Closed
    {
        add => Ref.Closed += value;
        remove => Ref.Closed -= value;
    }

    public event EventHandler<CancelEventArgs>? Closing
    {
        add
        {
            if (value != null)
            {
                var handler = new EventHandler<WindowClosingEventArgs>(value.Invoke);
                _closingHandlers.Add(value, handler);
                Ref.Closing += handler;
            }
        }
        remove
        {
            if (value != null)
            {
                Ref.Closing += _closingHandlers[value];
                _closingHandlers.Remove(value);
            }
        }
    }
    private readonly Dictionary<EventHandler<CancelEventArgs>, EventHandler<WindowClosingEventArgs>> _closingHandlers = new();

    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)Ref.DataContext!;
        set => Ref.DataContext = value;
    }

    public INotifyPropertyChanged? Owner { get; set; }

    public Task ShowDialogAsync(IView owner) => Ref.ShowDialog<bool?>((Window)owner.RefObj!);

    public void Show(IView? owner) => Ref.Show((Window)owner!.RefObj);

    public void Activate() => Ref.Activate();

    public void Close() => Ref.Close();

    public bool IsEnabled
    {
        get => Ref.IsEnabled;
        set => Ref.IsEnabled = value;
    }

    public bool IsVisible => Ref.IsVisible;

    public bool ClosingConfirmed { get; set; }
}
