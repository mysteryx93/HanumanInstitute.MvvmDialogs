using System.Collections;
using System.Runtime.CompilerServices;
using Avalonia.Interactivity;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <inheritdoc cref="INavigationManager"/>
public class NavigationManager : INotifyPropertyChanged, INavigationManager, IWeakEventSubscriber<EventArgs>
{
    // Keep observing closure after detachment without letting the host retain an abandoned navigation root.
    private static readonly WeakEvent<TopLevel, EventArgs> HostClosed = WeakEvent.Register<TopLevel>(
        static (top, handler) => top.Closed += handler,
        static (top, handler) => top.Closed -= handler);

    private readonly List<ViewNavigationWrapper> _stack = [];
    private readonly ViewClosingHandler? _closingHandler;
    private Control? _root;
    private TopLevel? _top;

    /// <summary>
    /// Creates a navigation manager with an optional close handler.
    /// </summary>
    /// <param name="closingHandler">The handler that confirms page close requests.</param>
    public NavigationManager(ViewClosingHandler? closingHandler)
    {
        _closingHandler = closingHandler;
        History = new ViewModelHistory(_stack);
        CancellableActions.RegistrationsChanged.Subscribe(CancellableActions.RegistrationSource, this);
    }

    /// <inheritdoc />
    public IReadOnlyList<INotifyPropertyChanged> History { get; }

    private sealed class ViewModelHistory(List<ViewNavigationWrapper> stack) : IReadOnlyList<INotifyPropertyChanged>
    {
        public int Count => stack.Count;
        public INotifyPropertyChanged this[int index] => stack[index].ViewModel;
        public IEnumerator<INotifyPropertyChanged> GetEnumerator() => stack.Select(x => x.ViewModel).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <inheritdoc />
    public void Launch(Control? customNavigationRoot = null)
    {
        var lifetime = Application.Current?.ApplicationLifetime;
        var root = lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime => customNavigationRoot as Window ?? new NavigationRootWindow(),
            ISingleViewApplicationLifetime => customNavigationRoot ?? new NavigationRoot(),
            _ => customNavigationRoot
        };
        if (ReferenceEquals(root, _root)) { return; }
        DetachHost();
        if (root == null) { return; }

        _root = root;
        root.DataContext = this;
        root.AttachedToVisualTree += RootAttached;
        root.DetachedFromVisualTree += RootDetached;
        SetTopLevel(TopLevel.GetTopLevel(root));
        if (lifetime is ISingleViewApplicationLifetime single) { single.MainView = root; }
        if (lifetime is IClassicDesktopStyleApplicationLifetime desktop) { desktop.MainWindow = (Window)root; }
    }

