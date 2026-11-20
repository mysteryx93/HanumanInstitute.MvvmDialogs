// ReSharper disable MemberCanBePrivate.Global

using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using DialogHostAvalonia;
using FluentAvalonia.Styling;
using HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;
using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
using HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;
using Microsoft.Extensions.Logging;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class NavigationTests : IAsyncDisposable
{
    private HeadlessSession _ui;

    private HeadlessSession Ui => _ui ??= new();

    /// <summary>
    /// Releases the dispatcher session owned by this test instance.
    /// </summary>
    public ValueTask DisposeAsync() => _ui?.DisposeAsync() ?? ValueTask.CompletedTask;

    public IDialogService DialogService => field ??= new DialogService(DialogManager);

    public DialogManager DialogManager => field ??= new(
        new ViewLocatorBase { ForceSinglePageNavigation = true },
        dispatcher: _ui == null ? new ImmediateDispatcher() : null);

    public INavigationManager NavigationManager => DialogManager.NavigationManager!;

    [Fact]
    public void Constructor_CurrentViewNull()
    {
        _ = DialogService;

        Assert.Null(NavigationManager.CurrentView);
    }

    [Fact]
    public void Show_First_CurrentViewSet()
    {
        var vm = new FirstViewModel();

        DialogService.Show(null, vm);

        Assert.NotNull(NavigationManager.CurrentView);
        Assert.Equal(vm, NavigationManager.CurrentView.DataContext);
    }

    [Fact]
    public void Show_Second_UpdatesHistoryAndCurrentView()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);

        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm1, NavigationManager.History[0]);
        Assert.Same(vm2, NavigationManager.History[1]);
        Assert.Same(vm2, NavigationManager.CurrentViewModel);
    }

    [Fact]
    public void Show_FirstSecondFirst_HistoryContainsSecondFirst()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        DialogService.Show(null, vm1);

        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm2, NavigationManager.History[0]);
        Assert.Same(vm1, NavigationManager.History[1]);
    }

    [Fact]
    public void Show_CloseSecond_CurrentViewSet()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        vm2.OnRequestClose();

        Assert.NotNull(NavigationManager.CurrentView);
        Assert.Equal(vm1, NavigationManager.CurrentView.DataContext);
    }

    [Fact]
    public async Task Show_SecondAndGarbageCollect_FirstReleased()
    {
        var view1 = Act();

        [MethodImpl(MethodImplOptions.NoInlining)]
        WeakReference Act()
        {
            var vm1 = new FirstViewModel();
            var vm2 = new SecondViewModel();
            DialogService.Show(null, vm1);
            var result = new WeakReference(NavigationManager.CurrentView);
            DialogService.Show(null, vm2);
            return result;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        await Task.Delay(100, TestContext.Current.CancellationToken);
        Assert.False(view1.IsAlive);
    }

    [Fact]
    public void ShowDialogAsync_SecondAndGarbageCollect_FirstReleased()
    {
        var view1 = Act();

        [MethodImpl(MethodImplOptions.NoInlining)]
        WeakReference Act()
        {
            var vm1 = new FirstViewModel();
            var vm2 = new SecondViewModel();
            DialogService.Show(null, vm1);
            var result = new WeakReference(NavigationManager.CurrentView);
            _ = DialogService.ShowDialogAsync(vm1, vm2);
            return result;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(view1.IsAlive);
    }

    [Fact]
    public void Activate_NotShown_DoNothing()
    {
        var vm1 = new FirstViewModel();

        vm1.OnRequestActivate();

        Assert.Null(NavigationManager.CurrentView);
    }

    [Fact]
    public void Activate_AlreadyActive_CurrentViewRemainsSame()
    {
        var vm1 = new FirstViewModel();

        DialogService.Show(null, vm1);
        vm1.OnRequestActivate();

        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        Assert.Single(NavigationManager.History);
    }

    [Fact]
    public void Activate_FirstSecondFirst_CurrentViewFirst()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        vm1.OnRequestActivate();

        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm2, NavigationManager.History[0]);
        Assert.Same(vm1, NavigationManager.History[1]);
    }

    [Fact]
    public void Activate_LastPageIsCoveredByAssignedView_RestoresLastPage()
    {
        var first = new FirstViewModel();
        var second = new SecondViewModel();
        DialogService.Show(null, first);
        var firstView = NavigationManager.CurrentView;
        DialogService.Show(null, second);
        NavigationManager.CurrentView = firstView;

        var activated = NavigationManager.Activate(second);

        Assert.True(activated);
        Assert.Same(second, NavigationManager.CurrentViewModel);
        Assert.True(NavigationManager.CanGoBack);
    }

    [Fact]
    public async Task Activate_ModalOwner_LeavesModalInFront()
    {
        var owner = new FirstViewModel();
        var modal = new SecondViewModel();
        DialogService.Show(null, owner);
        var pending = DialogService.ShowDialogAsync(owner, modal);

        NavigationManager.Activate(owner);

        Assert.Same(modal, NavigationManager.CurrentViewModel);
        Assert.True(await NavigationManager.GoBackAsync());
        await pending.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        Assert.Same(owner, NavigationManager.CurrentViewModel);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task ShowDialogAsync_Second_ReturnsValue(bool? dialogResult)
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel
        {
            DialogResult = dialogResult
        };

        DialogService.Show(null, vm1);
        var task = DialogService.ShowDialogAsync(vm1, vm2);
        vm2.OnRequestClose();
        var result = await task;

        Assert.Equal(dialogResult, result);
    }

    [Fact]
    public Task ShowDialogAsync_Second_WaitUntilClosed() =>
        Ui.Run(async () =>
        {
            var vm1 = new FirstViewModel();
            var vm2 = new SecondViewModel();

            DialogService.Show(null, vm1);
            var task = DialogService.ShowDialogAsync(vm1, vm2);

            await Task.Delay(10, TestContext.Current.CancellationToken);
            Assert.False(task.IsCompleted);
            vm2.OnRequestClose();
            await Task.Delay(10, TestContext.Current.CancellationToken);
            Assert.True(task.IsCompleted);
            Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        });

    [Fact]
    public Task ShowDialogAsync_RecursiveDialogs_WaitUntilClosed() =>
        Ui.Run(async () =>
        {
            var vm1 = new FirstViewModel();
            var vm2 = new SecondViewModel();
            var vm3 = new ThirdViewModel();

            DialogService.Show(null, vm1);
            var t1 = DialogService.ShowDialogAsync(vm1, vm2);
            var t2 = DialogService.ShowDialogAsync(vm2, vm3);

            await Task.Delay(10, TestContext.Current.CancellationToken);
            Assert.False(t1.IsCompleted);
            Assert.False(t2.IsCompleted);
            vm3.OnRequestClose();
            vm2.OnRequestClose();
            await Task.Delay(10, TestContext.Current.CancellationToken);
            Assert.True(t1.IsCompleted);
            Assert.True(t2.IsCompleted);
            Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        });

    [Fact]
    public void ShowDialogAsync_Second_AddToHistory()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        var _ = DialogService.ShowDialogAsync(vm1, vm2);

        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm1, NavigationManager.History[0]);
        Assert.Same(vm2, NavigationManager.History[1]);
    }

    [Fact]
    public async Task ShowDialogAsync_OwnerFirst_ShowFirstAfterClose()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        var vm3 = new ThirdViewModel();

        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        var task = DialogService.ShowDialogAsync(vm1, vm3);
        vm3.OnRequestClose();
        await task;

        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        Assert.Single(NavigationManager.History);
        Assert.Same(vm1, NavigationManager.History[0]);
    }

    [Fact]
    public async Task Show_AboveModal_PreservesCoveredEditor()
    {
        var owner = new FirstViewModel();
        var modal = new SecondViewModel();
        var editor = new ThirdViewModel();
        DialogService.Show(null, owner);
        _ = DialogService.ShowDialogAsync(owner, modal);
        DialogService.Show(null, editor);

        DialogService.Show(null, new FirstViewModel());
        var wentBack = await NavigationManager.GoBackAsync();

        Assert.True(wentBack);
        Assert.Same(editor, NavigationManager.CurrentViewModel);
        Assert.Equal(
            new INotifyPropertyChanged[]
            {
                owner, modal, editor
            },
            NavigationManager.History);
    }

    [Fact]
    public async Task ShowDialogAsync_AlreadyOpenModeless_RejectsDuplicate()
    {
        var owner = new FirstViewModel();
        var editor = new SecondViewModel();
        DialogService.Show(null, editor);
        DialogService.Show(null, owner);

        var act = () => DialogService.ShowDialogAsync(owner, editor).WaitAsync(TimeSpan.FromSeconds(1));

        await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal(
            new INotifyPropertyChanged[]
            {
                editor, owner
            },
            NavigationManager.History);
        Assert.Same(owner, NavigationManager.CurrentViewModel);
    }

    [Fact]
    public void Close_Normal_ClosingClosedRaised()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        var _ = DialogService.ShowDialogAsync(vm1, vm2);
        vm2.OnRequestClose();

        Assert.Equal(1, vm2.ClosingRaised);
        Assert.Equal(0, vm2.ClosingAsyncRaised);
        Assert.Equal(1, vm2.ClosedRaised);
    }

    [Fact]
    public Task Close_CancelledAndReset_ClosedRaised() =>
        Ui.Run(async () =>
        {
            var vm1 = new FirstViewModel();
            var vm2 = new SecondViewModel
            {
                ClosingCancel = true,
                ClosingAsyncCancel = false
            };

            DialogService.Show(null, vm1);
            var _ = DialogService.ShowDialogAsync(vm1, vm2);
            vm2.OnRequestClose();

            await Task.Delay(1, TestContext.Current.CancellationToken); // There's Task.Yield() in View_Closing
            Assert.Equal(1, vm2.ClosingRaised);
            Assert.Equal(1, vm2.ClosingAsyncRaised);
            Assert.Equal(1, vm2.ClosedRaised);
        });

    [Fact]
    public Task Close_3Times_RaiseClosingOnce() =>
        Ui.Run(async () =>
        {
            var vm1 = new FirstViewModel();
            var vm2 = new SecondViewModel();
            vm2.ClosingCancel = true;
            vm2.ClosingAsyncCancel = true;

            DialogService.Show(null, vm1);
            var _ = DialogService.ShowDialogAsync(vm1, vm2);
            vm2.OnRequestClose();
            vm2.OnRequestClose();
            vm2.OnRequestClose();

            await Task.Delay(1, TestContext.Current.CancellationToken); // There's Task.Yield() in View_Closing
            Assert.Equal(1, vm2.ClosingRaised);
            Assert.Equal(1, vm2.ClosingAsyncRaised);
            Assert.Equal(0, vm2.ClosedRaised);
        });

    [Fact]
    public void ShowDialogAsync_SynchronousContinuation_ShowsAgain()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        Task second = null;

        DialogService.Show(null, vm1);
        var first = DialogService.ShowDialogAsync(vm1, vm2);
        first.ContinueWith(
            _ => second = DialogService.ShowDialogAsync(vm1, vm2),
            TaskContinuationOptions.ExecuteSynchronously);
        vm2.OnRequestClose();

        Assert.NotNull(second);
        Assert.False(second.IsCompleted);
        Assert.Same(vm2, NavigationManager.CurrentView?.DataContext);
    }

    [Fact]
    public void GetViewForViewModel_OtherInstance_KeepsVisibleDataContext()
    {
        var shown = new FirstViewModel();
        var other = new FirstViewModel();

        DialogService.Show(null, shown);
        var visible = NavigationManager.CurrentView;
        _ = NavigationManager.GetViewForViewModel(other);

        Assert.Same(shown, visible!.DataContext);
        Assert.Same(visible, NavigationManager.CurrentView);
    }

    [Fact]
    public void GetViewForViewModel_StandaloneCacheViewCollected_ReusesFactory()
    {
        var cache = new ViewCache();
        var model = new FirstViewModel();
        var created = 0;
        var first = CreateView();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(first.IsAlive);

        var recreated = cache.GetViewForViewModel(model);

        Assert.Same(model, recreated?.DataContext);
        Assert.Equal(2, created);

        [MethodImpl(MethodImplOptions.NoInlining)]
        WeakReference CreateView() => new(
            cache.GetView(
                model,
                new(
                    typeof(FirstView),
                    () =>
                    {
                        created++;
                        return new FirstView();
                    })));
    }

    [Fact]
    public Task GetView_WorkerThread_Throws() =>
        Ui.Run(async () =>
        {
            var cache = new ViewCache();
            var viewModel = new FirstViewModel();
            var definition = new ViewDefinition(typeof(FirstView), () => new FirstView());
            cache.GetView(viewModel, definition);

            var act = () => Task.Run(() => cache.GetView(viewModel, definition));

            await Assert.ThrowsAsync<InvalidOperationException>(act);
        });

    [Fact]
    public void Close_OnlyPage_ClearsCurrentView()
    {
        var vm = new FirstViewModel();

        DialogService.Show(null, vm);
        vm.OnRequestClose();

        Assert.Null(NavigationManager.CurrentView);
        Assert.Empty(NavigationManager.History);
    }

    [Fact]
    public async Task GoBackAsync_RootPage_Stays()
    {
        var vm = new FirstViewModel();

        DialogService.Show(null, vm);
        var went = await NavigationManager.GoBackAsync();

        Assert.False(went);
        Assert.False(NavigationManager.CanGoBack);
        Assert.Same(vm, NavigationManager.CurrentView?.DataContext);
    }

    [Fact]
    public Task BackRequested_BareRoot_LeavesEventUnhandled() =>
        Ui.Run(() =>
        {
            var window = OpenNavigationWindow();
            var viewModel = new FirstViewModel();
            var args = new RoutedEventArgs(TopLevel.BackRequestedEvent);
            DialogService.Show(null, viewModel);

            window.RaiseEvent(args);

            Assert.False(args.Handled);
            Assert.Same(viewModel, NavigationManager.CurrentViewModel);
        });

    [Fact]
    public Task BackRequested_ChildPage_PopsChild() =>
        Ui.Run(() =>
        {
            var window = OpenNavigationWindow();
            var root = new FirstViewModel();
            var args = new RoutedEventArgs(TopLevel.BackRequestedEvent);
            DialogService.Show(null, root);
            DialogService.Show(null, new SecondViewModel());

            window.RaiseEvent(args);

            Assert.True(args.Handled);
            Assert.Same(root, NavigationManager.CurrentViewModel);
            Assert.Single(NavigationManager.History);
        });

    [Fact]
    public Task BackRequested_OwnedDialog_ConsumesBackBeforePoppingPage() =>
        Ui.Run(async () =>
        {
            var window = OpenNavigationWindow();
            var child = new SecondViewModel();
            var args = new RoutedEventArgs(TopLevel.BackRequestedEvent);
            var dismissed = new TaskCompletionSource<bool>();
            try
            {
                DialogService.Show(null, new FirstViewModel());
                DialogService.Show(null, child);
                var dialog = CancellableActions.RunAsync(() => dismissed.Task, () => dismissed.SetResult(true), window);

                window.RaiseEvent(args);
                Dispatcher.UIThread.RunJobs();
                var result = await dialog.WaitAsync(TimeSpan.FromSeconds(1));

                Assert.True(args.Handled);
                Assert.True(result);
                Assert.Same(child, NavigationManager.CurrentViewModel);
            }
            finally
            {
                dismissed.TrySetResult(false);
            }
        });

    [Fact]
    public Task BackRequested_DialogInAnotherWindow_LeavesRootUnhandled() =>
        Ui.Run(async () =>
        {
            var window = OpenNavigationWindow();
            var other = Ui.ShowWindow(new());
            var args = new RoutedEventArgs(TopLevel.BackRequestedEvent);
            var dismissed = new TaskCompletionSource<bool>();
            Dispatcher.UIThread.RunJobs();
            var dialog = CancellableActions.RunAsync(() => dismissed.Task, () => dismissed.TrySetResult(true), other);
            try
            {
                DialogService.Show(null, new FirstViewModel());

                window.RaiseEvent(args);

                Assert.False(args.Handled);
                Assert.False(NavigationManager.CanGoBack);
            }
            finally
            {
                dismissed.TrySetResult(false);
                await dialog;
            }
        });

    [Fact]
    public Task GoBackAsync_DeferredApproval_WaitsForPageToClose() =>
        Ui.Run(async () =>
        {
            var root = new FirstViewModel();
            var child = new SecondViewModel
            {
                ClosingCancel = true,
                ClosingAsyncCancel = false
            };
            DialogService.Show(null, root);
            DialogService.Show(null, child);

            var closed = await NavigationManager.GoBackAsync();

            Assert.True(closed);
            Assert.Same(root, NavigationManager.CurrentViewModel);
        });

    [Fact]
    public Task GoBackAsync_RepeatedDuringConfirmation_SharesDecision() =>
        Ui.Run(async () =>
        {
            var child = new SecondViewModel
            {
                ClosingCancel = true,
                ClosingAsyncCancel = false
            };
            DialogService.Show(null, new FirstViewModel());
            DialogService.Show(null, child);

            var results = await Task.WhenAll(NavigationManager.GoBackAsync(), NavigationManager.GoBackAsync());

            Assert.All(results, result => Assert.True(result));
            Assert.Equal(1, child.ClosingAsyncRaised);
            Assert.Single(NavigationManager.History);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Close_EarlierPage_DisplaysRemainingPage(bool displayEarlierPage)
    {
        var first = new FirstViewModel();
        var second = new SecondViewModel();
        DialogService.Show(null, first);
        var earlierView = NavigationManager.CurrentView;
        DialogService.Show(null, second);
        if (displayEarlierPage) { NavigationManager.CurrentView = earlierView; }

        first.OnRequestClose();

        Assert.Same(second, NavigationManager.CurrentViewModel);
        Assert.Same(second, Assert.Single(NavigationManager.History));
        Assert.Equal(0, second.ClosedRaised);
    }

    [Fact]
    public void Show_ViewCreationThrows_PreservesCurrentPageAndHistory()
    {
        var root = new FirstViewModel();
        var child = new SecondViewModel();
        var definition = new ViewDefinition(typeof(SecondView), () => throw new InvalidOperationException());
        DialogService.Show(null, root);
        var act = () => NavigationManager.Show(child, definition);

        Assert.Throws<InvalidOperationException>(act);
        Assert.Same(root, Assert.Single(NavigationManager.History));
        Assert.Same(root, NavigationManager.CurrentView?.DataContext);
    }

    [Fact]
    public void Close_ReshownViewModel_NotifiesOnce()
    {
        var viewModel = new FirstViewModel();
        DialogService.Show(null, viewModel);
        DialogService.Show(null, viewModel);

        viewModel.OnRequestClose();

        Assert.Equal(1, viewModel.ClosedCount);
    }

    [Fact]
    public Task Launch_RootReplaced_RetiresPreviousStack() =>
        Ui.Run(async () =>
        {
            var previous = new NavigationRootWindow();
            var replacement = new Window();
            var owner = new FirstViewModel();
            NavigationManager.Launch(previous);
            DialogService.Show(null, owner);
            var modal = DialogService.ShowDialogAsync(owner, new SecondViewModel());

            NavigationManager.Launch(replacement);

            Assert.Empty(NavigationManager.History);
            Assert.Null(previous.DataContext);
            Assert.Null(NavigationManager.CurrentView);
            Assert.Same(NavigationManager, replacement.DataContext);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => modal.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken));
        });

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public Task HostClose_WithUserControlRoot_CancelsModal(bool waitForLoaded, bool unloadFirst) =>
        Ui.Run(async () =>
        {
            var root = new NavigationRoot();
            var window = new Window
            {
                Content = root
            };
            var owner = new FirstViewModel();
            NavigationManager.Launch(root);
            Ui.ShowWindow(window);
            if (waitForLoaded) { Dispatcher.UIThread.RunJobs(); }
            DialogService.Show(null, owner);
            var modal = DialogService.ShowDialogAsync(owner, new SecondViewModel());
            if (unloadFirst)
            {
                window.Content = null;
                Dispatcher.UIThread.RunJobs();
                Assert.False(root.IsLoaded);
                Assert.False(modal.IsCompleted);
            }

            window.Close();

            Assert.Empty(NavigationManager.History);
            Assert.Null(root.DataContext);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => modal.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken));
        });

    [Fact]
    public void GetViewForViewModel_VisibleInstanceOfRepeatedType_ReturnsDisplayedView()
    {
        var visible = new FirstViewModel();
        DialogService.Show(null, new FirstViewModel());
        DialogService.Show(null, visible);

        var view = NavigationManager.GetViewForViewModel(visible);

        Assert.Same(NavigationManager.CurrentView, view);
    }

    [Fact]
    public Task CanGoBack_ActionRegisteredOnRoot_NotifiesBinding() =>
        Ui.Run(async () =>
        {
            var manager = (NavigationManager)NavigationManager;
            DialogService.Show(null, new FirstViewModel());
            var owner = manager.CurrentView;
            var dismissed = new TaskCompletionSource<bool>();
            var states = new List<bool>();
            manager.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(manager.CanGoBack)) { states.Add(manager.CanGoBack); }
            };

            var dialog = CancellableActions.RunAsync(() => dismissed.Task, () => dismissed.SetResult(true), owner);

            try
            {
                Assert.Equal(
                    new[]
                    {
                        true
                    },
                    states);
            }
            finally
            {
                dismissed.TrySetResult(true);
                await dialog;
            }
            Assert.Equal(
                new[]
                {
                    true, false
                },
                states);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ViewLookup_CoveredViewCollected_RecreatesView(bool useDialogManager)
    {
        var covered = new FirstViewModel();
        DialogService.Show(null, covered);
        DialogService.Show(null, new SecondViewModel());
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var view = useDialogManager ? DialogManager.FindViewByViewModel(covered)?.RefObj : NavigationManager.GetViewForViewModel(covered);

        Assert.Same(covered, Assert.IsType<FirstView>(view).DataContext);
        Assert.IsType<SecondViewModel>(NavigationManager.CurrentViewModel);
    }

    [Fact]
    public void Close_CollectedCoveredView_LogsTypeWithoutRecreatingControl()
    {
        var logger = new Mock<ILogger<DialogManager>>();
        var manager = new DialogManager(
            new ViewLocatorBase
            {
                ForceSinglePageNavigation = true
            },
            logger: logger.Object);
        var service = new DialogService(manager);
        var navigation = manager.NavigationManager!;
        var covered = new FirstViewModel();
        var creations = 0;
        var coveredView = ShowAndCover();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(coveredView.IsAlive);

        var closed = service.Close(covered);

        Assert.True(closed);
        Assert.DoesNotContain(covered, navigation.History);
        Assert.Equal(1, creations);
        var logEntries = logger.Invocations
            .Where(x => x.Method.Name == nameof(ILogger.Log))
            .Select(x => x.Arguments[2])
            .OfType<IReadOnlyList<KeyValuePair<string, object>>>();
        var closeLog = Assert.Single(
            logEntries,
            entry => entry.Any(pair => pair.Key == "{OriginalFormat}" &&
                                       Equals(pair.Value, "Close View: {View}; ViewModel: {ViewModel}")));
        Assert.Contains(new("View", typeof(FirstView)), closeLog);
        Assert.Contains(new("ViewModel", typeof(FirstViewModel)), closeLog);

        [MethodImpl(MethodImplOptions.NoInlining)]
        WeakReference ShowAndCover()
        {
            navigation.Show(
                covered,
                new(
                    typeof(FirstView),
                    () =>
                    {
                        creations++;
                        return new FirstView();
                    }));
            var view = new WeakReference(navigation.CurrentView);
            service.Show(null, new SecondViewModel());
            return view;
        }
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public Task Close_ModalWrappedWithExplicitHandler_RespectsBothDecisions(bool handlerVeto, bool modelVeto) =>
        Ui.Run(async () =>
        {
            var owner = new FirstViewModel();
            var modal = new SecondViewModel
            {
                ClosingCancel = modelVeto
            };
            DialogService.Show(null, owner);
            var pending = DialogService.ShowDialogAsync(owner, modal);
            var handlerCalls = 0;
            var closedCalls = 0;
            var shouldClose = !handlerVeto && !modelVeto;
            var wrapper = NavigationManager.CurrentView.AsWrapper(
                NavigationManager,
                (view, args) =>
                {
                    handlerCalls++;
                    args.Cancel = handlerVeto;
                    view.ClosingConfirmed = !handlerVeto;
                });
            wrapper.Closed += (_, _) => closedCalls++;

            wrapper.Close();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, handlerCalls);
            Assert.Equal(shouldClose ? 1 : 0, closedCalls);
            Assert.Equal(shouldClose ? 1 : 0, modal.ClosedRaised);
            Assert.Equal(shouldClose, pending.IsCompleted);
            Assert.Same(shouldClose ? owner : modal, NavigationManager.CurrentViewModel);
            if (shouldClose) { await pending.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken); }
        });

    [Fact]
    public Task ShowDialogAsync_LoadCallbackThrows_RestoresOwner() =>
        Ui.Run(async () =>
        {
            var window = new NavigationRootWindow();
            var owner = new FirstViewModel();
            var modal = new LoadingModal();
            NavigationManager.Launch(window);
            DialogService.Show(null, owner);
            var wrapper = new ViewNavigationWrapper(NavigationManager, null);
            wrapper.Initialize(modal, new(typeof(SecondView), () => new SecondView()));
            var ownerView = DialogManager.FindViewByViewModel(owner);

            var show = () => wrapper.ShowDialogAsync(ownerView);

            await Assert.ThrowsAsync<InvalidOperationException>(show);
            Assert.Same(owner, Assert.Single(NavigationManager.History));
            Assert.Same(owner, NavigationManager.CurrentViewModel);
        });

    [Fact]
    public void FindViewByViewModel_ClosedViewOfSameTypeCached_ReturnsNull()
    {
        var owner = new FirstViewModel();
        DialogService.Show(null, owner);
        var cachedView = NavigationManager.CurrentView;
        NavigationManager.Close(owner);

        var result = DialogManager.FindViewByViewModel(new FirstViewModel());

        Assert.Null(result);
        GC.KeepAlive(cachedView);
    }

    [Fact]
    public void Show_ExistingModelWithSameViewType_ReusesView()
    {
        var model = new FirstViewModel();
        DialogService.Show(null, model);
        var view = NavigationManager.CurrentView;

        DialogService.Show(null, model);

        Assert.Same(view, NavigationManager.CurrentView);
    }

    [Fact]
    public async Task Show_ModalOwner_LeavesModalInFront()
    {
        var owner = new FirstViewModel();
        var modal = new SecondViewModel();
        DialogService.Show(null, owner);
        var pending = DialogService.ShowDialogAsync(owner, modal);

        DialogService.Show(null, owner);

        Assert.Same(modal, NavigationManager.CurrentViewModel);
        Assert.True(await NavigationManager.GoBackAsync());
        await pending.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        Assert.Same(owner, NavigationManager.CurrentViewModel);
    }

    [Fact]
    public void Show_ExistingModelWithDifferentViewType_UsesNewView()
    {
        var model = new FirstViewModel();
        DialogService.Show(null, model);

        NavigationManager.Show(model, new(typeof(SecondView), () => new SecondView()));

        Assert.IsType<SecondView>(NavigationManager.CurrentView);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Close_OwnerOfNestedModals_CancelsOwnedDialogs(bool wrapOwner)
    {
        var owner = new FirstViewModel();
        var modal = new SecondViewModel();
        DialogService.Show(null, owner);
        var ownerView = wrapOwner
            ? NavigationManager.CurrentView.AsWrapper(NavigationManager, (view, _) => view.ClosingConfirmed = true)
            : DialogManager.FindViewByViewModel(owner);
        var dialog = new ViewNavigationWrapper(NavigationManager, DialogManager.View_Closing);
        dialog.Initialize(modal, new(typeof(SecondView), () => new SecondView()));
        var first = dialog.ShowDialogAsync(ownerView);
        var second = DialogService.ShowDialogAsync(modal, new ThirdViewModel());

        NavigationManager.Close(owner);

        Assert.True(first.IsCanceled);
        Assert.True(second.IsCanceled);
        Assert.Empty(NavigationManager.History);
        Assert.False(await NavigationManager.GoBackAsync());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task GoBackAsync_ModalReturnsPastEditor_HonorsConfirmation(bool approve) =>
        Ui.Run(async () =>
        {
            var window = new NavigationRootWindow();
            var root = new FirstViewModel();
            var editor = new SecondViewModel
            {
                ClosingCancel = true,
                ClosingAsyncCancel = !approve
            };
            var modal = new ThirdViewModel();
            NavigationManager.Launch(window);
            DialogService.Show(null, root);
            DialogService.Show(null, editor);
            var pending = DialogService.ShowDialogAsync(root, modal);

            var closed = await NavigationManager.GoBackAsync();

            Assert.Equal(approve, closed);
            Assert.Equal(1, editor.ClosingAsyncRaised);
            Assert.Equal(approve ? 1 : 0, editor.ClosedRaised);
            Assert.Same(approve ? root : modal, NavigationManager.CurrentViewModel);
            Assert.Equal(approve ? 1 : 3, NavigationManager.History.Count);
            Assert.Equal(approve, pending.IsCompleted);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task GoBackAsync_PredecessorClosesDuringConfirmation_KeepsLastPage(bool constructManager) =>
        Ui.Run(async () =>
        {
            var navigation = constructManager ? new NavigationManager(DialogManager.View_Closing) : NavigationManager;
            var root = new FirstViewModel();
            var editor = new WaitingCloseViewModel();
            var definition = new ViewDefinition(typeof(FirstView), () => new FirstView());
            navigation.Show(root, definition);
            navigation.Show(editor, definition);
            var back = navigation.GoBackAsync();
            var wasPending = !back.IsCompleted;
            navigation.Close(root);

            editor.Approval.SetResult();
            var closed = await back;

            Assert.True(wasPending);
            Assert.False(closed);
            Assert.False(navigation.CanGoBack);
            Assert.Same(editor, Assert.Single(navigation.History));
        });

    private class WaitingCloseViewModel : FirstViewModel
    {
        public TaskCompletionSource Approval { get; } = new();

        public override void OnClosing(CancelEventArgs e) => e.Cancel = true;

        public override async Task OnClosingAsync(CancelEventArgs e)
        {
            await Approval.Task;
            e.Cancel = false;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Close_CoveredPageConfirmation_KeepsDialogsIndependent(bool closeModal) =>
        Ui.Run(async () =>
        {
            var navigation = NavigationManager;
            var definition = new ViewDefinition(typeof(FirstView), static () => new FirstView());
            var root = new FirstViewModel();
            var editor = new Mock<FirstViewModel> { CallBase = true };
            var modal = new SecondViewModel();
            var prompt = new ThirdViewModel();
            Task confirmation = null;
            editor.Setup(x => x.OnClosing(It.IsAny<CancelEventArgs>())).Callback<CancelEventArgs>(e => e.Cancel = true);
            editor.Setup(x => x.OnClosingAsync(It.IsAny<CancelEventArgs>()))
                .Returns(() => confirmation = navigation.ShowDialogAsync(prompt, definition, editor.Object));
            navigation.Show(root, definition);
            navigation.Show(editor.Object, definition);
            var shown = navigation.ShowDialogAsync(modal, definition, root);
            navigation.Close(editor.Object);
            Dispatcher.UIThread.RunJobs();
            Assert.Same(prompt, navigation.CurrentViewModel);

            if (closeModal) { navigation.Close(modal); }

            Assert.Equal(closeModal, shown.IsCompletedSuccessfully);
            Assert.False(confirmation.IsCompleted);
            Assert.Same(prompt, navigation.CurrentViewModel);
            Assert.True(await navigation.GoBackAsync());
            Assert.True(confirmation.IsCompletedSuccessfully);
            Assert.Same(closeModal ? editor.Object : modal, navigation.CurrentViewModel);
        });

    [Fact]
    public async Task Close_IntermediatePageCallbackThrows_CompletesRemovedDialog()
    {
        var owner = new FirstViewModel();
        var middle = new ThrowingCloseViewModel();
        var modal = new SecondViewModel();
        var definition = new ViewDefinition(typeof(FirstView), () => new FirstView());
        NavigationManager.Show(owner, definition);
        NavigationManager.Show(middle, definition);
        var pending = NavigationManager.ShowDialogAsync(modal, definition, owner);

        var act = () => NavigationManager.Close(modal);

        Assert.Throws<InvalidOperationException>(act);
        Assert.True(pending.IsCompletedSuccessfully);
        Assert.Same(owner, Assert.Single(NavigationManager.History));
        await pending;
    }

    private class ThrowingCloseViewModel : FirstViewModel
    {
        public override void OnClosed() => throw new InvalidOperationException("Close callback failed.");
    }

    [Fact]
    public void FindViewByViewModel_AssignedControl_ReturnsVisibleOwner()
    {
        var model = new FirstViewModel();
        var assigned = new FirstView
        {
            DataContext = model
        };
        DialogService.Show(null, model);
        NavigationManager.CurrentView = assigned;

        var owner = DialogManager.FindViewByViewModel(model);

        Assert.Same(assigned, owner.RefObj);
    }

    private class LoadingModal : FirstViewModel, IModalDialogViewModel
    {
        public bool? DialogResult => true;
        public override void OnLoaded() => throw new InvalidOperationException("Load callback failed.");
    }

    [Fact]
    public Task GoBackAsync_CoveredEditorOpensPopup_DismissesPromptBeforePage() =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(CreatePopupFactory());
            var navigation = manager.NavigationManager;
            var editor = new ConfirmCloseEditor(manager);
            navigation.Show(editor, new(typeof(FirstView), static () => new FirstView()));
            var modal = new SecondViewModel();
            var shown = manager.ShowDialogAsync(owner, modal);

            var close = navigation.GoBackAsync();
            Dispatcher.UIThread.RunJobs();
            var dismissed = await navigation.GoBackAsync();

            Assert.True(dismissed);
            Assert.False(await close.WaitAsync(TestTimeout, TestContext.Current.CancellationToken));
            Assert.False(shown.IsCompleted);
            Assert.Same(modal, navigation.CurrentViewModel);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task InlineDialog_ManagedContentIsBound_PreservesUpdates(bool dialogHost) =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(dialogHost ? new DialogHostDialogFactory() : CreatePopupFactory());
            var view = manager.NavigationManager.CurrentView;
            var source = new ContentControl
            {
                Content = "original"
            };
            using var binding = view.Bind(
                ContentControl.ContentProperty,
                new Binding(nameof(ContentControl.Content))
                {
                    Source = source,
                    Mode = BindingMode.TwoWay
                });
            var prompt = dialogHost
                ? manager.ShowFrameworkDialogAsync(owner, new DialogHostSettings("Confirm?"))
                : manager.ShowFrameworkDialogAsync(
                    owner,
                    new MessageBoxSettings
                    {
                        Content = "Confirm?"
                    });
            Dispatcher.UIThread.RunJobs();

            Assert.False(prompt.IsCompleted);
            Assert.Equal("original", source.Content);
            source.Content = "updated";
            Assert.True(await manager.NavigationManager.GoBackAsync());
            await prompt.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            Assert.Equal("updated", source.Content);
            Assert.Equal("updated", view.Content);
        });

    [Fact]
    public Task DialogHost_AutomaticSurface_UsesOwnersDataTemplate() =>
        Ui.Run(async () =>
        {
            var (manager, window, owner) = OpenHost(new DialogHostDialogFactory());
            var navigation = manager.NavigationManager;
            navigation.CurrentView.DataTemplates.Add(
                new FuncDataTemplate<SecondViewModel>((_, _) => new TextBlock
                {
                    Text = "Page-local template"
                }));
            var prompt = manager.ShowFrameworkDialogAsync(owner, new DialogHostSettings(new SecondViewModel()));
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.Contains(window.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "Page-local template");
            Assert.True(await navigation.GoBackAsync());
            await prompt.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
        });

    [Fact]
    public Task Popups_WhenOwnerDetaches_RestoreOriginalContent() =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(CreatePopupFactory());
            manager.AllowConcurrentDialogs = true;
            var navigation = manager.NavigationManager;
            var page = navigation.CurrentView;
            var original = new TextBlock
            {
                Text = "Owner content"
            };
            page.Content = original;
            var first = manager.ShowFrameworkDialogAsync(
                owner,
                new MessageBoxSettings
                {
                    Content = "First"
                });
            var second = manager.ShowFrameworkDialogAsync(
                owner,
                new MessageBoxSettings
                {
                    Content = "Second"
                });
            Assert.False(first.IsCompleted);
            Assert.False(second.IsCompleted);

            manager.Show(null, new SecondViewModel());
            await Task.WhenAll(first, second).WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            Assert.True(await navigation.GoBackAsync());

            Assert.Same(original, navigation.CurrentView.Content);
        });

    [Fact]
    public Task WindowPopups_BackThenWindowClose_DismissInOrderAndPreserveBinding() =>
        Ui.Run(async () =>
        {
            Application.Current.Styles.Add(new FluentAvaloniaTheme());
            var window = new Window();
            var source = new ContentControl
            {
                Content = "original"
            };
            using var binding = window.Bind(
                ContentControl.ContentProperty,
                new Binding(nameof(ContentControl.Content))
                {
                    Source = source,
                    Mode = BindingMode.TwoWay
                });
            var factory = CreatePopupFactory();
            Ui.ShowWindow(window);
            var owner = window.AsWrapper();
            var first = factory.ShowDialogAsync(owner, new MessageBoxSettings { Content = "First" });
            var second = factory.ShowDialogAsync(owner, new MessageBoxSettings { Content = "Second" });
            source.Content = "updated";

            Assert.True(CancellableActions.CancelLast(window));
            await second.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            Assert.False(first.IsCompleted);
            Assert.Equal("updated", window.Content);

            window.Close();
            await first.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            Assert.False(CancellableActions.CancelLast(window));
            Assert.Equal("updated", window.Content);
        });

    [Fact]
    public Task ContentDialog_WhenClosedCallbackThrows_NextDialogOpens() =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(new FluentDialogFactory());
            var first = manager.ShowFrameworkDialogAsync(
                owner,
                new ContentDialogSettings(new ThrowingCloseViewModel())
                {
                    CloseButtonText = "OK"
                });
            var next = new SignalOnLoaded();
            var second = manager.ShowFrameworkDialogAsync(
                owner,
                new ContentDialogSettings(next)
                {
                    CloseButtonText = "OK"
                });
            var navigation = manager.NavigationManager;

            Dispatcher.UIThread.RunJobs();
            Assert.True(await navigation.GoBackAsync());
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                first.WaitAsync(TestTimeout, TestContext.Current.CancellationToken));
            await next.Loaded.Task.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            Assert.True(await navigation.GoBackAsync());
            await second.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
        });

    [Fact]
    public Task TaskDialog_WhenOwnerDetaches_ReleasesBackRegistration() =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(new FluentDialogFactory());
            var content = new SecondViewModel
            {
                ClosingCancel = true
            };
            var pending = manager.ShowFrameworkDialogAsync(owner, new TaskDialogSettings(content));
            Dispatcher.UIThread.RunJobs();
            Assert.False(pending.IsCompleted);

            manager.NavigationManager.Close(owner);

            await pending.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            Assert.False(manager.NavigationManager.CanGoBack);
            Assert.Equal(0, content.ClosingRaised);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task FluentDialog_WhenLoadCallbackThrows_NextDialogOpens(bool taskDialog) =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(new FluentDialogFactory());
            var content = new ThrowOnLoaded();
            var next = new SignalOnLoaded();
            var first = taskDialog
                ? manager.ShowFrameworkDialogAsync(owner, new TaskDialogSettings(content))
                : manager.ShowFrameworkDialogAsync(
                    owner,
                    new ContentDialogSettings(content)
                    {
                        CloseButtonText = "OK"
                    });

            var second = manager.ShowFrameworkDialogAsync(
                owner,
                new ContentDialogSettings(next)
                {
                    CloseButtonText = "OK"
                });
            var failure = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                first.WaitAsync(TestTimeout, TestContext.Current.CancellationToken));
            await next.Loaded.Task.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);
            var closed = await manager.NavigationManager.GoBackAsync();
            await second.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);

            Assert.Equal("Load callback failed.", failure.Message);
            Assert.True(closed);
            Assert.False(manager.NavigationManager.CanGoBack);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task DialogHost_WhenLoadAndCancellableCloseThrow_NextDialogOpens(bool concurrent) =>
        Ui.Run(async () =>
        {
            var (manager, _, owner) = OpenHost(new DialogHostDialogFactory());
            manager.AllowConcurrentDialogs = concurrent;
            var host = new DialogHostAvalonia.DialogHost { IsMultipleDialogsEnabled = concurrent };
            manager.NavigationManager.CurrentView.Content = host;
            host.DialogClosing += FailCancellableClose;
            var existing = concurrent ? manager.ShowFrameworkDialogAsync(owner, new DialogHostSettings("Existing")) : null;
            var first = manager.ShowFrameworkDialogAsync(owner, new DialogHostSettings(new ThrowOnLoaded()));
            var next = concurrent ? null : manager.ShowFrameworkDialogAsync(owner, new DialogHostSettings("Next"));

            var failure = await Assert.ThrowsAsync<AggregateException>(() =>
                first.WaitAsync(TestTimeout, TestContext.Current.CancellationToken));
            Assert.Equal(2, failure.InnerExceptions.Count);
            if (existing != null)
            {
                Assert.True(existing.IsCompletedSuccessfully);
                Assert.Empty(host.CurrentSessions);
                Assert.False(manager.NavigationManager.CanGoBack);
            }
            host.DialogClosing -= FailCancellableClose;
            next ??= manager.ShowFrameworkDialogAsync(owner, new DialogHostSettings("Next"));
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Next", Assert.Single(host.CurrentSessions).Content);
            host.CurrentSession.Close();
            await next.WaitAsync(TestTimeout, TestContext.Current.CancellationToken);

            static void FailCancellableClose(object sender, DialogClosingEventArgs e)
            {
                if (e.CanBeCancelled && e.Session.Content is ThrowOnLoaded)
                {
                    throw new InvalidOperationException("Close callback failed.");
                }
            }
        });

    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(3);

    // Routes native Back events through an open navigation window.
    private Window OpenNavigationWindow()
    {
        var window = new NavigationRootWindow();
        NavigationManager.Launch(window);
        Ui.ShowWindow(window);
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    // Present a real navigation page so overlay attachment and detachment use the bound host.
    private (DialogManager Manager, Window Window, FirstViewModel Owner) OpenHost(IDialogFactory factory)
    {
        Application.Current.Styles.Add(new FluentAvaloniaTheme());
        Application.Current.Styles.Add(new DialogHostStyles());
        var window = new NavigationRootWindow();
        var manager = new DialogManager(
            new ViewLocatorBase
            {
                ForceSinglePageNavigation = true
            },
            factory,
            customNavigationRoot: window);
        Ui.ShowWindow(window);
        var owner = new FirstViewModel();
        manager.Show(null, owner);
        Dispatcher.UIThread.RunJobs();
        return (manager, window, owner);
    }

    // Use the popup mode that participates in Back navigation.
    private static MessageBoxDialogFactory CreatePopupFactory() => new() { Mode = MessageBoxMode.Popup };

    private class ConfirmCloseEditor(DialogManager manager) : FirstViewModel
    {
        public override void OnClosing(CancelEventArgs e) => e.Cancel = true;

        public override async Task OnClosingAsync(CancelEventArgs e)
        {
            var answer = await manager.ShowFrameworkDialogAsync(
                this,
                new MessageBoxSettings
                {
                    Content = "Discard edits?",
                    Button = MessageBoxButton.YesNoCancel
                });
            e.Cancel = answer is not true;
        }
    }

    private class ThrowOnLoaded : FirstViewModel
    {
        public override void OnLoaded() => throw new InvalidOperationException("Load callback failed.");
    }

    private class SignalOnLoaded : FirstViewModel
    {
        public TaskCompletionSource Loaded { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override void OnLoaded() => Loaded.TrySetResult();
    }
}
