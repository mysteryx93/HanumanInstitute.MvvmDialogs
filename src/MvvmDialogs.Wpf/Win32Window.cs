using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Class describing a <see cref="IWin32Window"/> wrapper around a WPF window.
/// </summary>
internal class Win32Window : IWin32Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Win32Window"/> class.
    /// </summary>
    /// <param name="window">The WPF window to wrap.</param>
    public Win32Window(Window window) => Handle = new WindowInteropHelper(window).Handle;

    /// <inheritdoc />
    public IntPtr Handle { get; }
}
