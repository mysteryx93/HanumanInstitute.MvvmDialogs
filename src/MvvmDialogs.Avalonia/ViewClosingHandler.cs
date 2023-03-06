using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// A handler for the Closing event. Note that the Closing event is unsupported by the <see cref="ViewNavigationWrapper"/> and we thus support a single listener.
/// </summary>
public delegate void ViewClosingHandler(IView dialog, CancelEventArgs e);
