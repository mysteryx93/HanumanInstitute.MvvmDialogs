using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;
using Avalonia.Media;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// DialogManager for Avalonia.
/// </summary>
public class DialogManager : DialogManagerBase<ContentControl>
{
    private readonly INavigationManager? _navigationManager;
    private readonly IDispatcher _dispatcher;
    private readonly bool _useNavigation;

    /// <inheritdoc />
    public DialogManager(
        IViewLocator? viewLocator = null,
        IDialogFactory? dialogFactory = null,
        ILogger<DialogManager>? logger = null,
        IDispatcher? dispatcher = null,
        Control? customNavigationRoot = null)
        :
        base(
            viewLocator ?? new ViewLocatorBase(),
            dialogFactory ?? new DialogFactory(),
            logger)
    {
        _dispatcher = dispatcher ?? Dispatcher.UIThread;
        _useNavigation = Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime || viewLocator is IViewLocatorNavigation { ForceSinglePageNavigation: true };
        if (_useNavigation)
        {
            ForwardViewEvents = false;
            _navigationManager = new NavigationManager(View_Closing);
            _navigationManager.Launch(customNavigationRoot);
        }
    }

    /// <summary>
    /// If using single-page navigation mode, returns the navigation manager.
    /// </summary>
    public INavigationManager? NavigationManager => _navigationManager;

    /// <inheritdoc />
    protected override IView CreateWrapper(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        var wrapper = _useNavigation ?
            (IView)new ViewNavigationWrapper(_navigationManager!, View_Closing) :
            new ViewWrapper();
        wrapper.Initialize(viewModel, viewDef);
        return wrapper;
    }

    /// <inheritdoc />
    protected override IView AsWrapper(ContentControl view) =>
        view is Window w ? w.AsWrapper() : ((UserControl)view).AsWrapper(_navigationManager!, View_Closing);

    private static IEnumerable<Window> Windows =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows ?? Array.Empty<Window>();

    /// <inheritdoc />
    public override IView? FindViewByViewModel(INotifyPropertyChanged viewModel)
    {
        if (_useNavigation)
        {
            return _navigationManager!.GetViewForViewModel(viewModel).AsWrapper(_navigationManager, View_Closing);
        }
        else
        {
            return Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();
        }
    }

    /// <inheritdoc />
    public override IView? GetMainWindow()
    {
        if (_useNavigation)
        {
            return null;
        }

        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow.AsWrapper();
    }

    /// <inheritdoc />
    public override IView? GetDummyWindow()
    {
        if (_useNavigation)
        {
            return null;
        }

        var parent = new Window()
        {
            Height = 1,
            Width = 1,
            SystemDecorations = SystemDecorations.None,
            ShowInTaskbar = false,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Background = Brushes.Transparent
        };
        parent.Show();
        return parent.AsWrapper();
    }

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
        //_dispatcher.CheckAccess() ? Task.FromResult(action()) : _dispatcher.InvokeAsync(action, DispatcherPriority.Render);
        _dispatcher.CheckAccess() ? Task.FromResult(action()) : DispatchWithResult(action);

    /// <summary>
    /// Work-around for missing interface member in Avalonia v11-preview1.
    /// </summary>
    private Task<T> DispatchWithResult<T>(Func<T> action)
    {
        var tcs = new TaskCompletionSource<T>();
        _ = _dispatcher.InvokeAsync(
            () =>
            {
                tcs.SetResult(action());
            },
            DispatcherPriority.Render);
        return tcs.Task;
    }
}
