// ReSharper disable VirtualMemberCallInConstructor

using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Class wrapping an instance of Avalonia <see cref="Window"/> within <see cref="IView"/>.
/// </summary>
/// <seealso cref="IView" />
public class ViewWrapper : IView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewWrapper"/> class.
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="viewType"></param>
    public void Initialize(INotifyPropertyChanged viewModel, Type viewType)
    {
        Ref = (Window)Activator.CreateInstance(viewType)!;
        ViewModel = viewModel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewWrapper"/> class.
    /// </summary>
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        Ref = (Window)view;
        ViewModel = viewModel;
    }

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public Window Ref { get; private set; } = default!;

    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public object RefObj => Ref;

    /// <summary>
    /// Fired when the window is loaded.
    /// </summary>
    public event EventHandler? Loaded
    {
        add => Ref.Opened += value;
        remove => Ref.Opened -= value;
    }

    /// <summary>
    /// Fired when the window is closing.
    /// </summary>
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

    /// <summary>
    /// Fired when the window is closed.
    /// </summary>
    public event EventHandler? Closed
    {
        add => Ref.Closed += value;
        remove => Ref.Closed -= value;
    }

    /// <inheritdoc />
    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)Ref.DataContext!;
        set => Ref.DataContext = value;
    }

    /// <inheritdoc />
    public Task ShowDialogAsync(IView owner)
    {
        return Ref.ShowDialog<bool?>((Window)owner.RefObj);
    }

    /// <inheritdoc />
    public void Show(IView? owner)
    {
        if (owner?.RefObj != null)
        {
            Ref.Show((Window)owner.RefObj);
        }
        else
        {
            Ref.Show();
        }
    }

    /// <inheritdoc />
    public void Activate() => Ref.Activate();

    /// <inheritdoc />
    public void Close() => Ref.Close();

    /// <inheritdoc />
    public bool IsEnabled
    {
        get => Ref.IsEnabled;
        set => Ref.IsEnabled = value;
    }

    /// <inheritdoc />
    public bool IsVisible => Ref.IsVisible;

    /// <inheritdoc />    
    public bool ClosingConfirmed { get; set; }
}
