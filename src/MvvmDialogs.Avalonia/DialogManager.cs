using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// DialogManager for Avalonia.
/// </summary>
public class DialogManager : DialogManagerBase<Window>
{
    private readonly IDispatcher _dispatcher;

    /// <inheritdoc />
    public DialogManager(IViewLocator? viewLocator = null,
        IDialogFactory? dialogFactory = null,
        ILogger<DialogManager>? logger = null, IDispatcher? dispatcher = null) :
        base(viewLocator ?? new ViewLocatorBase(),
            dialogFactory ?? new DialogFactory(), logger)
    {
        _dispatcher = dispatcher ?? Dispatcher.UIThread;
    }

    /// <inheritdoc />
    protected override IWindow CreateWrapper(Window window) => window.AsWrapper();

    private static IEnumerable<Window> Windows =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows ?? Array.Empty<Window>();

    /// <inheritdoc />
    public override IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel) =>
        Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();

    /// <inheritdoc />
    protected override void Dispatch(Action action)
    {
        if (_dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            _dispatcher.Post(action, DispatcherPriority.Render);
        }
    }

    /// <inheritdoc />
    protected override Task<T> DispatchAsync<T>(Func<T> action) =>
        _dispatcher.CheckAccess() ? Task.FromResult(action()) : _dispatcher.InvokeAsync(action, DispatcherPriority.Render);
}