    private void RootAttached(object? sender, VisualTreeAttachmentEventArgs e) => SetTopLevel(TopLevel.GetTopLevel(_root));
    private void RootDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_top != null) { _top.BackRequested -= BackRequested; }
    }

    private void SetTopLevel(TopLevel? top)
    {
        if (_top != null)
        {
            _top.BackRequested -= BackRequested;
            HostClosed.Unsubscribe(_top, this);
        }
        _top = top;
        if (_top != null)
        {
            HostClosed.Subscribe(_top, this);
            _top.BackRequested += BackRequested;
        }
    }

    private void DetachHost()
    {
        if (_root == null) { return; }
        SetTopLevel(null);
        _root.AttachedToVisualTree -= RootAttached;
        _root.DetachedFromVisualTree -= RootDetached;
        if (ReferenceEquals(_root.DataContext, this)) { _root.DataContext = null; }
        _root = null;
        if (_stack.Count == 0) { CurrentView = null; }
        else { RemovePages(_stack.ToArray(), completed: null); }
    }

    private object BackScope => (object?)TopLevel.GetTopLevel(_root) ?? _root ?? (object?)CurrentView ?? this;

    private bool CanPop => _stack.Count > 1 && ReferenceEquals(CurrentView, _stack[^1].Ref);

    /// <inheritdoc />
    public bool CanGoBack => CanPop || CancellableActions.CanCancel(BackScope);

    void IWeakEventSubscriber<EventArgs>.OnEvent(object? sender, WeakEvent ev, EventArgs e)
    {
        if (ev == HostClosed)
        {
            try { DetachHost(); }
            catch (Exception error) { Dispatcher.UIThread.Post(ExceptionDispatchInfo.Capture(error).Throw); }
        }
        else { OnPropertyChanged(nameof(CanGoBack)); }
    }

    private async void BackRequested(object? sender, RoutedEventArgs e)
    {
        // Claim Back before the platform event returns. Dismiss popups after dispatch.
        if (!e.Handled && CanGoBack)
        {
            e.Handled = true;
            if (!CancellableActions.CancelLast(BackScope, post: true)) { await GoBackPageAsync(); }
        }
    }

    /// <inheritdoc />
    public Task<bool> GoBackAsync() =>
        CancellableActions.CancelLast(BackScope) ? Task.FromResult(true) : GoBackPageAsync();

    private Task<bool> GoBackPageAsync() => CanPop ? RequestCloseAsync(_stack[^1], back: true) : Task.FromResult(false);

    /// <inheritdoc />
    public UserControl? CurrentView
    {
        get => _currentView;
        set
        {
            if (value?.DataContext is INotifyPropertyChanged model)
            {
                var page = FindOpenView(model);
                if (page != null && page.Ref != value) { page.Ref = value; }
            }
            if (SetField(ref _currentView, value))
            {
                OnPropertyChanged(nameof(CurrentViewModel));
            }
            OnPropertyChanged(nameof(CanGoBack));
        }
    }
    private UserControl? _currentView;

    /// <inheritdoc />
    public INotifyPropertyChanged? CurrentViewModel => CurrentView?.DataContext as INotifyPropertyChanged;

    internal ViewNavigationWrapper? FindOpenView(INotifyPropertyChanged model) =>
        _stack.FindLast(x => ReferenceEquals(x.ViewModel, model));

    internal bool IsOpen(ViewNavigationWrapper page) => _stack.Contains(page);

    private bool OwnsModal(ViewNavigationWrapper page) => _stack.Any(x => ReferenceEquals(x.Owner, page));

    /// <inheritdoc />
    public UserControl? GetViewForViewModel(INotifyPropertyChanged viewModel) =>
        ReferenceEquals(CurrentViewModel, viewModel) ? CurrentView : FindOpenView(viewModel)?.Realize();

    private ViewNavigationWrapper CreatePage(INotifyPropertyChanged model, ViewDefinition definition)
    {
        var page = new ViewNavigationWrapper(this, _closingHandler);
        page.Initialize(model, definition);
        return page;
    }

    /// <inheritdoc />
    public void Show(INotifyPropertyChanged viewModel, ViewDefinition viewDef) =>
        Present(CreatePage(viewModel, viewDef), null);

    /// <inheritdoc />
    public Task ShowDialogAsync(INotifyPropertyChanged viewModel, ViewDefinition viewDef, INotifyPropertyChanged ownerViewModel) =>
        Present(
            CreatePage(viewModel, viewDef),
            FindOpenView(ownerViewModel) ??
            throw new InvalidOperationException("The dialog owner is not open."));

    internal Task Present(ViewNavigationWrapper page, IView? owner)
    {
        if (owner?.ViewModel != null && !(owner is ViewNavigationWrapper wrapper && IsOpen(wrapper)))
        {
            owner = FindOpenView(owner.ViewModel) ?? owner;
        }
        var previous = FindOpenView(page.ViewModel);
        if ((previous != null && (owner != null || previous.Completion != null)) || ReferenceEquals(page, owner))
        {
            throw new InvalidOperationException("Dialog is already shown.");
        }
        var view = page.Realize(previous?.Ref);
        if (previous != null)
        {
            var staysCovered = OwnsModal(previous);
            if (staysCovered) { _stack[_stack.IndexOf(previous)] = page; }
            else { _stack.Remove(previous); }
            foreach (var child in _stack.Where(x => ReferenceEquals(x.Owner, previous))) { child.Owner = page; }
            if (!ReferenceEquals(page, previous)) { previous.NotifyClosed(notifyModel: false); }
            if (staysCovered) { return Task.CompletedTask; }
        }
        page.Owner = owner;
        var completion = owner == null ? null : new TaskCompletionSource<bool>();
        page.Completion = completion;
        _stack.Add(page);
        try
        {
            CurrentView = view;
            if (IsOpen(page)) { page.NotifyLoaded(); }
        }
        catch
        {
            if (IsOpen(page)) { RemovePages([page], completed: null); }
            throw;
        }
        return completion?.Task ?? Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool Activate(INotifyPropertyChanged viewModel)
    {
        var page = FindOpenView(viewModel);
        if (page == null || OwnsModal(page)) { return false; }
        var alreadyLast = ReferenceEquals(page, _stack[^1]);
        if (alreadyLast && page.IsVisible) { return true; }

        var view = page.Realize();
        if (!alreadyLast)
        {
            _stack.Remove(page);
            _stack.Add(page);
        }
        CurrentView = view;
        if (IsOpen(page)) { page.NotifyLoaded(); }
        return true;
    }

    /// <inheritdoc />
    public void Close(INotifyPropertyChanged viewModel)
    {
        FindOpenView(viewModel)?.Close();
    }

    // Closing a foreground modal returns to its owner. Removing an owner also retires its modal children.
    private ViewNavigationWrapper[] PagesToClose(ViewNavigationWrapper page)
    {
        if (page.Completion == null && !OwnsModal(page)) { return [page]; }

        var removed = new HashSet<ViewNavigationWrapper>
        {
            page
        };
        // A close confirmation only removes the prompt and its own children.
        if (ReferenceEquals(page, _stack[^1]) && page.Owner is ViewNavigationWrapper owner &&
            !owner.IsConfirming && _stack.Contains(owner))
        {
            removed.UnionWith(_stack.Skip(_stack.IndexOf(owner) + 1));
        }
        foreach (var entry in _stack)
        {
            for (var parent = entry.Owner; parent is ViewNavigationWrapper ancestor; parent = ancestor.Owner)
            {
                if (removed.Contains(ancestor))
                {
                    removed.Add(entry);
                    break;
                }
            }
        }
        return _stack.Where(removed.Contains).Reverse().ToArray();
    }

    internal Task<bool> RequestCloseAsync(ViewNavigationWrapper request, bool back = false)
    {
        var page = IsOpen(request) ? request : FindOpenView(request.ViewModel);
        var view = request.Ref;
        if (page == null || (request != page && (view == null || !ReferenceEquals(view, page.Ref))))
        {
            return Task.FromResult(false);
        }
        var pending = page.PendingClose;
        if (pending != null && !pending.IsCompleted) { return pending; }
        return page.PendingClose = CloseConfirmedAsync();

        async Task<bool> CloseConfirmedAsync()
        {
            var removed = PagesToClose(page);
            try
            {
                if (request != page && request.ClosingHandler != null && !Equals(request.ClosingHandler, page.ClosingHandler) &&
                    !await request.ConfirmAsync())
                {
                    return false;
                }
                foreach (var entry in removed)
                {
                    if (!await entry.ConfirmAsync())
                    {
                        return false;
                    }
                }
            }
            catch (OperationCanceledException) when (!IsOpen(page))
            {
                // Retiring the page can cancel the modal dialog used to confirm its close.
                return false;
            }
            if (!IsOpen(page) || (back && (_stack.Count <= removed.Length || _stack[^1] != page)) ||
                !removed.SequenceEqual(PagesToClose(page))) { return false; }

            RemovePages(removed, page);
            if (request != page) { request.NotifyClosed(notifyModel: false); }
            return true;
        }
    }

    private void RemovePages(ViewNavigationWrapper[] removed, ViewNavigationWrapper? completed)
    {
        var next = removed.Length == _stack.Count || removed.Any(page => page.IsVisible)
            ? _stack.LastOrDefault(x => !removed.Contains(x))?.Realize()
            : CurrentView;
        foreach (var entry in removed) { _stack.Remove(entry); }
        Exception? failure = null;
        try { CurrentView = next; }
        catch (Exception error) { failure = error; }
        foreach (var entry in removed)
        {
            var completion = entry.Completion;
            entry.Completion = null;
            entry.Owner = null;
            try { entry.NotifyClosed(); }
            catch (Exception error)
            {
                completion?.TrySetException(error);
                failure ??= error;
            }
            finally
            {
                if (entry == completed) { completion?.TrySetResult(true); }
                else { completion?.TrySetCanceled(); }
            }
        }
        if (failure != null) { ExceptionDispatchInfo.Throw(failure); }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies observers of a property change.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new(propertyName));

    /// <summary>
    /// Updates a bound property when its value changes.
    /// </summary>
    /// <param name="field">The field holding the current value.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The name of the changed property.</param>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <returns>True if the value changed.</returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
