using Avalonia.Media;
using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// DialogManager for Avalonia.
/// </summary>
public class DialogManager : DialogManagerBase<ContentControl>
{
    private readonly NavigationManager? _navigationManager;
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
        base(viewLocator ?? new ViewLocatorBase(), dialogFactory ?? new DialogFactory(), logger)
    {
        _dispatcher = dispatcher ?? Dispatcher.UIThread;
        _useNavigation = Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime ||
                         (viewLocator is IViewLocatorNavigation navigation && navigation.ForceSinglePageNavigation);
        if (_useNavigation)
        {
            _navigationManager = new(View_Closing);
            _navigationManager.Launch(customNavigationRoot);
        }
    }

    /// <summary>
    /// If using single-page navigation mode, returns the navigation manager.
    /// </summary>
    public INavigationManager? NavigationManager => _navigationManager;

    // Awaits the close decision so navigation can recheck history before removing a page.
    internal Task ConfirmNavigationCloseAsync(IView dialog, CancelEventArgs e) =>
        ConfirmViewClosingAsync(dialog, e);

    /// <inheritdoc />
    protected override bool IsDesignMode => Design.IsDesignMode;

    /// <inheritdoc />
    public override IDisposable HandleDialogEvents(INotifyPropertyChanged viewModel, IView dialog) =>
        dialog is ViewNavigationWrapper ? HandleDialogEvents(viewModel, dialog, forwardLoadedAndClosed: false) : base.HandleDialogEvents(viewModel, dialog);

    /// <inheritdoc />
    protected override IView CreateWrapper(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        var wrapper = _useNavigation ? (IView)new ViewNavigationWrapper(_navigationManager!, View_Closing) : new ViewWrapper();
        wrapper.Initialize(viewModel, viewDef);
        return wrapper;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">The navigation view has no <see cref="INotifyPropertyChanged"/> data context.</exception>
    protected override IView AsWrapper(ContentControl view) =>
        view is Window w
            ? w.AsWrapper()
            : ((UserControl)view).AsWrapper(_navigationManager!, View_Closing) ??
              throw new ArgumentException("The navigation view requires an INotifyPropertyChanged data context.", nameof(view));

    private static IEnumerable<Window> Windows =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows ?? Array.Empty<Window>();

    /// <inheritdoc />
    public override IView? FindViewByViewModel(INotifyPropertyChanged viewModel)
    {
        if (_useNavigation)
        {
            return _navigationManager!.FindOpenView(viewModel);
        }
        return Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();
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

        var parent = new Window
        {
            Height = 1,
            Width = 1,
            WindowDecorations = WindowDecorations.None,
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
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        _dispatcher.Post(
            () =>
            {
                try
                {
                    tcs.SetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            },
            DispatcherPriority.Render);
        return tcs.Task;
    }
}
