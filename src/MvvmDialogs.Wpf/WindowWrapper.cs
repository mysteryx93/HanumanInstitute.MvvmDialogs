using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class wrapping an instance of WPF <see cref="Window"/> within <see cref="IWindow"/>.
/// </summary>
/// <seealso cref="IWindow" />
public class WindowWrapper : IWindow, IWindowSync
{
    /// <summary>
    /// Gets the Window reference held by this class.
    /// </summary>
    public Window Ref { get; private set; }

    /// <summary>
    /// Returns a IWin32Window class that can be used for API calls.
    /// </summary>
    public IWin32Window Win32Window => new Win32Window(Ref);

    public event EventHandler? Closed
    {
        add => Ref.Closed += value;
        remove => Ref.Closed -= value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowWrapper"/> class.
    /// </summary>
    /// <param name="window">The window.</param>
    public WindowWrapper(Window window) =>
        this.Ref = window ?? throw new ArgumentNullException(nameof(window));

    /// <inheritdoc />
    public object? DataContext
    {
        get => Ref.DataContext;
        set => Ref.DataContext = value;
    }

    /// <inheritdoc />
    public IWindow? Owner
    {
        get => Ref.Owner.AsWrapper();
        set =>
            Ref.Owner = value switch
            {
                null => null,
                WindowWrapper w => w.Ref,
                _ => throw new ArgumentException($"Owner must be of type {typeof(WindowWrapper).FullName}")
            };
    }

    /// <inheritdoc />
    public Task<bool?> ShowDialogAsync() => Ref.RunUiAsync(ShowDialog);

    /// <inheritdoc />
    public bool? ShowDialog() => Ref.ShowDialog();

    /// <inheritdoc />
    public void Show() => Ref.Show();

    /// <inheritdoc />
    public void Activate() => Ref.Activate();

    /// <inheritdoc />
    public void Close() => Ref.Close();
}
